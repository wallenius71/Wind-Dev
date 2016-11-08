//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework; 
//using System.IO;
//using System.Data.OleDb;
//using WindART.DAL;
//using System.Data;


//namespace TurbineDataUtility.Model
//{
//    [TestFixture]
//    public class TestModel
//    {
//        static string _tagfile = @"P:\JesseRichards\Operations\SourceTags\OLEDB\All_WAEI_Tags.csv";
//        static DataTable _data=new DataTable ();
//        static List<Tag> _tags = new List<Tag>();
//        static IDataRepository repository ;
        
//        [TestFixtureSetUp]
//        public static void setup()
//        {
//            _data = null;
//            repository=new DataRepository(_tagfile, DataSourceType.CSV);
//            _data = repository.GetAllData();
//        }

//        [Test]
//        public void TestPopulatingTag()
//        {

//            Tag tag = new Tag();
//            tag.TagIndex = 1;
//            tag.TagName = @"testTag";
//            tag.TagType = tagType.PermanentMet;

//            Assert.AreEqual("testTag", tag.TagName);
//            Assert.AreEqual(1, tag.TagIndex);
//            Assert.AreEqual(tagType.PermanentMet, tag.TagType);

//        }
//        [Test]
//        public void testTagImport()
//        {
                      
            
//            int RowCount=_data.Rows.Count;
           
//            string val=string.Empty;
            

//            for (int i = 0; i < RowCount; i++)
//            {
//                if (_data.Rows[i][0].GetType().ToString() == "System.DBNull")
//                    {
//                        val = default(string);
//                    }
//                    else
//                    {
//                        val = _data.Rows[i][0].ToString();
//                    }
//                Tag thisTag = new Tag();
//                thisTag.TagName = val;
//                thisTag.TagIndex = i;
//                _tags.Add(thisTag);
//            }

//            TagManager tagMgr = new TagManager(_tags);

            
                 


//        }
//        [Test]
//        public void TestProjectCategorization()
//        {
//            //tag manager creates the projects from the tag list 
//            TagManager mngr = new TagManager(_tags);
            
//            Console.WriteLine(_tags.Count);
//            foreach (string p in mngr.ProjectList.Keys)
//            {
//                Console.WriteLine(p);
//            }
//            Assert.AreEqual(14,mngr.ProjectList.Count  );
//        }
//        [Test]
//        public void testExtractProject()
//        {
//            string teststring = "set.test";
//            string result = Utils.GetTagElement(teststring, 1);
//            Assert.AreEqual("test", result);

//        }
//        [Test]
//        public void testProjectTagCounts()
//        {
//            TagManager mngr = new TagManager(_tags);
//            foreach (string p in mngr.ProjectList.Keys)
//            {
//                Console.WriteLine("project " + p + "tag count " + mngr.ProjectList[p].tags .Count);
//                Assert.Greater(mngr.ProjectList[p].tags.Count, 20);
//            }
           

//        }

//        [Test]
//        public void testDownLoadfromWindARTPI()
//        {
//            PIDownloader downloader = new PIDownloader(DataSourceType.Operations_PI);
//            DateTime start=DateTime.Parse("01/01/2014");
//            DateTime end =DateTime.Parse("1/2/");
//            //SortedDictionary <DateTime ,double> resultset= downloader.Download(@"BPMT.US.OR.00002.DPD.49_7.WS.135.Avg", start, end);

//           // Console.WriteLine(resultset.Count + " records downloaded");
//            //Assert.Greater(resultset.Count, 3000);


//        }

//        [Test]
//        public void testGettingSiteList()
//        {
//            DataRepository  windartsmi=new DataRepository ("10.128.10.8",DataSourceType.WindART_SQL  );
//            string siteSQL=@"select oldid from vwSiteWithPiSiteID";
//            DataTable sites=windartsmi .GetData(siteSQL);
//            Assert.AreEqual (347,sites.Rows.Count);

//        }

       
    
//    }

    
//}
