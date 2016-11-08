using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WindART.DAL;
using System.Data;
using System.ComponentModel;
using System.Windows.Input;
using TurbineDataUtility.Model;
using System.IO;
using System.Windows;
using System.Threading;

using NUnit.Framework;
using System.Diagnostics;

namespace Analysis_UI
{
    public class ProjectViewModel : BaseDataSourceView
    {


        int _downloadProgress;
        string _downloadProgressText;
        DataSourceType _dst;
       
        RelayCommand _outputFileCommand;
        RelayCommand _cancelDownloadCommand;
        Project _project;
        bool _useColumnOrderTemplate;
        bool _LoopDownload;
        int _loopDays;


        public ProjectViewModel(Project project)
        {
            start = DateTime.Today;
            end = DateTime.Today;
            _project = project;
            DisplayName = _project.name;
           

        }
        public ProjectViewModel(Project project, DataSourceType dst)
        {
            start = DateTime.Today;
            end = DateTime.Today;
            _project = project;
            DisplayName = _project.name;
            _dst = dst;

        }

        #region properties



        public Project Project
        {
            get { return _project; }

            set
            {
                _project = value;
                OnPropertyChanged("Project");
            }
        }




        public bool UseOrder
        {
            get { return _useColumnOrderTemplate; }
            set
            {
                _useColumnOrderTemplate = value;
                OnPropertyChanged("UseOrder");
            }
        }



        public override ObservableCollection<DataTable> DownloadedData
        {
            get { return _downloadedData; }
            set
            {
                _downloadedData = value;
                OnPropertyChanged("DownloadedData");
            }
        }
        public string DownloadProgressText
        {
            get
            {
                return _downloadProgressText;
            }

            set
            {
                _downloadProgressText = value;
                OnPropertyChanged("DownloadProgressText");

            }
        }
        public int DownloadProgress
        {
            get
            {
                return _downloadProgress;
            }

            set
            {
                _downloadProgress = value;
                OnPropertyChanged("DownloadProgress");

            }
        }
        public bool DownloadIsCanceled
        {
            get;
            set;
        }
        public bool UseColumnOrderTemplate
        {
            get { return _useColumnOrderTemplate; }
            set { _useColumnOrderTemplate = value; }
        }
        public bool LoopDownload
        {
            get {return _LoopDownload; }
            set 
            {
                _LoopDownload =value;
                OnPropertyChanged("LoopDownload");
            }
        }
        public int LoopDays {
            get { return _loopDays; }
            set
            {
                _loopDays = value;
                OnPropertyChanged("LoopDays");
            }
        }

        #endregion
        #region commands



        public ICommand OutputFileCommand
        {
            get
            {
                if (_outputFileCommand == null)
                    _outputFileCommand = new RelayCommand(param => this.OutputFile(param), param => this.CanOutputFile());

                return _outputFileCommand;
            }
        }
        public ICommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                    _downloadCommand = new RelayCommand(param => this.Download(param), param => this.CanDownload());

                return _downloadCommand;
            }
        }
        public ICommand CancelDownloadCommand
        {
            get
            {
                if (_cancelDownloadCommand == null)
                    _cancelDownloadCommand = new RelayCommand(param => this.CancelDownload(), param => this.CanCancelDownload());

                return _cancelDownloadCommand;
            }
        }


        #endregion
        #region methods

        void Download(object param)
        {   
            //pass the output file location 
            //manage the download according to if we are only getting a single segment or if we need to loop over auto incremented segments 
            Dictionary<DateTime, DateTime> dateset = new Dictionary<DateTime, DateTime>();
            
                if (param.ToString() == "")
                {
                    MessageBox.Show("Specify an output location before downloading");
                    return;
                }
                if (LoopDownload)
                {      //create a dateset to loop through 
                    DateTime workstart = start;
                    DateTime workend = start;
                    DateTime originalEnd = end;
                    DateTime originalStart = start;

                    while (workend < originalEnd)
                    {
                        if (workend == start)
                        {
                            workend = workend.AddDays(LoopDays);

                        }
                        else
                        {
                            workend = workend.AddDays(LoopDays);
                            workstart = workstart.AddDays(LoopDays);
                        }

                        dateset.Add(workstart, workend);
                    }

                }
                else
                    dateset.Add(start, end);

            _isDownloading = true;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;


            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                DownloadProgress = e.ProgressPercentage;
                Debug.WriteLine(DownloadProgress);
            };

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled != true)
                    OutputFile(param);

            };
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {

                PIDownloader pi = new PIDownloader(_dst);
                int tagdownloadcount = 0;

                Dictionary<DateTime, double> TagData = new Dictionary<DateTime, double>();

                TagManager tmReference = null;
                if (TagManagers.Count == 0)
                {
                    tmReference = new TagManager(_project.tags);
                    TagManagers.Add(tmReference);
                }
                else
                {
                    foreach (TagManager tm in TagManagers)
                    {
                        //if no tags are found--assumed to be a new dataset;add a new tag manager

                        tmReference = tm;
                        //add the tags that are not already in the tag manager's tags collection 
                        foreach (Tag t in _project.tags)
                        {
                            if (!tm.Tags.Contains(t))
                                tm.Tags.Add(t);
                        }
                    }
                }

                //download the tag data for the current dataset 
                foreach (KeyValuePair<DateTime, DateTime> dates in dateset)
                {
                                   
                    
                    if (DownloadIsCanceled)
                        {
                            worker.CancelAsync();
                            DownloadedData.Clear();
                            _isDownloading = false;
                            DownloadIsCanceled = false;
                            DownloadProgressText = string.Empty;
                            worker.ReportProgress(0);
                            break;
                        }
                    
                        start = dates.Key;
                        end = dates.Value;
                    foreach (Tag tag in _project.tags)
                    {
                        
                        bool runagain = true;
                        int runcount = 0;
                        while (runagain)
                        {
                            try
                            {
                                if (Compressed | Sampled)
                                {
                                    tmReference[tag.TagName].StringData = pi.Download(tag.TagName, start, end, Compressed, Sampled );
                                    if (tag.StringData != null)
                                        DownloadProgressText = tag.StringData.Count + " Records Downloaded: " + tag.TagName;
                                }
                                else
                                {
                                    tmReference[tag.TagName].Data = pi.Download(tag.TagName, start, end );
                                    if (tag.Data != null)
                                        DownloadProgressText = tag.Data.Count + " Records Downloaded: " + tag.TagName;
                                }

                                runagain = false;
                            }
                            catch (Exception e)
                            {
                                if (e.GetType() == typeof(OutOfMemoryException))
                                {
                                    DownloadProgressText = "Error Downloading " + tag.TagName + " " + e.Message + " Canceling..";
                                    worker.CancelAsync();
                                }
                                else
                                {
                                    //Console.WriteLine(tag.TagName + "   " + e.Message);
                                    DownloadProgressText = "Error Downloading " + tag.TagName + " " + e.Message + " Attempt " + runcount;
                                    runcount++;
                                    runagain = true;
                                }

                                
                            }
                        }

                        tagdownloadcount++;

                        int progress = (int)(double.Parse(tagdownloadcount.ToString()) / double.Parse(Project.tags.Count().ToString()) * 100);
                        Console.WriteLine(tag.TagName + " segment 4" + tag.TagNameElement(4) + " " + progress.ToString());
                        worker.ReportProgress(progress, Project);
                    } //foreach tag 
                    
                    //add the downloaded data to a datatable 
                    DownloadedData.Clear();
                    DownloadedData.Add(CreateDateTimeDataTable(tmReference));
                    //output the file 
                    OutputFile(param);
                    tagdownloadcount = 0;
                    
                } //foreach dateset
                _isDownloading = false;
            };

            worker.RunWorkerAsync();

                          
                
        }
      
        protected bool CanDownload()
        {
            if (LoopDownload)
            {
                if (LoopDays == default(int)||LoopDays<1)
                    return false;
                else
                    return true; 
            }
            return true;

        }
        void CancelDownload()
        {
            DownloadIsCanceled = true;
        }
        protected bool CanCancelDownload()
        {
            if (_isDownloading && !DownloadIsCanceled) return true;
            return false;
        }
        public bool OutputFile(object param)
        {
            string outputFileLocation = param as string;
            int offset = Utils.GetHourOffset(this.Project.name);

            string filename = outputFileLocation + @"\" + Project.name + "_" + start.ToShortDateString().Replace("/", "") + "_to_" + end.ToShortDateString().Replace("/", "") + @".csv";
            
            //string logfile = @"P:\0_Projects\US\0_Operations\_Monthly RAW pi data\PiDownloadLog.csv";

            // Create the CSV file to which grid data will be exported.
            StreamWriter sw = null;
            try
            {
                 sw = new StreamWriter(filename, false);

            }
            catch (IOException e)
            {
                MessageBox.Show(filename + " is already open and cannot be written to. Close the file and output again.");
                return false;
            }
            // StreamWriter log = new StreamWriter(logfile, true);

            //log.WriteLine(DateTime.Now + "," +  Project.name + ",time offset:" + offset + ",start:" + this.start  + ",end:" + this.end + ", output to:" + outputFileLocation  );
            if (!Compressed)
            sw.Write("DateTime, OffsetDateTime,");

            //column heads********************

            List<Tag> workTags = new List<Tag>();


            if (UseOrder)
            {
                //order the columns as they are ordered in this project's column order template resource 
                workTags.AddRange(TurbineDataUtility.Model.Utils.GetOrderedColumns(this.Project));

            }
            else
            {
                List<string> crit = new List<string>(2) { "TUR", "MET", "SUB"};
                foreach (string s in crit)
                {
                    //create correctly ordered tag lists
                    workTags.AddRange(Project.tags.Where(t => t.TagNameElement(4).Contains(s)).OrderBy(t => t.LastTagNameElement()));
                }
            }

            int max=0;
            //write the header row 
            foreach (Tag tag in workTags)
            {
                //find the max number of records across all tags 
                if (Compressed )
                {
                    if (tag.StringData != null)
                    {
                        if (tag.StringData.Count > max)
                            max = tag.StringData.Count;
                    }
                }
                else
                {
                    if (tag.Data != null)
                    {
                        if (tag.Data.Count > max)
                            max = tag.Data.Count;
                    }
                }

                tag.TagEmpty = true;
                if (Compressed)
                {
                    sw.Write("DateTime");
                    sw.Write(",");

                }
                sw.Write(tag.TagName);
                sw.Write(",");

            }

            sw.Write(sw.NewLine);

            //find first tag with data 
            int colwithdata;
            try
            {
                if (!Compressed)
                    colwithdata = workTags.IndexOf(workTags.Where(c => c.Data != null).Where(c => c.Data.Count > 2).First());
                else
                    colwithdata = workTags.IndexOf(workTags.Where(c => c.StringData != null).Where(c => c.StringData.Count > 2).First());
            }
            catch { colwithdata = 2; }
            //  write all the rows.
            //use the first column's datetime as the anchor datetime when writing the full dataset 
            //TODO:find max array length, write all columns to that row count 
                
            //todo refactor to factory pattern 
            //if we are not working with compressed data then handle dates and data one way 
            if (!Compressed & ! Sampled)
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
            else if (Sampled)
            {
                

                foreach (string d in workTags[colwithdata].StringData.Keys)
                {
                    DateTime date = DateTime.Parse(d);
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
                        if (tag.StringData == null)
                        {
                            sw.Write(",");
                            continue;
                        }
                        string val = string.Empty;

                        if (tag.StringData.ContainsKey(d))
                        {

                            val = tag.StringData [d];
                            tag.TagEmpty = false;
                        }
                        else
                            val = "-9999.0";

                        if (val == default(string))
                        {
                            val = "-9999.0";
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
            else if (Compressed) //if we are working with compressed data then handle it a different way
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
            }
            sw.Close();
            return true;

        }
        public override bool CanOutputFile()
        {
            return true;


        #endregion
        }
    }
}

