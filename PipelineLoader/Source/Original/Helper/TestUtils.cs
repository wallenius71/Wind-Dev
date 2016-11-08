using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Helper;
using System.Security;

namespace Helper
{
    [TestFixture]
    public class TestUtils
    {
        private static string processName = "Utils";

        [Test]
        public void TestWriteToLog()
        {
            int before;
            int after;

            WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

            var LogQuery = from logs in windARTDataContext.Logs
                                     select logs;

            before = LogQuery.Count();
            
            Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Testing writing to log");

            after = LogQuery.Count();

            // Something else might have logged at the same time
            // so we cannot assume after = before++

            Assert.IsTrue(after > before);
        }

        [Test]
        public void TestReadFile()
        {
            Byte[] fileContents = Utils.ReadFile("./Helper.dll.config", "TestClass");
            Assert.IsTrue(fileContents.Length > 0);
        }

        [Test]
        public void TestJulianToDateTime1()
        {
            DateTime res = Utils.JulianToDateTime("2002", "1", "10");
            Assert.AreEqual(new DateTime(2002, 1, 1, 0, 10, 0), res);
        }

        [Test]
        public void TestJulianToDateTime2()
        {
            DateTime res = Utils.JulianToDateTime("2002", "1", "120");
            Assert.AreEqual(new DateTime(2002, 1, 1, 1, 20, 0), res);
        }

        [Test]
        public void TestJulianToDateTime3()
        {
            DateTime res = Utils.JulianToDateTime("2002", "1", "1430");
            Assert.AreEqual(new DateTime(2002, 1, 1, 14, 30, 0), res);
        }

        [Test]
        public void TestJulianToDateTime4()
        {
            DateTime res = Utils.JulianToDateTime("2002", "32", "1430");
            Assert.AreEqual(new DateTime(2002, 2, 1, 14, 30, 0), res);
        }

        [Test]
        public void TestJulianToDateTime5()
        {
            // leap year
            DateTime res = Utils.JulianToDateTime("2008", "60", "1430");
            Assert.AreEqual(new DateTime(2008, 2, 29, 14, 30, 0), res);
        }

        [Test]
        public void TestJulianToDateTime6()
        {
            // non leap year version of above
            DateTime res = Utils.JulianToDateTime("2007", "60", "1430");
            Assert.AreEqual(new DateTime(2007, 3, 1, 14, 30, 0), res);
        }

        [Test]
        public void TestCopyFilesToDocumentLibrary()
        {
            Utils.CopyFilesToDocumentLibrary(new clsFileList() {FullFilePath="C:\\devsvn5\\trunk\\Source\\PipelineLoader\\TestFiles\\37020302.prn"} , Utils.FileStatus.Test, "Test");
        }

        /*
        [Test]
        public void TestEncryptString()
        {
            SecureString secret = string.Empty;
            string notSecret = string.Empty;
            const SecureString message = new SecureString("Pssst. This is secret", 25);

            secret = Encryption.EncryptString(message);

            notSecret = Encryption.DecryptString(secret);

            Assert.AreNotEqual(message, secret);

            Assert.AreEqual(message, notSecret);

        }
        */
    }
}
