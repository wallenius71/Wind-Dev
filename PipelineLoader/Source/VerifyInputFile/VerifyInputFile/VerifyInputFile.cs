using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Helper;
using System.Configuration;
using System.Diagnostics;

namespace VerifyInputFile
{
    class VerifyInputFile
    {
        WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

        static string processName = "VerfiyInputFile";
        //****production setting************************************************ 
        static string rootLocation = @"D:\";
        //**********************************************************************

        //******************************************************debug setting 
        //static string rootLocation = @"\\10.254.13.3\";
        //******************************************************************
        
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

            //DocLibHelper dlh = new DocLibHelper();
            string destFilename = string.Empty;

            foreach (clsFileList vf in verifyFiles.Values)
            {
               // Console.WriteLine(vf.Directory + " " + vf.Filename);
                //Console.ReadLine();

                importedFileContent = loadFileFromList(vf);
                //Console.WriteLine("nothing imported for file " + importedFileContent == null);
               // Console.ReadLine();


                if (importedFileContent == null)
                {
                   // Console.WriteLine("attempting to delete file");
                    //Console.ReadLine();

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

                        //Console.WriteLine("Header matched: " + headersMatch);
                        //Console.ReadLine();

                        // Now we have the fileFormatId, check the multi line details match
                        if (headersMatch)
                        {
                           // Console.WriteLine("Attempting to match multiline header rows");
                           // Console.ReadLine();
                           
                            multiLineHeadersMatch = checkMultiLineHeaderMatchs(importedFileContent, vf);
                           
                           // Console.WriteLine("multilines matched: " + multiLineHeadersMatch);
                           // Console.ReadLine();
                        }
                    }
                }

                //******* comment out for debug
                vf.FullFilePath = vf.FullFilePath.Replace(@"\\10.254.13.3\", @"D:\");

                var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                          where processedFiles.Name== vf.FullFilePath &&
                                          processedFiles.FileStatus == (int)Utils.FileStatus.ReadyForImport
                                          select processedFiles;

                foreach (ProcessFile pf in ProcessedFilesQuery)
                {
                   
                    if (headersMatch && multiLineHeadersMatch)
                    {
                        // insert date from Julian date
                        insertDateFromJulianDate(importedFileContent);

                        //delete any marked garbage columns that would cause an error on bulk import 
                        deleteColumns(importedFileContent,vf.FileFormatId,vf );

                        // save file contents to file
                        destFilename = string.Format("{0}\\f{1}_{2}", rootLocation + @"PipelineData\4.Verified", vf.FileFormatId.ToString().PadLeft(4, '0'), vf.Filename);
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
                        //dlh.UpdateFileStatus(pf.ProcessFileId, Utils.FileStatus.Verified, (int)pf.FileFormatId, (int)pf.SiteId);

                        Utils.WriteToLog(Utils.LogSeverity.Info, processName, vf.FullFilePath + " verified as format " + vf.FileFormatId);
                        unParkFile(vf);
                    }
                    else
                    {
                        //Console.WriteLine("Parking File");
                        //Console.ReadLine();
                        parkFile(vf, pf);
                    }

                }

            }


        }
        
        private void unParkFile(clsFileList vf)
        {


            //once we have decided that the format, check to see if there is lingering file in parked
            //if there is, delete it. 
            string targetFilename = rootLocation +  @"PipelineData\5.Parked" + @"\" + vf.Filename;
            if (File.Exists(targetFilename))
            {
                File.Delete(targetFilename);
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, vf.Filename + " removed from parked files");
            }
        }

        private void parkFile(clsFileList vf, ProcessFile pf)
        {
            //DocLibHelper dlh = new DocLibHelper();

            // move file into Parked folder
            string destFilename = rootLocation + @"PipelineData\5.Parked" + @"\" + vf.Filename;
            
            //Console.WriteLine("Parked location: " + destFilename);
            Console.ReadLine();

            Utils.CopyLocalFileFromTo(vf.FullFilePath, destFilename, true);

            //Console.WriteLine("copied file to parked");
            Console.ReadLine();

            // set db status
            pf.FileStatus = (int)Utils.FileStatus.Parked;
            pf.Name = destFilename;

            //Console.WriteLine("attempting to write parked status to db");
            Console.ReadLine();

            windARTDataContext.SubmitChanges();

            // set Sharepoint status
            //dlh.UpdateFileStatus(pf.ProcessFileId, Utils.FileStatus.Parked, 0, 0);

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
            List<int> badrows = new List<int>();

            for (int i = 0; i < importedFileContent.Count(); i++)
            {
                
                    // Once we've found the header row, we can start inserting the date
                    if (headerFound)
                    {
                        // year in column 3
                        startPos = importedFileContent[i].IndexOf(',');
                        startPos = importedFileContent[i].IndexOf(',', startPos + 1);
                        endPos = importedFileContent[i].IndexOf(',', startPos + 1);
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
                            try
                            {
                                timeNormalised = Utils.JulianToDateTime(timeYear, timeDOY, timeMinutes);

                                //back the time off by 10 minutes so that it is referring to the row 
                                //with start time instead of end time

                                timeNormalised = timeNormalised.AddMinutes(-10); 
                                importedFileContent[i] += "," + timeNormalised.ToString("dd-MMM-yyyy HH:mm");
                            }
                            catch //store bad rows. for now. we won't delete yet because it would screw up the iteration 
                            {
                                badrows.Add(i);
                            }

                           
                        }
                       
                                
                    }
                    else if (importedFileContent[i].Contains("Year,JulianDay,Time"))
                    {
                        headerFound = true;
                    }
               
            }
            //remove rows where there is no good date
            foreach (int j in badrows)
            {

                importedFileContent.Remove(importedFileContent[j]);
            }
        }

        private void deleteColumns(List<string> importedFileContent, int fileFormatID,clsFileList vf)
        {
            string delimiter=string.Empty;
            string delim =windARTDataContext.UFLDetails.Where(c => c.UFLDetailId == fileFormatID).Select(c => c.Delimitor).Single().ToString();
            if (delim == "TAB") delimiter = "\t";
            else delimiter = ",";

            List<int> GarbageCols = new List<int>();
           
            //get header row number from db
            int headerRow=(int)windARTDataContext.FileFormats .Where(c=>c.FileFormatId ==fileFormatID).Select(c=>c.HeaderRowNumber  ).First();
           
            List<string> row = new List<string>();
            string UpdatedRow = string.Empty;

            string header = windARTDataContext.UFLDetails.Where(c => c.UFLDetailId == fileFormatID).Select(c => c.PiTagList).Single();


              List<string> headerCols = header.Split('|').ToList();          

            if (importedFileContent == null) return;

            //find columns marked as N/A and change values to a missing numeric value
            //this is done because many of the N/A columns have text in them and cause the bulk insert to fail
            int count = 0;
            foreach (string  s in headerCols)
            {
                if (s == "N/A")
                {
                    GarbageCols.Add(count);
                }

                count++;
            }
            if (GarbageCols.Count > 0)
            {
                for (int i = headerRow; i < importedFileContent.Count(); i++)
                {
                    row = importedFileContent[i].Split(delimiter.ToCharArray ()).ToList();

                    foreach (int j in GarbageCols)
                    {
                        row[j] = "-9999.99";
                    }


                    //save updated row back to the comma delim data 
                    string seperator = string.Empty;
                    foreach (string s in row)
                    {
                       
                            seperator = delimiter ;
                        
                            
                        UpdatedRow +=  s + seperator;


                    }

                    //remove trailing comma 
                    UpdatedRow = UpdatedRow.Remove(UpdatedRow.Length - 1);

                    importedFileContent[i] = UpdatedRow;
                    UpdatedRow = string.Empty;

                }
            }
        }

        private bool checkHeaderMatchs(List<string> importedFile, clsFileList fl)
        {
            string dbHeaderRowContents = string.Empty;
            System.Linq.IOrderedQueryable<FileFormat> FileFormatQuery;
            // only check file formats after the last one we checked
            if (fl.FileFormatId == 0)
            {//get everything on the first iteration 
                 FileFormatQuery = from fileFormats in windARTDataContext.FileFormats
                                      where fileFormats.FileLocationId == fl.FileLocationId
                                      where fileFormats.FileFormatId > fl.FileFormatId
                                      orderby fileFormats.FileFormatId descending
                                      select fileFormats;
            }
            else
            {//only choose formats less than ones we've already processed 
                 FileFormatQuery = from fileFormats in windARTDataContext.FileFormats
                                      where fileFormats.FileLocationId == fl.FileLocationId
                                      where fileFormats.FileFormatId < fl.FileFormatId
                                      orderby fileFormats.FileFormatId descending
                                      select fileFormats;
            }

            foreach (FileFormat ff in FileFormatQuery)
            {
                int hr = (int)ff.HeaderRowNumber - 1;


                // Catch negative index values before they cause any harm
                if ((hr > importedFile.Count - 1) || (hr < 0))
                {
                    Utils.WriteToLog(Utils.LogSeverity.Warning, processName, 
                        String.Format("Found a HeaderRowNumber of {0} in a file with {1} lines.  File format {2}", hr, importedFile.Count, ff.FileFormatId));
                }
                else
                {

                    // Fix any abnormal characters, that Excel will have magic-ed away from us
                    // during the DefineUFL phase
                    // Treat raw file contnets and db contents in the same manner for consistency
                    dbHeaderRowContents = ff.HeaderRowContents;
                    dbHeaderRowContents = dbHeaderRowContents.Replace("\"", "");

                    string fileHeaderRow = importedFile[hr].Replace("\"", "").Replace('"', ' ');
                   
                    // Ensure our array is at least as big as the line we are trying to pull out
                   
                    //comment out for production ******
                    //if (ff.FileFormatId == 446)
                    //{
                    //    Debug.WriteLine("found id");
                    //}
                    
                    //*************************
                    
                    if (fileHeaderRow  == dbHeaderRowContents)
                    {
                         fl.FileFormatId = ff.FileFormatId;
                        
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
                compareLine = compareLine.Replace("\"", "");

                // Sometimes we see trailing tabs which confuses the match
                // Maybe we will see other trailing delims that will cause trouble?
                while (compareLine.EndsWith("\t"))
                {
                    compareLine = compareLine.Remove(compareLine.Length - 1);
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
            List<string> cells = new List<string>();
            while ((inputLine = sr.ReadLine()) != null)
            {
                // Remove any weird characters in the line and don't insert blank lines 
               
                   contents.Add(inputLine.Replace('', ' '));
                



               
            }
            sr.Close();

            return (contents);

            

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
