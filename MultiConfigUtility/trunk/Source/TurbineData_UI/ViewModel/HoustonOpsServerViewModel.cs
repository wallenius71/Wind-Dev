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
    public class HoustonOpsServerViewModel:BaseDataSourceView 
    {

        string _selectedFile;
        RelayCommand _loadFileCommand;
        //RelayCommand _outputFileCommand;
        SelectionList<SelectionItem<ProjectViewModel>> _projectList;
        List<Tag> _tags=new List<Tag> ();
        
         public HoustonOpsServerViewModel()
        {
            start = DateTime.Today;
            end = DateTime.Today;

        }
        
 #region properties

        //public override DateTime start
        // {
        //     get { return _start; }
        //     set
        //     {
        //         start = value;
        //         //update the project dates as well 
        //         foreach (SelectionItem<ProjectViewModel> pvm in ProjectList.AllItems)
        //         {
        //             pvm.SelectedItem.start = value;
        //         }
        //     }
        // }
        //public override DateTime end
        // {
        //     get { return _end; }
        //     set
        //     {
        //         _end = value;
        //         foreach (SelectionItem<ProjectViewModel> pvm in ProjectList.AllItems)
        //         {
        //             pvm.SelectedItem.end = value;
        //         }
        //     }
        // }
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
                    _loadFileCommand = new RelayCommand(param => this.LoadInputFile(), param => this.CanLoadInputFile());

                return _loadFileCommand;
            }
        }
        
#endregion
        
        #region methods
                
            void LoadInputFile()
        {

            string filename = Utils.GetFile();
            if (filename.Length < 1)
                return;

            try
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    //get the tags from the repository 
                    DataRepository repository = new DataRepository(filename, DataSourceType.XL2007);
                    DataTable data = repository.GetAllData();
                    int RowCount = data.Rows.Count;
                    string val = string.Empty;
                    
                    for (int i = 0; i < RowCount; i++)
                    {
                        if (data.Rows[i][0].GetType().ToString() == "System.DBNull")
                        {
                            val = default(string);
                        }
                        else
                        {
                            val = data.Rows[i][0].ToString();
                        }
                        Tag thisTag = new Tag();
                        thisTag.TagName = val;
                        thisTag.TagIndex = i;
                        _tags.Add(thisTag);
                    }

                    TagManager manager = new TagManager(_tags);


                    MultiThreadObservableCollection<SelectionItem<ProjectViewModel>> projects
                        = new MultiThreadObservableCollection<SelectionItem<ProjectViewModel>>();

                    foreach (Project proj in manager.ProjectList.Values)
                    {

                        ProjectViewModel pvm = new ProjectViewModel(proj) { start = start, end = end };
                        projects.Add(new SelectionItem<ProjectViewModel>(pvm));
                    }

                    ProjectList = new SelectionList<SelectionItem<ProjectViewModel>>(projects);


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
       

        #endregion
    }
}
