using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Helper;
using System.Configuration;

namespace VerifyInputFile
{
    class VerifyInputFile
    {
        WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

        static string processName = "VerfiyInputFile";

        public void Start()
        {
            Dictionary<string, clsFileList> verifyFiles = new Dictionary<string, clsFileList>();

            Utils.WriteToLogStart(processName);

            getFileList(verifyFiles);

            IdentifyFileFormat(verifyFiles);

            Utils.WriteToLogFinish(processName);

        }

        public void IdentifyFileFormat(Dictionary<string, clsFileList> verifyFiles)
        {
            List<String> importedFileContent;
            bool headersMatch = false;
            bool multiLineHeadersMatch = false;

            DocLibHelper dlh = new DocLibHelper();
            string destFilename = string.Empty;

            foreach (clsFileList vf in verifyFiles.Values)
            {

                importedFileContent = loadFileFromList(vf);
                if (importedFileContent == null)
                {
                    Utils.DeleteMissingFileinDB(vf);
                    continue;
                }

                // Did we load a file with content?
                // Only check data matches if we did
                if (importedFileContent != null && importedFileContent.Count > 0)
                {
                    multiLineHeadersMatch = false;
                    headersMatch = true;

                    // repeat whilst we are finding a header
                    // but mismatching the multiline header rows
                    while (headersMatch && !multiLineHeadersMatch)
                    {
                        headersMatch = checkHeaderMatchs(importedFileContent, vf);

                        // Now we have the fileFormatId, check the multi line details match
                        if (headersMatch)
                        {
                            multiLineHeadersMatch = checkMultiLineHeaderMatchs(importedFileContent, vf);
                        }
                    }
                }

                var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                          where processedFiles.Name == vf.FullFilePath && 
                                          processedFiles.FileStatus == (int)Utils.FileStatus.ReadyForImport 
                                          select processedFiles;

                foreach (ProcessFile pf in ProcessedFilesQuery)
                {
                    if (headersMatch && multiLineHeadersMatch)
                    {
                        // insert date from Julian date
                        insertDateFromJulianDate(importedFileContent);

                        // save file contents to file
                        destFilename = string.Format("{0}\\f{1}_{2}", ConfigurationSettings.AppSettings["FolderVerified"], vf.FileFormatId.ToString().PadLeft(4, '0'), vf.Filename);
                        saveToFile(importedFileContent, destFilename);

                        // remove original file
                        File.Delete(vf.FullFilePath);

                        // set db status
                        pf.FileStatus = (int)Utils.FileStatus.Verified;
                        pf.FileFormatId = vf.FileFormatId;
                        pf.Name = destFilename;
                        pf.SiteId = vf.SiteId;

                        windARTDataContext.SubmitChanges();

                        // set Sharepoint status
                        dlh.UpdateFileStatus(pf.ProcessFileId, Utils.FileStatus.Verified, (int)pf.FileFormatId, (int)pf.SiteId);

                        Utils.WriteToLog(Utils.LogSeverity.Info, processName, vf.FullFilePath + " verified as formart " + vf.FileFormatId);
                        unParkFile(vf);
                    }
                    else
                    {
                        parkFile(vf, pf);
                    }

                }
                
            }


        }
        private void unParkFile(clsFileList vf)
        {
            

            //once we have decided that the format, check to see if there is lingering file in parked
            //if there is, delete it. 
            string targetFilename = ConfigurationSettings.AppSettings["FolderParked"] + @"\" + vf.Filename;
            if (File.Exists(targetFilename))
            {
                File.Delete(targetFilename);
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, vf.Filename + " removed from parked files");
            }
        }


        private void parkFile(clsFileList vf, ProcessFile pf)
        {
            DocLibHelper dlh = new DocLibHelper();

            // move file into Parked folder
            string destFilename = ConfigurationSettings.AppSettings["FolderParked"] + @"\" + vf.Filename;
            Utils.CopyLocalFileFromTo(vf.FullFilePath, destFilename, true);

            // set db status
            pf.FileStatus = (int)Utils.FileStatus.Parked;
            pf.Name = destFilename;

            windARTDataContext.SubmitChanges();

            // set Sharepoint status
            dlh.UpdateFileStatus(pf.ProcessFileId, Utils.FileStatus.Parked, 0, 0);

            Utils.WriteToLog(Utils.LogSeverity.Warning, processName, vf.FullFilePath + " parked - unknown formart");
        }

        private void saveToFile(List<string> importedFileContent, string outputFile)
        {
            StreamWriter sw = File.CreateText(outputFile);

            for (int i = 0; i < importedFileContent.Count(); i++)
            {
                sw.WriteLine(importedFileContent[i]);
            }

            sw.Close();
        }

        private void insertDateFromJulianDate(List<string> importedFileContent)
        {
            bool headerFound = false;
            int startPos = 0;
            int endPos = 0;
            string timeYear = string.Empty;
            string timeDOY = string.Empty;
            string timeMinutes = string.Empty;
            DateTime timeNormalised = new DateTime();
            if (importedFileContent == null) return;
            for(int i=0;i<importedFileContent.Count(); i++)
            {
                // Once we've found the header row, we can start inserting the date
                if (headerFound)
                {
                    // year in column 3
                    startPos = importedFileContent[i].IndexOf(',');
                    startPos = importedFileContent[i].IndexOf(',', startPos+1);
                    endPos = importedFileContent[i].IndexOf(',', startPos+1);
                    if (startPos > 0 && endPos > 0)
                    {
                        timeYear = importedFileContent[i].Substring(startPos + 1, endPos - startPos - 1);

                        // day of year in column 4
                        startPos = importedFileContent[i].IndexOf(',', startPos + 1);
                        endPos = importedFileContent[i].IndexOf(',', startPos + 1);
                        timeDOY = importedFileContent[i].Substring(startPos + 1, endPos - startPos - 1);

                        // minutes in column 5
                        startPos = importedFileContent[i].IndexOf(',', startPos + 1);
                        endPos = importedFileContent[i].IndexOf(',', startPos + 1);
                        timeMinutes = importedFileContent[i].Substring(startPos + 1, endPos - startPos - 1);

                        timeNormalised = Utils.JulianToDateTime(timeYear, timeDOY, timeMinutes);
                        
                        //back the time off by 10 minutes so that it is referring to the row 
                        //with start time instead of end time

                        timeNormalised=timeNormalised.AddMinutes(-10);

                        importedFileContent[i] += "," + timeNormalised.ToString("dd-MMM-yyyy HH:mm");
                    }
                }
                else if (importedFileContent[i].Contains("Year,JulianDay,Time"))
                {
                    headerFound = true;
                }
            }
            
        }

        private bool checkHeaderMatchs(List<string> importedFile, clsFileList fl)
        {
            string dbHeaderRowContents = string.Empty;

            // only check file formats after the last one we checked
            var FileFormatQuery = from fileFormats in windARTDataContext.FileFormats
                                  where fileFormats.FileLocationId == fl.FileLocationId
                                  && fileFormats.FileFormatId > fl.FileFormatId
                                  orderby fileFormats.FileFormatId
                                  select fileFormats;

            foreach (FileFormat ff in FileFormatQuery)
            {
                int hr = (int)ff.HeaderRowNumber - 1;


                // Catch negative index values before they cause any harm
                if ((hr > importedFile.Count - 1) || (hr < 0))
                {
                    Utils.WriteToLog(Utils.LogSeverity.Warning, processName, String.Format("Found a HeaderRowNumber of {0} in a file with {1} lines.  File format {2}", hr, importedFile.Count, ff.FileFormatId));
                }
                else
                {

                    // Fix any abnormal characters, that Excel will have magic-ed away from us
                    // during the DefineUFL phase
                    // Treat raw file contnets and db contents in the same manner for consistency
                    dbHeaderRowContents = ff.HeaderRowContents;
                    dbHeaderRowContents = dbHeaderRowContents.Replace("\"", "");

                    importedFile[hr] = importedFile[hr].Replace("\"", "");

                    // Ensure our array is at least as big as the line we are trying to pull out
                    if (importedFile[hr] == dbHeaderRowContents)
                    {
                        fl.FileFormatId = ff.FileFormatId;
                        //fl.SiteId = (int)ff.siteId;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool checkMultiLineHeaderMatchs(List<string> importedFile, clsFileList fl)
        {
            var MultiLineHeaderRowsQuery = from multiLineHeaderRows in windARTDataContext.MultiLineHeaderRows
                                  where multiLineHeaderRows.FileFormatId == fl.FileFormatId
                                  select multiLineHeaderRows;

            foreach (MultiLineHeaderRow ml in MultiLineHeaderRowsQuery)
            {
                int hr = (int)ml.HeaderRowNumber - 1;

                string compareLine = importedFile[hr];

                // Sometimes we see trailing tabs which confuses the match
                // Maybe we will see other trailing delims that will cause trouble?
                while (compareLine.EndsWith("\t"))
                {
                    compareLine=compareLine.Remove(compareLine.Length - 1);
                }

                // MS 17 Oct 2008
                // Sometimes Excel strips away blank columns for us,
                // and this causes problems when comparing it to the raw file
                // e.g. we compare: XXX with XXX,,,,,,,,,
                // So we strip away the trailing commas
                // This may need to be tweaked later?
                while (compareLine.EndsWith(","))
                {
                    compareLine = compareLine.Remove(compareLine.Length - 1);
                }

                // Do they match?
                if (compareLine != ml.HeaderRowContents)
                {
                    return false;
                }
            }

            return true;
        }


        private List<String> loadFileFromList(clsFileList verifyFile)
        {
            if (!File.Exists(verifyFile.FullFilePath))
            {
                Utils.WriteToLog(Utils.LogSeverity.Error, processName, String.Format("{0} does not exist.", verifyFile.FullFilePath));
                Console.WriteLine("{0} does not exist.", verifyFile.FullFilePath);
                return null;
            }

            StreamReader sr = File.OpenText(verifyFile.FullFilePath);
            List<String> contents = new List<String>();
            String inputLine;

            while ((inputLine = sr.ReadLine()) != null)
            {
                // Remove any weird characters in the line
                contents.Add(inputLine.Replace('',' '));
            }
            sr.Close();

            return (contents);

            /*
            foreach (string s in contents)
            {
                Console.WriteLine(s);
            }
            */

        }

        private void showFileList(List<String> importFiles)
        {
            foreach (string f in importFiles)
            {
                Console.WriteLine(f);
            }
        }

        private void getFileList(Dictionary<string, clsFileList> verifyFiles)
        {
            Utils.ReadProcessFileList(verifyFiles, Utils.FileStatus.ReadyForImport);
        }



    }
}
