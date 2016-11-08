using System;
using System.Collections.Generic;
//using NUnit.Framework;
using WindART.DAL;
using System.Data;
using TurbineDataUtility.Model;
using System.Threading;

namespace OpsDataDownloader
{   
    
    public class TestDownloadcs
    {
        static string _tagfile = string.Empty;
        //@"\\10.92.83.205\public\JesseRichards\Operations\SourceTags\OLEDB\All_WAEI_Tags_NoMet.csv";
        static DataTable _data = new DataTable();
        static List<Tag> _tags = new List<Tag>();
        static IDataRepository repository;
        static TagManager tagMgr;
        DateTime _start = DateTime.Parse(DateTime.Today.AddMonths(-1).Month.ToString() + @"/01/" + DateTime.Today.Year.ToString());  //DateTime.Today.AddDays(-2.0);
        //DateTime start = DateTime.Today.AddDays(-2.0);
        DateTime _end = DateTime.Today;

        public TestDownloadcs(DateTime start, DateTime end, string tagfile)
        {
            _start = start;
            _end = end;
            _tagfile = tagfile;
        }
        public TestDownloadcs(int NumDays, string tagfile)
        {
            _end = DateTime.Today.AddHours(DateTime.Now.Hour);
            _start=_end.AddDays(-1);
            _tagfile = tagfile;
        }
        public TestDownloadcs()
        { }
        public static void setup()
        {
            repository = new DataRepository(_tagfile, WindART.DAL.DataSourceType.CSV);

           
            _data = repository.GetAllData();
            
           
            
            int RowCount=_data.Rows.Count;
           
            string val=string.Empty;
            
            //create tags list from dataTable 
            for (int i = 0; i < RowCount; i++)
            {
                if (_data.Rows[i][0].GetType().ToString() == "System.DBNull")
                    {
                        val = default(string);
                    }
                    else
                    {
                        val = _data.Rows[i][0].ToString();
                    }
                Tag thisTag = new Tag();
                thisTag.TagName = val;
                thisTag.TagIndex = i;
                _tags.Add(thisTag);
            }

            //create projects 
             tagMgr = new TagManager(_tags);
        }
        //// project has a collection of tags and is responsible for outputting its tags' data to file 
        //
        //// tag has data and attributes 
        //
        //// tag manager creates the projects from a single list of tags 
        //
        //// pidownloader takes datasourcetype to construct and download method returns data for each tag in each project, 
        //   start and end time passed 

        
        //2. set the dates and download all of the data for each project
       
        public void testDownLoadfromPI()
        {
            PIDownloader downloader = new PIDownloader();
            DateTime start = DateTime.Parse("3/1/2014");
            DateTime end = DateTime.Parse("4/2/2014");
            
            foreach (Project proj in tagMgr.ProjectList.Values)
            {
                foreach (Tag t in proj.tags )
                {
                    downloader.Download(t.TagName , start, end);
                    Console.WriteLine(t.TagName );
                }
            //after done downloading each tag output the file 
                string fileLoc=@"P:\JesseRichards\Operations\Operations Raw Data\03 2014";
                proj.OutputFile(fileLoc,start,end);
                Console.WriteLine("Output " + proj.name);
            }
           

          }


        //3. output the data after each project is finished downloading 

        //4. log whats been retreived and saved 

        public void testOutputData(string output)
        {
            string filename = output;
            
            PIDownloader downloader = new PIDownloader(DataSourceType.Operations_PI);

            foreach (Project p in tagMgr.ProjectList.Values)
            {
                foreach (Tag t in p.tags)
                {
                    int count = 0;
                    while (t.Data == null & count < 100)
                    {
                        try
                        {
                            count++;
                            t.Data = downloader.Download(t.TagName, _start, _end);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                           
                            Thread.Sleep(1000);
                        }
                    }
                    Console.WriteLine(t.TagName);
                   

                }
                Console.WriteLine("outputting " + p.name);
               

                //output the projects data to csv before moving to the next 


                p.OutputFile(filename,  _start,_end);

            }
        }


    }
}
