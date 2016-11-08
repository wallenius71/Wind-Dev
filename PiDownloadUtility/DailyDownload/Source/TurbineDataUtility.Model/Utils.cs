using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Collections;



namespace TurbineDataUtility.Model
{
    public static class Utils
    {
        static Dictionary<tagType, List<string>> tagtypeParams = new Dictionary<tagType, List<string>> 
        {
            {tagType .PermanentMet ,new List<string>{".MET."}},
            {tagType .TurbineWindSpeed ,new List<string >{".TUR.",".Wind.Speed"}},
            {tagType .TurbineOther ,new List<string>{".TUR."}}
        };
        public static string GetTagElement(string tag, int section)
        {
            string answer = string.Empty;
            //return the nth tag element specified 
            if (tag != null && tag.Contains('.'))
            {

                string[] thisArray = tag.Split('.');
                answer = thisArray[section];
            }
            if (answer.Length > 0)
                return answer;
            else
                return string.Empty;
        }
        public static string GetTagElement(string tag, int section,char seperator)
        {
            string answer = string.Empty;
            //return the nth tag element specified 
            if (tag != null && tag.Contains(seperator ))
            {

                string[] thisArray = tag.Split(seperator);
                answer = thisArray[section];
            }
            if (answer.Length > 0)
                return answer;
            else
                return string.Empty;
        }
        public static tagType GetTagType(string tag)
        {
            foreach (KeyValuePair<tagType, List<string>> kv in tagtypeParams)
            {
                foreach (string s in kv.Value)
                {

                }
            }
            return default(tagType);
        }
        public static string GetFile()
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Title = "Select File";
            fd.ShowDialog();
            return fd.FileName;
        }
        public static string GetFolder()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();

            fd.Description = "Select Folder";
            

            DialogResult result = fd.ShowDialog();

            if (result == DialogResult.OK)
            {
                return fd.SelectedPath;
            }
            else
            {

                return "";
            }

        }
        public static bool AddColtoDataTable<T>(string AddColName, List<T> valArray, DataTable data)
        {
            //add a column of type T to datatable and return true if successful

            try
            {
                //add the new column if it does not exist 

                if (!data.Columns.Contains(AddColName))
                {
                    DataColumn newCol = new DataColumn();
                    newCol.DataType = typeof(T);
                    newCol.ColumnName = AddColName;
                    data.Columns.Add(newCol);

                    int i = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        row.BeginEdit();
                        row[AddColName] = valArray[i];
                        i++;
                    }
                    data.AcceptChanges();
                }
            }

            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
            return true;

        }
        public static List<DateTime> GetExpectedSequence(DateTime first, DateTime last, TimeSpan interval)
        {

            List<DateTime> result = new List<DateTime>();
            DateTime workdate = first;
            result.Add(workdate);
            while (workdate <= last)
            {
                workdate = workdate + interval;
                result.Add(workdate);
            }

            return result;


        }

        public static List<DateTime[]> DividedDateRange(DateTime start, DateTime end)
        {  //4-30-2014 no longer used anywhere 
            //return original dates as multiple 2 month date  ranges 
            List<DateTime[]> Work = new List<DateTime[]>();
            const int monthstoadd = 1;
            DateTime workend = start.AddMonths(monthstoadd);
            DateTime workstart = start;

            bool endNotReached = true;

            while (endNotReached)
            {
                if (workend >= end)
                {
                    //if the current date range is less than 
                    DateTime[] thisDateRange = new DateTime[2] { workstart, workend.AddSeconds(-1) };
                    Work.Add(thisDateRange);
                    endNotReached = false;
                }
                else
                {

                    //add this to the list
                    DateTime[] thisDateRange = new DateTime[2] { workstart, workend.AddDays(-1) };
                    Work.Add(thisDateRange);
                    workstart = workstart.AddMonths(monthstoadd);
                    workend = workend.AddMonths(monthstoadd);


                }

            }
            return Work;
        }
        public static int GetHourOffset(string project)
        {
            //looks in setting to return the number of hours the file's timestamps should be offset
            int offset = (int)Settings1.Default[project];
            return offset;
        }
        public static string GetLongName(string project)
        {
            project = project + "_LongName";
            //looks in setting to return the long name mapped to the project abbreviation found in the pi tag 
            string LongName = (string)Settings1.Default[project];
            return LongName;
        }
        public static List<Tag> GetOrderedColumns(Project proj)
        {
            List<Tag> res = new List<Tag>();
           
            string[] filesfound = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\");

            foreach(string file in filesfound)
            {
                string thisfile=Path.GetFileName (file);

                if(thisfile.Contains (proj.name))
                {
                    
                    StreamReader sr =new StreamReader (file);
                    string[] orderedtags=sr.ReadLine().Split(',');
                   // Console.WriteLine(orderedtags.Length);
                        foreach(string tag in orderedtags)
                        {
                           // Console.WriteLine("source tag:" + tag);
                            Tag tagfound=proj.tags.Where (c=>c.TagName.Equals (tag.Trim())).FirstOrDefault();

                            if (tagfound != null)
                            {
                                //Console.WriteLine(tag + " Found");
                                //add this tag to the output tag list
                                res.Add(proj.tags[proj.tags.IndexOf(tagfound)]);
                            }
                            //else
                            //{
                            //    Console.WriteLine("Not Found" + tag);
                            //}
                        }
                    sr.Close();
                }
               
            }

            
                return res;
            }

            
        }
    }


