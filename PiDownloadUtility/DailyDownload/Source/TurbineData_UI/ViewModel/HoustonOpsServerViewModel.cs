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

using Analysis_UI;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Threading;


namespace Analysis_UI
{
    public class HoustonOpsServerViewModel:BaseDataSourceView 
    {

        string _selectedFile;
        DataSourceType _dataSource;
        RelayCommand _loadFileCommand;
        RelayCommand _checkFileCommand;
        //RelayCommand _outputFileCommand;
        SelectionList<SelectionItem<ProjectViewModel>> _projectList;
        List<Tag> _tags=new List<Tag> ();
        
        
         public HoustonOpsServerViewModel()
        {
            start = DateTime.Parse(DateTime.Now.AddMonths(-1).Month + @"/1/" + DateTime.Now.Year);
            end = DateTime.Today;
            _dataSource = DataSourceType.Operations_PI;
            LoadInputFile(false);
            

        }
         public HoustonOpsServerViewModel(DataSourceType dst)
         {
             start = DateTime.Parse(DateTime.Now.AddMonths(-1).Month + @"/1/" + DateTime.Now.Year);
             end = DateTime.Today;
             _dataSource = dst;
             LoadInputFile(false);


         }
        
 #region properties

        
         public string FileName
         {
             get { return filename; }
             set { filename = value; }
         }
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }
        public SelectionList<SelectionItem<ProjectViewModel>> ProjectList
        {
            get { return _projectList;}
            
            set
            {
                _projectList = value;
                OnPropertyChanged("ProjectList");
            }
        }
        public override ObservableCollection<DataTable> DownloadedData 
        {
            get
            {

                if (ProjectList == null) return null; 
                    foreach (SelectionItem<ProjectViewModel> p in ProjectList.AllItems)
                    {
                        if (p.SelectedItem.DownloadedData.Count > 0)
                        {
                            p.SelectedItem.DownloadedData.ToList().ForEach(delegate(DataTable d)
                            {
                                if(!_downloadedData.Contains(d))
                                _downloadedData.Add(d);
                            });
                        }
                    }
                
                return _downloadedData;
            }
            set { 
                _downloadedData = value; 
            }
        }
            

            
        

        #endregion 
 #region commands

        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                    _loadFileCommand = new RelayCommand(param => this.LoadInputFile(true), param => this.CanLoadInputFile());

                return _loadFileCommand;
            }
        }

        public ICommand CheckFileCommand
        {
            get
            {
                if (_checkFileCommand == null)
                    _checkFileCommand = new RelayCommand(param => this.CheckFiles(),param =>this.CanCheckFiles ());

                return _checkFileCommand;
            }
        }
        
#endregion
        
 #region methods
                
            void LoadInputFile(bool getfile)
        {
            string sourcefile = string.Empty;
            if (getfile) sourcefile = Utils.GetFile();

            try
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                _tags.Clear();
                worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {

                    //get the tags from the repository 
                    List<string> file = new List<string>();
                    //string sourcefile = 
                    if (getfile && sourcefile.Length >0)
                    {
                        DataRepository repository = new DataRepository( sourcefile , DataSourceType.CSV);
                        filename = string.Empty;
                        DataTable data = repository.GetAllData();
                        foreach (DataRow row in data.Rows)
                        {
                            file.Add(row[0].ToString());
                        }
                    }
                    //    else
                    //{
                    //     file = Properties.Resources.SourceTags.Split(',').Where(c => c.Length > 0).Select(c => c.Replace("\r\n", "")).ToList();
                    //}                

                    //int RowCount = data.Rows.Count;
                    string val = string.Empty;
                    //create add the tags that belong to this project to the project objects tag object  store 
                    foreach(string line in file)
                    {
                        
                        if(line==null)
                        {
                            val = default(string);
                        }
                        else
                        {
                            val = line;
                        }
                        Tag thisTag = new Tag();
                        thisTag.TagName = val;
                        //thisTag.TagIndex = i;
                        _tags.Add(thisTag);
                    }
                    
                    TagManager manager = new TagManager(_tags);
                    


                    MultiThreadObservableCollection<SelectionItem<ProjectViewModel>> projects
                        = new MultiThreadObservableCollection<SelectionItem<ProjectViewModel>>();

                    foreach (Project proj in manager.ProjectList.Values)
                    {

                        ProjectViewModel pvm = new ProjectViewModel(proj,_dataSource) { start = start, end = end };
                        projects.Add(new SelectionItem<ProjectViewModel>(pvm));
                    }
                    
                    ProjectList = new SelectionList<SelectionItem<ProjectViewModel>>(projects);
                   // Console.WriteLine("finished loading source file");
                   
                    this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                        (
                         delegate()
                         {
                             ProjectList = new SelectionList<SelectionItem<ProjectViewModel>>(projects);
                         }));
                };

                worker.RunWorkerAsync();


            }
            catch (Exception e)
            {
                throw e;
            }


        }
            bool CanLoadInputFile()
        {
            if (_isDownloading)
                return false;
            else
                return true;  
        }
            void CheckFiles()
            {
                string folder = OutPutFileLocation;
                string[] files = Directory.GetFiles(folder);
                if (files.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("No Files Found in Folder");
                    return;
                }

                StreamWriter sw=new StreamWriter (folder +  @"\TagCheck.csv");
                List<string> tagsNotFoundInOutput=new List<string>();
                
                //check inventory
                foreach (string file in files)
                {
                    foreach (SelectionItem<ProjectViewModel> pvm in ProjectList.AllItems )
                    {
                        if (Path.GetFileName(file).StartsWith(pvm.SelectedItem.Project.name))
                        {
                            Project selectedProject=pvm.SelectedItem.Project;
                            StreamReader sr = new StreamReader(file);
                            List<string> outputFile = sr.ReadLine().Split(',').ToList();
                            List<string> sourceTags=new List<string>();
                            selectedProject.tags.ForEach (c=>sourceTags.Add(c.TagName) );
                             tagsNotFoundInOutput = sourceTags.Except(outputFile ).ToList ().Where(c=>c.Length>0 && ! c.Contains("DateTime")).ToList();
                            tagsNotFoundInOutput.ForEach(c => sw.WriteLine(c));
                            continue;
                        }
                    }
                }

                sw.Close();
                if(tagsNotFoundInOutput .Count==0)
                {
                    System.Windows.Forms.MessageBox.Show("No Missng Tags");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Missng Tags Found and Output to Log File in output folder");
                }
            }
            bool CanCheckFiles()
            {
                if (OutPutFileLocation!=null)
                    return true;
                else
                    return false;
            }
       

        #endregion
    }
}

