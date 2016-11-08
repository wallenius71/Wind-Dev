using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using System.IO;

namespace DecryptFile
{
    class DecryptFile
    {
        static string processName = "DecryptFile";

        public void Decrypt()
        {
            Dictionary<string, clsFileList> encryptedFiles = new Dictionary<string,clsFileList>();

            Utils.WriteToLogStart(processName);
            
            getFileList(encryptedFiles);

            processEncryptedFiles(encryptedFiles);

            Utils.WriteToLogFinish(processName);
        }

        public void processEncryptedFiles(Dictionary<string, clsFileList> encryptedFiles)
        {
            DocLibHelper dlh = new DocLibHelper();
            string newFilename = string.Empty;

            foreach (clsFileList cfl in encryptedFiles.Values)
            {
                //if (cfl.Encrypted)
                //{
                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Attempting to decrypt: {0}", cfl.FullFilePath));

                    // Ensure the destination file doesn't exist
                    // otherwise the NRG app will create a new filename!
                    newFilename = Utils.GetKeyFromConfig("FolderReadyForImport") + @"\" + Utils.DropExtension(cfl.Filename) + ".txt";
                    File.Delete(newFilename);


                    bool result = Utils.Exec(Utils.GetKeyFromConfig("NRGExecutable"), String.Format("/s \"{0}\" ", cfl.FullFilePath), true);

                    // Check new file exists
                    bool newFileExists = File.Exists(newFilename);

                    if (result && newFileExists)
                    {
                        // Update status in MOSS to decrypted
                        dlh.UpdateFileStatus(cfl.ProcessFileId, Utils.FileStatus.Decrypted, cfl.FileFormatId, cfl.SiteId);

                        // Update status in DB
                        Utils.UpdateStatusInDB(cfl, Utils.FileStatus.Decrypted);

                        // Remove original encrypted file
                        File.Delete(cfl.FullFilePath);

                        // add to db
                        // add to sharepoint

                        cfl.OriginalFullFilepath = cfl.FullFilePath;
                        cfl.ProcessFileId = 0;
                        cfl.FullFilePath = newFilename;
                        cfl.Encrypted = false;
                        // addNewFileToDB
                        Utils.RecordFileDetailsInDB(cfl, processName);

                        // addNewFileToSharePoint
                        Utils.CopyFilesToDocumentLibrary(cfl, Utils.FileStatus.ReadyForImport, processName);
                    }
                    else
                    {
                        try
                        {
                            Utils.WriteToLog(Utils.LogSeverity.Critical, processName, String.Format("Error decrypting: {0}", cfl.FullFilePath));

                            // Move problematic file
                            File.Delete(Utils.GetKeyFromConfig("FolderParked") + @"\" + cfl.Filename);
                            
                            if (File.Exists(cfl.FullFilePath))
                            {
                                File.Move(cfl.FullFilePath, Utils.GetKeyFromConfig("FolderParked") + @"\" + cfl.Filename);
                                // Update status in DB
                                Utils.UpdateStatusInDB(cfl, Utils.FileStatus.Parked);
                                // Update status in MOSS to decrypted
                                dlh.UpdateFileStatus(cfl.ProcessFileId, Utils.FileStatus.Parked, cfl.FileFormatId, cfl.SiteId);
                            }
                            else
                            {
                                //the file does not exist. Remove the entry from processfile and report the action
                                Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("File does not exist, removing from listing: {0}", cfl.FullFilePath));
                                Utils.DeleteMissingFileinDB(cfl);
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.WriteToLog(Utils.LogSeverity.Critical, processName, String.Format("Error moving file to parked folder: {0}", e.Message));
                        }
                    }
                //}
            }
        }

        private void getFileList(Dictionary<string, clsFileList> processFiles)
        {
            Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Getting filelist");

            Utils.ReadProcessFileList(processFiles, Utils.FileStatus.ReadyForDecyption);
        }
    }
}
