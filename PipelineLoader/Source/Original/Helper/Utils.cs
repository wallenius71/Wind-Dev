using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;

namespace Helper
{
    public class Utils
    {
        public enum FileStatus
        {
            ReadyForImport,
            ReadyForConvertion,
            Verified,
            Parked,
            Loaded,
            Converted,
            ReadyForDecyption,
            Decrypted,
            Test,
            UploadingToSharepoint
        }

        public enum LogSeverity
        {
            Info,
            Warning,
            Error,
            Critical
        }

        public static string processName = "Utils";

        public static void RecordFileDetailsInDB(clsFileList cfl, string logProcessName)
        {
            Dictionary<string, clsFileList> importFiles = new Dictionary<string,clsFileList>();

            importFiles.Add(cfl.FullFilePath, cfl);

            RecordFileDetailsInDB(importFiles, logProcessName);
        }

        public static void RecordFileDetailsInDB(Dictionary<string, clsFileList> importFiles, string logProcessName)
        {
            uint count = 0;
            Utils.FileStatus status;
            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            foreach (clsFileList readFile in importFiles.Values)
            {
                ProcessFile pf = new ProcessFile();

                pf.ProcessDate = DateTime.Now;
                pf.OriginalName = readFile.OriginalFullFilepath;
                pf.Name = readFile.FullFilePath;
                pf.FileLocationId = readFile.FileLocationId;

                status = Utils.GetNextFileStatus(readFile);
                //status = Utils.FileStatus.UploadingToSharepoint;
                pf.FileStatus = (int)status;

                windARTDataContext.ProcessFiles.InsertOnSubmit(pf);

                windARTDataContext.SubmitChanges();

                // Now we have the ProcessFile primary key
                readFile.ProcessFileId = pf.ProcessFileId;

                count++;
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, logProcessName, count.ToString() + " file(s) marked as read");

        }

        public static void CopyFilesToDocumentLibrary(clsFileList cfl, Utils.FileStatus status, string logProcessName)
        {
            Dictionary<string, clsFileList> importFiles = new Dictionary<string, clsFileList>();

            importFiles.Add(cfl.FullFilePath, cfl);

            CopyFilesToDocumentLibrary(importFiles, status, logProcessName);
        }
        
        public static void CopyFilesToDocumentLibrary(Dictionary<string, clsFileList> importFiles, Utils.FileStatus status, string logProcessName)
        {
            uint count = 0;
            //Utils.FileStatus status;

            if (ConfigurationSettings.AppSettings["SkipSharePoint"] != "True")
            {
                foreach (clsFileList copyFile in importFiles.Values)
                {
                    string copyFrom = copyFile.FullFilePath;
                    string copyTo = ConfigurationSettings.AppSettings["SharePointDocumentLibrary"] + copyFile.Filename;

                    // Override ready for import status if we are going for ReadForImport
                    // We might not agree
                    if (status == Utils.FileStatus.ReadyForImport)
                    {
                        //status = GetNextFileStatus(copyFile.Filename);
                        status = GetNextFileStatus(copyFile);
                    }

                    Utils.UploadFile(copyFrom, copyTo, status, copyFile.FileLocationId, copyFile.ProcessFileId, logProcessName);

                    count++;
                }
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, logProcessName, count.ToString() + " file(s) uploaded to SharePoint Document Library");

        }

        public static Utils.FileStatus GetNextFileStatus(clsFileList cf)
        {
            if (cf.IsExcel())
            {
                return Utils.FileStatus.ReadyForConvertion;
            }
            else if (cf.Encrypted)
            {
                return Utils.FileStatus.ReadyForDecyption;
            }
            else
            {
                return Utils.FileStatus.ReadyForImport;
            }
        }

        public static void UploadFile(string copyFrom, string copyTo, FileStatus fileStatus, int fileLocationId, int processFileId, string realProcessName)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("FileStatus", fileStatus);
            properties.Add("FileLocationId", fileLocationId);
            properties.Add("ProcessFileId", processFileId);

            Byte[] filecontents = ReadFile(copyFrom, realProcessName);

            DocLibHelper dlh = new DocLibHelper();

            if (filecontents != null)
            {
                dlh.Upload(copyTo, filecontents, properties);
            }

        }

        public static bool CopyLocalFileFromTo(string copyFrom, string copyTo, bool move)
        {
            int retry = 1;
            bool success = false;

            if (File.Exists(copyFrom))
            {
                // If file already exists in destination, delete it.
                // Retry 5 times - file may be in use (e.g. still undergoing a file copy)
                while (!success && retry <= 5)
                {
                    try
                    {
                        if (move)
                        {
                            if (File.Exists(copyTo))
                            {
                                File.Delete(copyTo);
                            }
                            File.Move(copyFrom, copyTo);
                        }
                        else
                        {
                            File.Copy(copyFrom, copyTo, true);
                        }
                        success = true;
                    }
                    catch
                    {
                        Utils.WriteToLog(Utils.LogSeverity.Warning, processName, string.Format("Problem copying file {0} to {1} on attempt #{2}", copyFrom, copyTo, retry));
                        retry++;

                        Thread.Sleep(1000 * 60); // 60 seconds

                    }
                }
            }
            else
            {
                Utils.WriteToLog(Utils.LogSeverity.Warning, processName, string.Format("Could not find file {0}", copyFrom));
                success = false;
            }
            return success;
        }

        //public static void MoveLocalFileFromTo(string moveFrom, string moveTo)
        //{
        //    if (File.Exists(moveFrom))
        //    {
        //        // If file already exists in destination, delete it.
        //        if (File.Exists(moveTo))
        //        {
        //            File.Delete(moveTo);
        //        }

        //        File.Move(moveFrom, moveTo);
        //    }
        //}

        public static Byte[] ReadFile(string copyFrom, string trueProcessName)
        {
            Byte[] filecontents = null;
            int retry = 1;

            while (filecontents == null & retry <= 5)
            {
                try
                {
                    System.IO.FileStream Strm = new System.IO.FileStream(copyFrom, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(Strm);
                    filecontents = reader.ReadBytes(int.Parse(Strm.Length.ToString()));
                    reader.Close();
                    Strm.Close();
                    Utils.WriteToLog(LogSeverity.Info, trueProcessName, String.Format("Success reading file '{0}' on attempt {1}", copyFrom, retry));
                }
                catch (Exception e)
                {
                    Utils.WriteToLog(LogSeverity.Error, processName, String.Format("Error reading file '{0}' on attempt {1}, error: {2}", copyFrom, retry, e.Message));
                    retry++;
                    Thread.Sleep(1000 * 60); // Wait 60 secs
                }
            }
            return filecontents;

        }

        public static void WriteToLog(LogSeverity severity, string process, string message)
        {
            try
            {
                WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

                Log newLog = new Log();

                // field is varchar(4000)
                if (message.Length > 4000)
                {
                    message = message.Substring(0, 4000);
                }
                newLog.Time = DateTime.Now;
                newLog.LogSeverityId = (byte)severity;
                newLog.Process = process;
                newLog.Message = message;

                windARTDataContext.Logs.InsertOnSubmit(newLog);
                windARTDataContext.SubmitChanges();
            }
            catch (Exception)
            {
                // Hmmm, we cannot write to the log...
            }
        }

        public static void DeleteMissingFileinDB(clsFileList updateFile)
        {
            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                      where processedFiles.ProcessFileId == updateFile.ProcessFileId
                                      select processedFiles;

            // set db status
            // there should be only one, but do a foreach anyway
            foreach (ProcessFile pf in ProcessedFilesQuery)
            {
                windARTDataContext.ProcessFiles.DeleteOnSubmit(pf);
            }
            windARTDataContext.SubmitChanges();

        }


        public static DateTime JulianToDateTime(string year, string doy, string hoursMinutes)
        {
            DateTime dtOut = new DateTime(int.Parse(year), 1, 1);
            dtOut = dtOut.AddDays(int.Parse(doy) - 1);

            switch(hoursMinutes.Length )
            {
                case 2:
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes));
                    break;
                case 3:
                    dtOut = dtOut.AddHours(double.Parse(hoursMinutes.Substring(0,1)));
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes.Substring(1,2)));
                    break;
                case 4:
                    dtOut = dtOut.AddHours(double.Parse(hoursMinutes.Substring(0,2)));
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes.Substring(2,2)));
                    break;
            }

            return dtOut;
        }

        public static bool Exec(string cmd, string arg, bool wait)
        {
            bool result = false;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = arg;
            result = proc.Start();
            if (wait)
            {
                proc.WaitForExit();
            }

            return result;
        }

        public static void ReadProcessFileList(Dictionary<string, clsFileList> verifyFiles, Utils.FileStatus fileStatus)
        {
            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            var processFilesCollection = from processFiles in windARTDataContext.ProcessFiles
                                         where processFiles.FileStatus == (int)fileStatus
                                         select processFiles;

            foreach (var pf in processFilesCollection)
            {
                clsFileList fl = new clsFileList();
                fl.FullFilePath = pf.Name;
                fl.FileLocationId = (int)pf.FileLocationId;
                if (pf.FileFormatId != null)
                {
                    fl.FileFormatId = (int)pf.FileFormatId;
                }
                //fl.Encrypted = pf.Encrypted.Equals('Y'); // should be a bool
                fl.ProcessFileId = pf.ProcessFileId;
                if (pf.SiteId != null)
                {
                    fl.SiteId = (int)pf.SiteId;
                }
                // never add the same file twice
                if (!verifyFiles.Keys.Contains(fl.FullFilePath))
                {
                    verifyFiles.Add(fl.FullFilePath, fl);
                }
            }
        }

        public static string DropExtension(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf('.'));
        }

        public static string GetKeyFromConfig(string key)
        {
            string val = ConfigurationSettings.AppSettings[key];

            if (val.Equals(""))
            {
                WriteToLog(LogSeverity.Critical, processName, key + " not found in the .config file");
            }

            return val;
        }

        public static void UpdateStatusInDB(clsFileList updateFile, FileStatus newStatus)
        {
            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                      where processedFiles.ProcessFileId == updateFile.ProcessFileId
                                      select processedFiles;

            // set db status
            // there should be only one, but do a foreach anyway
            foreach(ProcessFile pf in ProcessedFilesQuery)
            {
                pf.FileStatus = (int)newStatus;
            }
            windARTDataContext.SubmitChanges();

        }

        public static void WriteToLogStart(string inProcessName)
        {
            Utils.WriteToLog(Utils.LogSeverity.Info, inProcessName, "Started application");
        }

        public static void WriteToLogFinish(string inProcessName)
        {
            Utils.WriteToLog(Utils.LogSeverity.Info, inProcessName, "Finished application");
        }

    }
}
