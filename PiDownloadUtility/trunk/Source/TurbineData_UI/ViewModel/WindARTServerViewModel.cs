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
using WindART;
using System.IO;
using System.Data.SqlTypes;


namespace Analysis_UI
{
    public class WindARTServerViewModel:BaseDataSourceView 
    {
        List<string> _sites;
        SelectionList<string> _configs;
        
        
        RelayCommand _cancelDownloadCommand;
        RelayCommand _loadConfigsCommand;
        RelayCommand _outputFileCommand;
        //RelayCommand _selectTagsCommand;
        bool _downloadCancelled;
        int _downloadProgress;
        string _downloadProgressText;
        List<string> _sensorType;
        string _selectedDataType;
        Dictionary<string,Tag> _tagWithConfig = new Dictionary<string, Tag>();
        Dictionary<int, DateTime[]> ConfigDates = new Dictionary<int, DateTime[]>();
        
                
        //string _selectedsite;
        

        public WindARTServerViewModel()
        {
            start = DateTime.Today;
            end = DateTime.Today;
            DisplayName = @"WindArt Server";
            data = new DataRepository(DataSourceType.WindART_SQL);
            
            DataTable result = data.GetData(@"select state + '-' + oldid, rtrim(ss.sitestatus), convert(varchar(20),installed,101) ,convert(varchar(20),removed,101)
                                            from tblsite s
                                            join tblsitestatus ss on ss.sitestatusid=s.sitestatus order by oldid");


            List<string> sites = new List<string>();
            
            
            for (int i = 1; i < result.Rows.Count;i++ )
            {
                
                StringBuilder sb = new StringBuilder();

                string site = result.Rows[i][0].ToString();
                sb.Append(site);
                sb.Append("   ");
                sb.Append(result.Rows[i][1].ToString());
                sb.Append(" ");
                sb.Append(result.Rows[i][2].ToString());
                sb.Append(" to ");
                sb.Append(result.Rows[i][3].ToString());
                
                sites.Add(sb.ToString ());
            }
           
            Sites = sites;
            
        }

        #region properties
        
        public List<string> DataTypes
        {
            get
            {
                if (_sensorType == null)
                { _sensorType = new List<string>() { "DAT", "DPD" }; }
                return _sensorType;
            }
            

        }
        public string SelectedDataType
        {
            get { return _selectedDataType; }
            set
            {
                _selectedDataType = value;
                OnPropertyChanged("SelectedDataType");
            }
        }
        
        public bool DownloadCancelled
        {
            get{return _downloadCancelled ;}
            set
            {
                _downloadCancelled = value;
                OnPropertyChanged("DownloadCancelled");
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

        public List<string> Sites
        {
            get
            {
                return _sites;
            }
            private set
            {
                _sites = value;
                OnPropertyChanged("Sites");
            }

        }
        public string SelectedSite { get; set; }
        public SelectionList<string> Configs
        {
            get { return _configs; }
            set
            {
                
                _configs = value;
                OnPropertyChanged("Configs");
            }
        }
        public bool UseDateRange { get; set; }
        
        
        public override ObservableCollection<DataTable> DownloadedData
        {
            get
            {
                return _downloadedData;
            }

            set
            {
                _downloadedData = value;
                OnPropertyChanged("DownloadedData");
            }
        }

#endregion 

        #region commands

           public ICommand DownloadCommand
            {
                get
                {
                    if (_downloadCommand == null)
                        _downloadCommand = new RelayCommand(param => this.Download(), param => this.CanDownload());

                    return _downloadCommand;
                }
            }
           public ICommand LoadConfigsCommand
           {
               get
               {
                   if (_loadConfigsCommand == null)
                       _loadConfigsCommand = new RelayCommand(param => this.LoadConfigs(), param => this.CanLoadConfigs());

                   return _loadConfigsCommand;
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
           public ICommand OutputFileCommand
           {
               get
               {
                   if (_outputFileCommand == null)
                       _outputFileCommand = new RelayCommand(param => this.OutputFile(), param => this.CanOutputFile());

                   return _outputFileCommand;
               }
           }       
        #endregion 

        void LoadConfigs()
        {
                
                string sitename = SelectedSite.Substring(0, 7);
                GetConfigs(sitename);
            
        }
        bool CanLoadConfigs()
        {
            return true;
        }
        void Download()
        {
            _isDownloading = true;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            DownloadProgress = 0;
           
            
            
            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {

                DownloadProgress=e.ProgressPercentage;
            };

            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                PIDownloader pi = new PIDownloader(DataSourceType.WindART_SQL);
                
                DownloadProgressText = "Initializing Connection....";


                foreach (string config in _configs.selectedItems)
                { 
                    List<Tag> SelectedTags = new List<Tag>();
                    DownloadProgress = 0;
                    int tagdownloadcount=0;
                    string sitename =SelectedSite.Substring(0,7);

                    SelectedTags = SelectTags(sitename );

                    TagManager tmReference = null;

                    tmReference = new TagManager(SelectedTags,sitename);

                    TagManagers.Add(tmReference);

                    foreach (Tag tag in SelectedTags)
                    {
                        if (tag == null) continue;
                        string tagname=tag.TagName ;
                        
                        if (_downloadCancelled)
                        {
                            tmReference = null;
                            worker.ReportProgress(0);
                            worker.CancelAsync();
                            _downloadCancelled = false;
                            break;
                        }
                        SortedDictionary<DateTime, double> TagData = new SortedDictionary<DateTime, double>();


                        StringBuilder sql = new StringBuilder();
                        sql.Append(@"select time, value from piserver.piarchive..picomp where tag='");
                        sql.Append(tagname);
                        sql.Append("' and time between '");
                        sql.Append(tag.StartDate.ToShortDateString ());
                        sql.Append(" 00:00:00");
                        sql.Append("' and '");
                        sql.Append(tag.EndDate.ToShortDateString());
                        sql.Append(" 00:00:00");
                        sql.Append("' and value is not null");
   
                            //System.Diagnostics.Debug.Print(sql.ToString ());
                            DataTable pidata = data.GetData(sql.ToString ());
                            
                            foreach(DataRow row in pidata.Rows)
                            {
                                DateTime thisDate = (DateTime)row["time"];
                                if (!TagData.ContainsKey(thisDate))
                                {
                                    // Console.WriteLine(row["time"].ToString() + " " + row["value"].ToString());
                                    TagData.Add(thisDate, Double.Parse(row["value"].ToString()));
                                }
                                else
                                {
                                    // Console.WriteLine(row["time"].ToString() + " not found in " + tag.TagName);
                                    TagData[thisDate] = -9999.0;
                                }


                            }

                        tmReference[tag.TagName].Data = TagData;
                        
                        tagdownloadcount++;

                        int progress = (int)(double.Parse(tagdownloadcount.ToString()) / double.Parse(SelectedTags.Count().ToString()) * 100);
                        worker.ReportProgress(progress);
                    }

                    //add the downloaded data to a datatable 

                    DownloadedData.Add(CreateDateTimeDataTable(tmReference));

                }
                
            };
            
            worker.RunWorkerAsync();
           

            _isDownloading = false;
        }
        protected bool CanDownload()
         {
             if (!_isDownloading && _selectedDataType !=null)
                 return true;
             else
             {
                 return false;
             }
         }
        void CancelDownload()
        {
            DownloadCancelled= true;
        }
        bool CanCancelDownload()
        {
            if (!DownloadCancelled)
                return true;
            else
                return false;
        }

        public void  OutputFile()
         {
             OutPutFileLocation = TurbineDataUtility.Model.Utils.GetFolder();
             try
             {
                 foreach (DataTable Data in DownloadedData)
                 {
                     string filename = OutPutFileLocation + @"\" + SelectedSite.Substring (0,4) + "_" + "set_" + DownloadedData.IndexOf (Data) + "_" + DateTime.Now.ToShortDateString().Replace(@"/", "") + ".csv";

                     // Create the CSV file to which grid data will be exported.
                     StreamWriter sw = new StreamWriter(filename, false);

                     //column heads
                     foreach (DataColumn col in Data.Columns)
                     {
                         sw.Write(col.ColumnName.Replace("#", "_"));
                        // Console.WriteLine(col.ColumnName.Replace("#", "_"));
                         if (Data.Columns.IndexOf(col) < Data.Columns.Count - 1)

                             sw.Write(",");
                     }



                     sw.Write(sw.NewLine);


                      //Now write all the rows.
                     foreach (DataRow dr in Data.Rows)
                     {
                         for (int i = 0; i < Data.Columns.Count; i++)
                         {
                             if (!Convert.IsDBNull(dr[Data.Columns[i]]) & dr[Data.Columns[i]].ToString().Length > 0)
                             {
                                 sw.Write(dr[Data.Columns[i]].ToString());
                             }
                             else
                             {
                                 sw.Write("-9999.99");
                             }
                             if (i < Data.Columns.Count - 1)
                             {
                                 sw.Write(",");
                             }
                         }
                         sw.Write(sw.NewLine);
                     }
                     sw.Close();

                 }
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

        void GetConfigs(string oldid)
        {
            if (Configs != null) Configs.Clear();
            ConfigDates.Clear();
            string site = oldid.Split('-')[1];
            string state = oldid.Split('-')[0];
            string sql = "select siteid from tblsite where oldid ='" + site + "' and state like '" + state + "'";
            DataTable tbl = data.GetData(sql);
            int siteid = int.Parse(tbl.Rows[0][0].ToString());
            tbl.Clear();

            sql = "exec procsiteconfig " + siteid +"; delete from tblprocesskeyconfig; " ;
            tbl = data.GetData(sql);
            

            List<string> localConfigs = new List<string>();
            int i = 0;
            

            foreach (DataRow dr in tbl.Rows)
            {
                            DateTime  start = DateTime.Parse(dr[0].ToString());
                            DateTime end = DateTime.Parse(dr[1].ToString().Length == 0 ? DateTime.Today.ToString() : dr[1].ToString());

                //store date ranges for later 
                ConfigDates .Add(i,new DateTime[2]{start,end});

                //for list box
                StringBuilder sbldr = new StringBuilder();
                sbldr.Append(start.ToShortDateString ());
                sbldr.Append("  to  ");
                sbldr.Append(end.ToShortDateString ());

                localConfigs.Add( sbldr.ToString());
                i++;
            }
            tbl = null;

            Configs = new SelectionList<string>(localConfigs);




        }
        
        List<Tag>SelectTags(string oldid )
        {
            if (oldid.Length > 0)
            {

                string sql = "select siteid from tblsite where oldid='" + oldid.Split('-')[1] + "' and state like '" + oldid.Split('-')[0] + "'";
                DataTable tbl = data.GetData(sql);
                int siteid = int.Parse(tbl.Rows[0][0].ToString());
                
                List < Tag > resultList = new List<Tag>();
                
                foreach (var config in _configs )
                {
                    if (!config.IsSelected) continue;

                    int index = _configs.IndexOf(config );
                    
                    DateTime _start = default(DateTime); 
                    DateTime _end = default(DateTime);
                    if (!UseDateRange)
                    {
                        _start=DateTime.Parse(ConfigDates[index][0].ToShortDateString() + " 00:00:00");
                        _end=DateTime.Parse(ConfigDates[index][1].ToShortDateString() + " 00:00:00");
                        
                    }
                    else
                    {
                        _start = start;
                        _end = end;
                    }
                    sql = "exec procsitecomp '" + _start.ToShortDateString () + "','" + _end.ToShortDateString () + "'," + siteid ;

                    tbl = data.GetData(sql);

                    double height=0.0;
                    double orientation=0.0;
                    SqlInt16 sensortype=0;

                    foreach (DataRow dr in tbl.Rows)
                    {
                        string temp = dr[4].ToString();
                        if (temp.Length >0)  height= double.Parse(temp);

                        temp = dr[5].ToString().Replace("°","");
                        if (temp.Length > 0) orientation = double.Parse(temp);
                        
                        temp=dr["comptypeid"].ToString();
                        if (temp.Length > 0) sensortype = SqlInt16.Parse(temp);
                        
                        sql = "select pitag from tblpitag pt join tblscvpitag scvp on scvp.pitagid=pt.pitagid where sitecompversid=" + dr["SiteCompVersID"].ToString();
                        tbl = data.GetData(sql);
                        if (tbl != null)
                        {
                            foreach (DataRow drow in tbl.Rows)
                            {
                                Tag tag = new Tag(drow["PiTag"].ToString(),_start,_end){Height =height,Orientation =orientation,SensorType =sensortype };
                                resultList.Add(tag);
                            }
                        }
                    }
                }
                //select only tags from the list that the user wants 
                var finalResult = from tag in resultList
                                  where tag.TagNameElement(4).Equals(_selectedDataType) 
                                  select tag;
                
                return finalResult.DefaultIfEmpty().ToList();
                
            }
            else
                return null;

            
        }
        
    }
}
