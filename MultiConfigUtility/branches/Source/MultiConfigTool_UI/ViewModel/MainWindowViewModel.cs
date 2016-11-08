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
using System.Windows.Forms;

namespace MultiConfigTool_UI
{
    public class MainWindowViewModel:ViewModelBase
    {
        #region fields
       
        RelayCommand _loadFileCommand;
                
        DataSourceType _datasource;
        
        ViewModelBase _workspace;
        private bool _fileIsLoading;
        AllConfigsViewModel _allConfigsVM;
        //bool _isProcessing;
        BackgroundWorker worker;
        ViewModelBase _lowerWorkspace;

        

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
            public ViewModelBase LowerWorkSpace
            {
                get { return _lowerWorkspace; }
                set
                {
                    _lowerWorkspace = value;
                    OnPropertyChanged("LowerWorkspace");
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

               //MessageBox.Show(" ");
            }

#endregion

            #region commands
        
        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                    _loadFileCommand = new RelayCommand(param => this.LoadFile(), param => this.CanLoadFile());

                return _loadFileCommand;
            }
        }
       
       
        #endregion 


        #region methods 
       
       
        
        
        
        void repository_UpdateProgress(string msg)
        {
            //FileProgressText = msg;
        }
        void LoadFile()
        {

           
           Filename = WindART.Utils.GetFile();
           //Console.WriteLine("filename has been set inside view model " + Filename);
          
            if (string.IsNullOrEmpty (Filename))
                return; 
            
            if (Path.GetExtension(Filename) != ".csv")
           {
               MessageBox.Show("File must be in  .csv format");
               return;
           }
            //Console.WriteLine("Viewmodel thinks filename length is > 0");
            FileIsLoading = true;
            
            try
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                DataRepository repository
                      = new DataRepository(Filename, DataSourceType.CSV);
                Console.WriteLine(repository.ToString());

                repository.FileOpening += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoading += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoaded += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                worker.RunWorkerCompleted +=delegate(object sender,RunWorkerCompletedEventArgs args)
                {
                    //Console.WriteLine("work complete");
                };

                
                
                worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {

                    DataTable data = repository.GetAllData();
                    

                    this.Dispatcher.UnhandledException += delegate(object s, DispatcherUnhandledExceptionEventArgs a)
                    {
                        Console.WriteLine(a.Exception.Message);
                    };
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action
                       (
                            delegate()
                       {
                             
                           SingleConfigViewModel thisConfig = new SingleConfigViewModel(new SessionColumnCollection(data),data);
                            thisConfig.DisplayName = Path.GetFileNameWithoutExtension ( Filename);
                            Console.WriteLine(thisConfig.DisplayName);
                         
                           //  need a better way to convert this collection to multithreaded oibservable 
                            ObservableCollection<ISessionColumn> colClxn = new MultiThreadObservableCollection<ISessionColumn>();
                            foreach (SessionColumn sc in thisConfig.ColumnCollection.Columns) colClxn.Add(sc);

                            //add to an observable collection 
                            thisConfig.LiveCollection = colClxn;

                           if (Workspace == null)
                           {
                               AllConfigsVM = new AllConfigsViewModel(thisConfig);
                               Workspace = AllConfigsVM;
                               Console.WriteLine("allconfigs created");
                               
                           }
                           else
                           {
                               

                                                                  
                                   AllConfigsVM.Configs.Add(thisConfig);
                               
                              // Console.WriteLine("added new config at index " + AllConfigsVM.Configs.IndexOf(thisConfig));
                           }


                       }));
                   FileIsLoading = false;
                };

                worker.RunWorkerAsync();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        bool CanLoadFile()
        {
            if (_fileIsLoading )
                return false;
            else
                return true;
        }
        
        


        #endregion 

    }
}
