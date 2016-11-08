using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System;
using Helper;

namespace FindFile
{
    class FindFileFromFileSystem
    {
        // Done: Read SP list back again
        // Done: Update Attributes
        // TO DO: Delete file from SP?
        // Done: Logging 
        // TO DO: Rejected file -> reprocess path?
        // Done: insert data via linq
        // Done: Create PI tags
        // Done: Verify file
        // Done: UFL Loader
        // Done: Unit Test
        // Done: sync db
        // TO DO: Encrypt db string in app.config
        // TO DO: Recursive files seraching for FindFile


        // Each module should be separate .exe

        // FIND FILE TO IMPORT
        // Get filepath
        // Get list of matching all files in filepath
        // Remove from list any files in ProcessFile table
        // Attempt load of first file in list

        // VERIFY FILE
        // Check header matches
        // If fail, log and abort import of this file

        // BUILD UFL file
        // Read template file
        // Read Column Mappings
        // Create 
        // Create UFL from template and column mappings
        //      (tag name, date/time format, column mappings)

        // Start UFL


        WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

        Dictionary<string, clsFileList> importFiles = new Dictionary<string, clsFileList>();

        static string processName = "FindFile";

        public void Start()
        {
            Utils.WriteToLogStart(processName);

            getFileList(importFiles);
            
            
            removeFilesAlreadyProcessed(importFiles);
                      

            copyFilesToWorkingFolder(importFiles);

            // mark files as 'importing' and get primary id
            Utils.RecordFileDetailsInDB(importFiles, processName);

            // Upload files to Sharepoint list
            Utils.CopyFilesToDocumentLibrary(importFiles, Utils.FileStatus.ReadyForImport, processName);
             

            // Now the files are safely uploaded, we can set the status flag as ready to be imported
            uploadCompleted(importFiles);

            Utils.WriteToLogFinish(processName);
        }

        private void uploadCompleted(Dictionary<string, clsFileList> importFiles)
        {
            foreach (clsFileList item in importFiles.Values)
            {
                Utils.UpdateStatusInDB(item, Utils.GetNextFileStatus(item));
            }
        }

        public void copyFilesToWorkingFolder(Dictionary<string, clsFileList> importFiles)
        {
            uint count = 0;
            bool success = false;

            foreach (clsFileList copyFile in importFiles.Values)
            {
                string copyTo = string.Empty;
                string copyFrom = copyFile.OriginalFullFilepath;
                string storageFolder = string.Empty;

                if (copyFile.Encrypted)
                {
                    // Wait for decryption
                    copyTo = System.Configuration.ConfigurationManager.AppSettings["FolderDecrypt"] + @"\" + copyFile.Filename;
                    storageFolder = System.Configuration.ConfigurationManager.AppSettings["FolderStorage"] + @"\" + copyFile.Filename;
                    Utils.CopyLocalFileFromTo(copyFrom, storageFolder, false);
                }
                else if (copyFile.IsExcel())
                {
                    // Excel Folder
                    copyTo = System.Configuration.ConfigurationManager.AppSettings["FolderConvert"] + @"\" + copyFile.Filename;
                }
                else
                {
                    // Ready to import
                    copyTo = System.Configuration.ConfigurationManager.AppSettings["FolderReadyForImport"] + @"\" + copyFile.Filename;
                }

                Utils.WriteToLog(Utils.LogSeverity.Info, processName, string.Format("copying file {0} to {1}", copyFrom, copyTo));

                // With 'remote' file systems, we copy the files - don't want/have rights to delete originals
                // With the email folder, we move files out
                // This is so we can reload emails by marking them as unread
                success = Utils.CopyLocalFileFromTo(copyFrom, copyTo, copyFrom.Contains("0.EmailFolder"));

                if (!success)
                {
                    Utils.WriteToLog(Utils.LogSeverity.Warning, processName, string.Format("Problem copying file {0} to {1} - removing it from the list", copyFrom, copyTo));
                    importFiles.Remove(copyFile.FullFilePath);
                }
                else
                {
                    copyFile.FullFilePath = copyTo;
                }

                count++;
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, processName, count.ToString() + " file(s) moved to working folder");
        }

        public void copyFilesToStorageFolder(Dictionary<string, clsFileList> importFiles)
        {
            //replacing sharepoint with simple copy to folder for storage of original data files 
            uint count = 0;
            
           

            foreach (clsFileList copyFile in importFiles.Values)
            {
                string copyTo = string.Empty;
                string copyFrom = copyFile.OriginalFullFilepath;


                copyTo = @"D:\PipelineData\00.PreProcess\" + copyFile.Filename;
               //copyTo = ConfigurationSettings.AppSettings["FolderStoreOriginals"] + @"\" + copyFile.Filename;
               Utils.WriteToLog(Utils.LogSeverity.Info, processName, string.Format("copying file {0} to {1}", copyFrom, copyTo));

                //copy all files to storage folder 
                if (!File.Exists(copyTo ))
                Utils.CopyLocalFileFromTo(copyFrom, copyTo, false);
                                

                count++;
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, processName, count.ToString() + " file(s) moved to original data folder");
        }
        public void removeFilesAlreadyProcessed(Dictionary<string, clsFileList> importFiles)
        {
            // Remove any files we have already processed
            // Always re-import files that have arrived via email
            var processFilesCollection = from processFiles in windARTDataContext.ProcessFiles
                                         where importFiles.Keys.Contains(processFiles.OriginalName)
                                         && !processFiles.OriginalName.Contains("0.EmailFolder")
                                         select processFiles.OriginalName;

            uint count = 0;

            foreach (var pf in processFilesCollection)
            {
                importFiles.Remove(pf);
                count++;
            }

            Utils.WriteToLog(Utils.LogSeverity.Info, processName, count.ToString() + " file(s) ignored as they are already processed");
        }
        public void getFileList(Dictionary<string, clsFileList> fileList)
        {
            var FileLocationsQuery = from fileLocations in windARTDataContext.FileLocations
                                     /* where fileLocations.Enabled == true */
                                     select fileLocations;

            foreach (var v in FileLocationsQuery)
            {
                DirectoryInfo di = new DirectoryInfo(v.Folder);
                try
                {
                    FileInfo[] rgFiles = di.GetFiles(v.FileMask, v.Recurse == true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                    foreach (FileInfo fi in rgFiles)
                    {
                        // Ensure we haven't already matched this file from a previous loop
                        if (!fileList.ContainsKey(fi.FullName))
                        {
                            clsFileList fl = new clsFileList();
                            fl.Filename = fi.Name;
                            fl.Directory = fi.Directory.FullName;
                            fl.OriginalFullFilepath = fl.FullFilePath;
                            fl.FileLocationId = (int)v.FileLocationId;
                            fl.Encrypted = (bool)v.Encrypted;
                            fileList.Add(fi.FullName, fl);
                        }
                        // Creates too much logging
                        //Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Found file " + fi.FullName);
                    }
                }
                catch(Exception e)
                {
                    Utils.WriteToLog(Utils.LogSeverity.Error, processName, "Error reading files from: " + v.Folder + "\n" + e.Message);
                }
            }

        }
        public void getFileList(Dictionary<string, clsFileList> fileList, string folder, string fileMask, bool recursive)
        {
                DirectoryInfo di = new DirectoryInfo(folder);
                FileInfo[] rgFiles;

                rgFiles = di.GetFiles(fileMask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foreach (FileInfo fi in rgFiles)
                {
                    clsFileList fl = new clsFileList();
                    fl.Filename = fi.Name;
                    fl.Directory = fi.Directory.FullName;
                    fl.FileLocationId = 0;
                    fileList.Add(fi.FullName, fl);

                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Found file " + fi.FullName);
                }
        }



    }
}
