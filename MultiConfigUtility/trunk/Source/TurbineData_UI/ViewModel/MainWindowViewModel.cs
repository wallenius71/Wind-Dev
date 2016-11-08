using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TurbineDataUtility.Model;
using System.Data;
using WindART.DAL;
using System.ComponentModel;
using WindART;
using System.Windows.Threading;
using System.IO;

namespace Analysis_UI
{
    public class MainWindowViewModel:ViewModelBase
    {
        #region fields
        ReadOnlyCollection<CommandViewModel> _commands;

       
        RelayCommand _OutputFileCommand;
        RelayCommand _clearCommand;
        RelayCommand _setOutputFileLocationCommand;
        RelayCommand _loadFileCommand;
        
        
        DataSourceType _datasource;
        ViewModelBase _selectDataSource;
        ViewModelBase _piDataView;
        ViewModelBase _workspace;
        private bool _fileIsLoading;
        AllConfigsViewModel _allConfigsVM;
        bool _isProcessing;
        BackgroundWorker worker;


        

        #endregion 

        #region properties

            public string Filename { get; set; }
            public bool FileIsLoading
        {
            get
            {
                return _fileIsLoading;
            }
            private set
            {
                _fileIsLoading = value;
                OnPropertyChanged("FileIsLoading");
            }

        }
            public DataSourceType Datasource
            {
                get
                {
                    return _datasource;
                }
                set
                {
                    _datasource = value;
                    OnPropertyChanged("Datasource");
                }
            }
            public ViewModelBase Workspace
            {
                get { return _workspace; }
                set
                {
                    _workspace = value;
                    OnPropertyChanged("Workspace");
                }
            }
            public AllConfigsViewModel AllConfigsVM
            {
                get { return _allConfigsVM; }
                set
                {
                    _allConfigsVM = value;
                    OnPropertyChanged("AllConfigsVM");
                }
            }
            

        #endregion 

#region constructor

            public MainWindowViewModel()
            {   
                
                AllConfigsVM = new AllConfigsViewModel( );
                AllConfigsVM.Configs.Add (new SingleConfigViewModel());

                Workspace = AllConfigsVM;
            }

#endregion

            #region commands
        public ICommand ClearCommand
            {
                get
                {
                    if (_clearCommand == null)
                        _clearCommand = new RelayCommand(param => this.Clear(),param=>this.CanClear());

                    return _clearCommand;
                }
            }
        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                    _loadFileCommand = new RelayCommand(param => this.LoadFile(), param => this.CanLoadFile());

                return _loadFileCommand;
            }
        }
        public ReadOnlyCollection<CommandViewModel> Commands
            {
                get
                {
                    if (_commands == null)
                    {
                        List<CommandViewModel> cmds = this.CreateCommands();
                        _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                    }
                    return _commands;
                }
            }
        List<CommandViewModel> CreateCommands()
            {
                return new List<CommandViewModel>
                {

                    
                    new CommandViewModel("Select Tags", 
                        new RelayCommand(param=>this.ShowSelectDataSource())),
                    new CommandViewModel("Data",
                        new RelayCommand(param=>this.ShowPiDataView(),param=>this.CanShowPiDataView())),
                    new CommandViewModel("Show Data Inventory", 
                        new RelayCommand(param=>this.ShowDataInventory(),param=>this.CanShowDataInventory())),
                     new CommandViewModel("Show Time Series", 
                        new RelayCommand(param=>this.ShowTimeSeries(),param=>this.CanShowTimeSeries()))
                };
            }
         
        
        


        #endregion 


        #region methods 
        void Clear()
        {
            SelectDataSourceViewModel sdsv = (SelectDataSourceViewModel)_selectDataSource;
            sdsv.DataSourceViewModel.DownloadedData.Clear();
            sdsv.DataSourceViewModel.TagManagers.Clear();
        }
        bool CanClear()
        {
            if (_selectDataSource ==null)
                return false;
            SelectDataSourceViewModel sdsv = (SelectDataSourceViewModel)_selectDataSource;
            if (sdsv.DataSourceViewModel==null)
                return false;
            if (sdsv.DataSourceViewModel.DownloadedData == null)
                return false;

            return true;
        }
        void ShowSelectDataSource()
        {
            
            if(_selectDataSource ==null)
                _selectDataSource =new SelectDataSourceViewModel();

            Workspace = _selectDataSource;
        }
        void ShowPiDataView()
        {
            if(_selectDataSource !=null)
            {   
                
                SelectDataSourceViewModel sdsv=(SelectDataSourceViewModel )_selectDataSource ;
                ObservableCollection<DataTable> data=sdsv.DataSourceViewModel.DownloadedData;
                
                Workspace = new ViewPiDataViewModel(data);
            }
        }
        bool CanShowPiDataView()
        {
            return true;
        }
        void ShowDataInventory()
        {
            SelectDataSourceViewModel sdsv = (SelectDataSourceViewModel)_selectDataSource;
            
            ObservableCollection<ViewModelBase> vmCol = new ObservableCollection<ViewModelBase>();
            foreach (TagManager tm in sdsv.DataSourceViewModel.TagManagers)
            {
                //add a view model to a collection 
                DataInventoryViewModel divm = new DataInventoryViewModel(tm.Tags);
                divm.DisplayName = tm.SetName;
                vmCol.Add(divm);
            }
            Workspace = new MultipleDataSetViewModel(vmCol);
            
        }
        bool CanShowDataInventory()
        {
            return true; 
        }
        void ShowTimeSeries()
        {
            SelectDataSourceViewModel sdsv = (SelectDataSourceViewModel)_selectDataSource;
            ObservableCollection<ViewModelBase> vmCol = new ObservableCollection<ViewModelBase>();
           
            

            
            if (sdsv.DataSourceViewModel  is HoustonOpsServerViewModel)
            {
                HoustonOpsServerViewModel hsvm = (HoustonOpsServerViewModel)sdsv.DataSourceViewModel ;
                foreach (SelectionItem <ProjectViewModel > pvm in hsvm.ProjectList.AllItems )
                {
                    foreach (TagManager tm in pvm.SelectedItem.TagManagers)
                    {
                        //add a view model to a collection 
                        TimeSeriesViewModel divm = new TimeSeriesViewModel(tm.Tags);
                       vmCol.Add(divm);
                    }
                }
            }
            else
            {
                foreach (TagManager tm in sdsv.DataSourceViewModel.TagManagers)
                {
                    //add a view model to a collection 
                    TimeSeriesViewModel divm = new TimeSeriesViewModel(tm.Tags, new List<int>(){4,6,8});
                    divm.DisplayName = tm.SetName;
                    vmCol.Add(divm);
                }
            }
            
                Workspace = new MultipleDataSetViewModel(vmCol);
                
                
           
            
            
        }
        bool CanShowTimeSeries()
        {
            return true;
        }
        void repository_UpdateProgress(string msg)
        {
            //FileProgressText = msg;
        }
        void LoadFile()
        {

           
            Filename = WindART.Utils.GetFile();

            if (Filename.Length < 1)
                return;

            FileIsLoading = true;
            if (AllConfigsVM == null) AllConfigsVM = new AllConfigsViewModel();
            Workspace = AllConfigsVM;
            try
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                DataRepository repository
                      = new DataRepository(Filename, DataSourceType.CSV);
                

                repository.FileOpening += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoading += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoaded += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);

                
                worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {

                    DataTable data = repository.GetAllData();
                    SingleConfigViewModel thisConfig = new SingleConfigViewModel(new SessionColumnCollection(data),data);
                    thisConfig.DisplayName = Path.GetFileNameWithoutExtension ( Filename);
                    this.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action
                       (
                       delegate()
                       {
                           
                           AllConfigsVM.Configs.Add(thisConfig);
                           
                           
                       }));
                    
                
                     

                    //need a better way to convert this collection to multithreaded oibservable 
                    ObservableCollection<ISessionColumn> colClxn = new MultiThreadObservableCollection<ISessionColumn>();
                    foreach (SessionColumn sc in thisConfig.ColumnCollection.Columns) colClxn.Add(sc);

                    //add to an observable collection 
                    thisConfig.LiveCollection = colClxn;

                    
                   FileIsLoading = false;
                };

                worker.RunWorkerAsync();


            }
            catch (Exception e)
            {
                throw e;
            }


        }
        bool CanLoadFile()
        {
            if (_fileIsLoading || _isProcessing)
                return false;
            else
                return true;
        }
        
        


        #endregion 

    }
}
