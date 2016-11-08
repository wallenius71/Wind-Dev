using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using System.Configuration;
using System.IO;


namespace UFLLoader
{
    class UFLLoader
    {
        WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

        static string processName = "UFLLoader";

        public void Start()
        {
            Dictionary<string, clsFileList> processFiles = new Dictionary<string, clsFileList>();
            List<int> uflFormats = new List<int>();

            Utils.WriteToLogStart(processName);
            
            // Find files for processing
            getFileList(processFiles);

            // Find file formats waiting to be processed
            uflFormats = getUFLFormats(processFiles);

            // Start UFL for waiting file formats
            startUFL(uflFormats);

            //startUFL(processFiles);

            // Check filenames exist with .OK extension
            // E.g. f001_37020102_21-May-2008_17-31-43.453._OK
            checkFilesLoaded(processFiles);

            Utils.WriteToLogFinish(processName);
        }

        private void startUFL(Dictionary<string, clsFileList> processFiles)
        {
            string appPath = ConfigurationSettings.AppSettings["PI_UFLExec"];
            string uflParams = ConfigurationSettings.AppSettings["PI_UFLParams"];
            string iniFolder = ConfigurationSettings.AppSettings["PI_UFLIniFolder"];
            string appParams = string.Empty;
            bool res = false;

            foreach (clsFileList fl in processFiles.Values)
            {
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Attempting to load UFLFormat_{0}", fl.FileFormatIdPadded()));
                appParams = string.Format("{0} /cf=\"{1}{2}\" ", uflParams, iniFolder, "UFLFormat_" + fl.FileFormatIdPadded() + ".ini");
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Using command: {0}, {1}", appPath, appParams));
                res = Utils.Exec(appPath, appParams, true);
            }
        }

        private void checkFilesLoaded(Dictionary<string, clsFileList> processFiles)
        {
            string findFile = string.Empty;
            string destFilename = string.Empty;

            DocLibHelper dlh = new DocLibHelper();

            // Check if we have anything to do
            if (processFiles.Count == 0)
            {
                return;
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Checking files have loaded");

            foreach (clsFileList f in processFiles.Values)
            {
                // PI UFL renames the files to use the following format:
                // <original filename><datetime>._OK
                findFile = String.Format("{0}*._OK", f.Filename.Substring(0, f.Filename.Length - 4));
                
                DirectoryInfo di = new DirectoryInfo(f.Directory);
                FileInfo[] rgFiles = di.GetFiles(findFile);

                foreach (FileInfo fi in rgFiles)
                {
                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "File loaded: " + fi.FullName);

                    destFilename = string.Format("{0}\\{1}", ConfigurationSettings.AppSettings["FolderLoaded"], f.Filename);

                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Moving file to {0}", destFilename));

                    Utils.CopyLocalFileFromTo(fi.FullName, destFilename, true);

                    // log status in db
                    var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                              where processedFiles.Name == f.FullFilePath && processedFiles.FileStatus == (int)Utils.FileStatus.Verified
                                              select processedFiles;

                    foreach (ProcessFile pf in ProcessedFilesQuery)
                    {
                        pf.FileStatus = (int)Utils.FileStatus.Loaded;
                        pf.Name = destFilename;

                        windARTDataContext.SubmitChanges();

                        // set Sharepoint status
                        dlh.UpdateFileStatus(pf.ProcessFileId, Utils.FileStatus.Loaded, f.FileFormatId, f.SiteId);
                    }
                }
            }
        }

        private void startUFL(List<int> uflFormats)
        {
            string appPath = ConfigurationSettings.AppSettings["PI_UFLExec"];
            string uflParams = ConfigurationSettings.AppSettings["PI_UFLParams"];
            string iniFolder = ConfigurationSettings.AppSettings["PI_UFLIniFolder"];
            string appParams = string.Empty;
            bool res = false;

            foreach (int uflFormat in uflFormats)
            {
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Attempting to load UFLFormat_{0}", uflFormat.ToString().PadLeft(4, '0')));
                appParams = string.Format("{0} /cf=\"{1}{2}\" ", uflParams, iniFolder, "UFLFormat_" + uflFormat.ToString().PadLeft(4, '0') + ".ini");
                res = Utils.Exec(appPath, appParams, true);
            }
        }

        private List<int> getUFLFormats(Dictionary<string, clsFileList> processFiles)
        {
            List<int> uflFormats = new List<int>();

            foreach (clsFileList f in processFiles.Values)
            {
                if (!uflFormats.Contains(f.FileFormatId))
                {
                    uflFormats.Add(f.FileFormatId);
                }
            }

            return (uflFormats);
        }

        private void getFileList(Dictionary<string, clsFileList> processFiles)
        {
            Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Getting filelist");

            Utils.ReadProcessFileList(processFiles, Utils.FileStatus.Verified);
        }

    }
}
