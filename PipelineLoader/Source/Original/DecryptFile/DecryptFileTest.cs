using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Helper;

namespace DecryptFile
{
    [TestFixture]
    class DecryptFileTest
    {
        [Test]
        public void ProcessEncryptedFilesTest()
        {
            Dictionary<string, clsFileList> encryptedFiles = new Dictionary<string,clsFileList>();
            clsFileList cfl = new clsFileList();
            DecryptFile df = new DecryptFile();

            //cfl.FullFilePath = @"C:\PipelineData\999.TestFiles\090120070402046.RWD";
            cfl.FullFilePath = @"C:\PipelineData\999.TestFiles\090120040516215.RWD";
            cfl.Encrypted = true;


            encryptedFiles.Add(cfl.FullFilePath, cfl);

            df.processEncryptedFiles(encryptedFiles);
        }
    }
}
