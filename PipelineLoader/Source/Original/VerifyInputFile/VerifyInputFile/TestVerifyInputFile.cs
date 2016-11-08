using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using System.Configuration;
using System.Linq;
using Helper;

namespace VerifyInputFile
{


    [TestFixture]
    public class TestVerifyInputFile
    {
        Dictionary<string, clsFileList> importFiles = new Dictionary<string, clsFileList>();
        //FindFileFromFileSystem findFile = new FindFileFromFileSystem();
        WindART_SMIDataContext windARTDataContext = new WindART_SMIDataContext();

        //static string processName = "TestVerfiyInputFile";

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void Start()
        {
            Dictionary<string, clsFileList> verifyFiles = new Dictionary<string, clsFileList>();

            VerifyInputFile vif = new VerifyInputFile();

            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\f0001_37020102.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\37020202.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\37020302.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\37020103.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\090120070402046.txt";
            //string testFile = @"C:\PipelineData\999.TestFiles\3718\37180108.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\37020208.prn";
            //string testFile = @"C:\devsvn3\WindART\trunk\Source\PipelineLoader\TestFiles\37020104.prn";

            //string testFile = @"C:\PipelineData\999.TestFiles\4201.csv";
            //string testFile = @"C:\PipelineData\999.TestFiles\CSV_Ford1_from_8_05_to_12_06.csv";

            //string testFile = @"C:\PipelineData\999.TestFiles\37020906.prn";
            //string testFile = @"C:\PipelineData\999.TestFiles\37020308.prn";

            //string testFile = @"C:\PipelineData\999.TestFiles\GEC\2006\Greenlight, Mehoopany PA -0406-validated.csv";

            string testFile = @"C:\PipelineData\999.TestFiles\1308\13080505.prn";

            int testFileLocation = 1;
              


            

            verifyFiles.Add(testFile, new clsFileList() {FullFilePath=testFile, FileLocationId=testFileLocation});

            vif.IdentifyFileFormat(verifyFiles);

            //Assert.AreEqual(1, ((clsFileList)verifyFiles.Values.First()).FileLocationId);
            Assert.AreEqual(4, ((clsFileList)verifyFiles.Values.First()).FileFormatId);

        }

    }
}
