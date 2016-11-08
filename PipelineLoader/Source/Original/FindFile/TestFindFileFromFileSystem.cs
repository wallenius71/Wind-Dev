using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using System.Configuration;
using System.Linq;
using Helper;

namespace FindFile
{


    [TestFixture]
    public class TestFindFileFromFileSystem
    {
        Dictionary<string, clsFileList> importFiles = new Dictionary<string, clsFileList>();
        FindFileFromFileSystem findFile = new FindFileFromFileSystem();
        string processName = "TestFindFileFromFileSystem";

        [SetUp]
        public void Setup()
        {
            //findFile.getFileList(importFiles);
        }

        [Test]
        public void CheckFileListLoaded()
        {
            findFile.getFileList(importFiles);

            Assert.IsTrue(importFiles.Count > 0);
        }

        [Test]
        public void CheckFileCopyWorks()
        {
            findFile.getFileList(importFiles);

            removeExistingFiles();

            // do the copying
            findFile.copyFilesToWorkingFolder(importFiles);

            // verify they exist now
            foreach (clsFileList fl in importFiles.Values)
            {
                Assert.IsTrue(File.Exists(fl.FullFilePath));
            }

            // clean up
            removeExistingFiles();
        }

        private void removeExistingFiles()
        {
            foreach (clsFileList fl in importFiles.Values)
            {
                if (File.Exists(System.Configuration.ConfigurationManager.AppSettings ["FolderReadyForImport"] + @"\" + fl.Filename))
                {
                    File.Delete(System.Configuration.ConfigurationManager.AppSettings["FolderReadyForImport"] + @"\" + fl.Filename);
                }
            }
        }

        [Test]
        public void CheckRemoveFilesAlreadyProcessed()
        {
            int beforeCount = 0;
            int afterCount = 0;

            findFile.getFileList(importFiles);
            findFile.copyFilesToWorkingFolder(importFiles);

            beforeCount = importFiles.Values.Count;

            findFile.removeFilesAlreadyProcessed(importFiles);

            afterCount = importFiles.Values.Count;

            // We should have less files in the list now
            Assert.LessOrEqual(afterCount, beforeCount);
        }

        [Test]
        public void TestMarkFilesAsRead()
        {
            Dictionary<string, clsFileList> dummyImportFiles = new Dictionary<string, clsFileList>();

            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            dummyImportFiles.Add("dummyFile", new clsFileList { Directory = @"c:\dummyFolder", Filename = "dummyFile" });

            var ProcessedFilesQuery = from processedFiles in windARTDataContext.ProcessFiles
                                      select processedFiles;

            int before = ProcessedFilesQuery.Count();

            Utils.RecordFileDetailsInDB(dummyImportFiles, processName);

            int after = ProcessedFilesQuery.Count();

            Assert.Greater(after, before);


        }


    }
}
