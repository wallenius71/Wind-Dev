using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace TurbineDataUtility.Model
{
    public class Project:INotifyPropertyChanged 
    {
        

        public string name{get;private set;}
        public List<Tag> tags { get;  set; }

        
    
        public Project(string projname)
        {
            name = projname;
            tags = new List<Tag>();
        }
        public List<List<Tag>> TagGroups()
        {
            //return different tag types as seperate lists
            return null;

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void SortByType()
        {

            tags = tags.OrderBy(t => t.TagNameElement (5) + t.LastTagNameElement () ).ToList ();
            OnPropertyChanged("tags");
        }
        public bool OutputFile(string outputFileLocation, DateTime start, DateTime end, bool Compressed)
        {
            
            int offset = Utils.GetHourOffset(this.name);

            string st = start.Month.ToString() + start.Day.ToString() + start.Year.ToString();
            string e = end.Month.ToString() + end.Day.ToString() + end.Year.ToString();

            // start.ToShortDateString().Replace("/", "") + "_to_" + end.ToShortDateString().Replace("/", "")
            string filename = outputFileLocation + @"\" + name +  "_" + st + "_to_" + e + "_ID" + DateTime.Now.Second.ToString() +  @".csv";

            //string logfile = @"P:\0_Projects\US\0_Operations\_Monthly RAW pi data\PiDownloadLog.csv";

            // Create the CSV file to which grid data will be exported.
            StreamWriter sw = new StreamWriter(filename, false);
            // StreamWriter log = new StreamWriter(logfile, true);

            //log.WriteLine(DateTime.Now + "," +  Project.name + ",time offset:" + offset + ",start:" + this.start  + ",end:" + this.end + ", output to:" + outputFileLocation  );
            
                sw.Write("DateTime, OffsetDateTime,");

            //column heads********************

            List<Tag> workTags = new List<Tag>();


           
                List<string> crit = new List<string>(2) { "TUR", "MET", "SUB" };
                foreach (string s in crit)
                {
                    //create correctly ordered tag lists
                    workTags.AddRange(tags.Where(t => t.TagNameElement(4).Contains(s)).OrderBy(t => t.LastTagNameElement()));
                }
          

            int max = 0;
            //write the header row 
            foreach (Tag tag in workTags)
            {
                
                    if (tag.Data != null)
                    {
                        if (tag.Data.Count > max)
                            max = tag.Data.Count;
                    }
               

                tag.TagEmpty = true;
                
                sw.Write(tag.TagName);
                sw.Write(",");

            }

            sw.Write(sw.NewLine);

            //find first tag with data 
            int colwithdata;
            try
            {
                
                    colwithdata = workTags.IndexOf(workTags.Where(c => c.Data != null).Where(c => c.Data.Count > 2).First());
               
            }
            catch { colwithdata = 2; }
            //  write all the rows.
            //use the first column's datetime as the anchor datetime when writing the full dataset 
            //TODO:find max array length, write all columns to that row count 

            //todo refactor to factory pattern 
            //if we are not working with compressed data then handle dates and data one way 
            if (!Compressed)
            {
                foreach (DateTime date in workTags[colwithdata].Data.Keys)
                {
                    DateTime thisdate;
                    if (date > DateTime.Parse("01/01/1950"))
                        thisdate = date.AddHours(offset);
                    else
                        continue;
                    sw.Write(date.ToShortDateString() + " " + date.Hour + ":" + date.Minute + ":" + date.Second);
                    sw.Write(",");
                    sw.Write(thisdate.ToShortDateString() + " " + thisdate.Hour + ":" + thisdate.Minute + ":" + thisdate.Second);
                    sw.Write(",");

                    foreach (Tag tag in workTags)
                    {
                        if (tag.Data == null)
                        {
                            sw.Write(",");
                            continue;
                        }
                        double val = default(double);

                        if (tag.Data.ContainsKey(date))
                        {

                            val = tag.Data[date];
                            tag.TagEmpty = false;
                        }
                        else
                            val = -9999.0;

                        if (val == default(double))
                        {
                            val = -9999.0;
                        }
                        sw.Write(val);

                        if (!tag.TagName.Equals(workTags.Last()))
                        {
                            sw.Write(",");
                        }

                    }
                    sw.Write(sw.NewLine);
                }
            }
            else //if we are working with compressed data then handle it a different way
            {
                int thisCount = 0;
                string line = string.Empty;
                DateTime thisdate = default(DateTime);
                DateTime date = default(DateTime);
                bool writeNewLine = false;

                for (int i = 0; i < max; i++) //iterate down row by row
                {

                    foreach (Tag tag in workTags)
                    {
                        //how many records of data does this tag have? 
                        if (tag.StringData != null)
                            thisCount = tag.StringData.Count;
                        else
                            thisCount = 0;

                        //if the iterator is still within the tag count then get the date 
                        if (i < thisCount)
                        {
                            date = DateTime.Parse(tag.dates[i]);
                            if (date > default(DateTime).AddYears(1))
                            {
                                thisdate = date.AddHours(offset);
                                line += thisdate + "," + tag.values[i];
                                writeNewLine = true; //only takes one good value to print the line 
                            }
                            else
                            {
                                line += ",";

                            }

                        }
                        else  //if the iteration is greater than the count of recs then write blanks 
                        {
                            line += ",";
                        }

                        //write another column or move to another row? 
                        if (!tag.TagName.Equals(workTags.Last().TagName))
                        {
                            line += ",";
                        }
                        else
                        {
                            if (writeNewLine)
                            {
                                sw.Write(line);
                                sw.Write(sw.NewLine);
                                line = string.Empty;
                            }
                            else
                            {
                                line = string.Empty;
                            }
                        }
                    }//foreach tag

                }//i
            }// if compressed 
            sw.Close();
            return true;

        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
