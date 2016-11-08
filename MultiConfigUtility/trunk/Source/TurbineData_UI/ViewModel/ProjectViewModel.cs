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
using Analysis_UI.Properties;

namespace Analysis_UI
{
    public class ProjectViewModel:BaseDataSourceView 
    {
        

        int _downloadProgress;
        string _downloadProgressText;
        DataSourceType _dst;
        
        RelayCommand _outputFileCommand;
        RelayCommand _cancelDownloadCommand;
        Project _project;
        

         public ProjectViewModel(Project project)
        {
            start = DateTime.Today;
            end = DateTime.Today;
            _project = project;
            DisplayName = _project.name;
            _dst = DataSourceType.Operations_PI;

        }
         public ProjectViewModel(Project project,DataSourceType dst)
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
            get { return _project;}
            
            set
            {
                _project = value;
                OnPropertyChanged("Project");
            }
        }
        public override ObservableCollection<DataTable> DownloadedData
        {
            get { return _downloadedData; }
            set { _downloadedData = value;
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
                        _downloadCommand = new RelayCommand(param => this.Download(), param => this.CanDownload());

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

        void Download()
        {
            _isDownloading = true;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            
            
            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                DownloadProgress=e.ProgressPercentage;
            };

            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                PIDownloader pi = new PIDownloader(_dst);
                int tagdownloadcount=0;

                Dictionary<DateTime, double> TagData = new Dictionary<DateTime, double>();

                TagManager tmReference=null;
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
                    foreach (Tag tag in _project.tags)
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
                        bool runagain=true;
                        int runcount = 0;
                        while (runagain)
                        {
                            try
                            {

                                tmReference[tag.TagName].Data = pi.Download(tag.TagName, start, end);
                                DownloadProgressText = tag.Data.Count + " Records Downloaded: " + tag.TagName;
                                runagain = false;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(tag.TagName + "   " + e.Message);
                                DownloadProgressText = "Error Downloading " + tag.TagName + " " + e.Message;
                                runcount++;
                                if(runcount >3)
                                { runagain = false; }
                                else
                                { runagain = true; }
                            }
                        }
                        
                        tagdownloadcount ++;
                        
                        int progress = (int)(double.Parse (tagdownloadcount.ToString ()) /double.Parse ( Project.tags.Count().ToString ())  * 100);
                        Console.WriteLine(tag.TagName + " segment 4" + tag.TagNameElement (4) + " " +  progress.ToString());
                       worker.ReportProgress (progress,Project  );
                    }
                    //add the downloaded data to a datatable 
                    DownloadedData.Clear();
                    DownloadedData.Add( CreateDateTimeDataTable(tmReference ));
                    
                    tagdownloadcount = 0;
               _isDownloading = false;
            };
            
            worker.RunWorkerAsync();

            
        }
        protected bool CanDownload()
         {
             return true;
                      
         }
        void CancelDownload()
        {
            DownloadIsCanceled = true;
        }
        protected bool CanCancelDownload()
        {
            if (_isDownloading && !DownloadIsCanceled)  return true;
            return false;
        }
        public bool OutputFile(object param)
        {
            string outputFileLocation = param as string;
                

                string filename = outputFileLocation + @"\" + Project.name + "_" + start .ToShortDateString ().Replace("/","") + "_to_" + end.ToShortDateString ().Replace("/","")+  @".csv";
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(filename, false);

                sw.Write("DateTime,");

                //column heads
                List<string> crit = new List<string>(2) { "TUR", "MET" };
                List<Tag> workTags = new List<Tag>();

                foreach (string s in crit)
                {
                    //create correctly ordered tag lists
                    workTags.AddRange (Project.tags.Where(t => t.TagNameElement(4).Contains(s)).OrderBy(t => t.LastTagNameElement()));
                }

                    foreach (Tag tag in workTags )
                    {

                        sw.Write(tag.TagName);
                        Console.WriteLine("output " + tag.TagName);
                        sw.Write(",");

                    }

                    sw.Write(sw.NewLine);



                    //  write all the rows.
                    foreach (DateTime date in workTags [0].Data.Keys)
                    {
                        sw.Write(date.ToShortDateString() + " " + date.Hour + ":" + date.Minute + ":" + date.Second);
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
                                val = tag.Data[date];
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
                
                
                sw.Close();
                return true;
            
        }
        public override bool CanOutputFile()
        {
            return true;
           
        }
        
      
        #endregion
    }
    }

