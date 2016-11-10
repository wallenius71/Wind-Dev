using System;
using System.Collections.Generic;
using System.Collections.ObjectModel ;
using System.Windows.Input ;
using System.Data;
using WindART_UI.Properties; 
using System.ComponentModel ;
using WindART.DAL;
using WindART;
using System.Windows.Threading;

namespace WindART_UI.ViewModel
{
 public class FrameViewModel:ViewModelBase
 {
#region fields
            RelayCommand _loadFileCommand;
            RelayCommand _saveConfigCommand;
            RelayCommand _loadPersistedConfigCommand;

            List<ISessionColumnCollection> sessionCollections=new List<ISessionColumnCollection> ();
            ReadOnlyCollection<CommandViewModel> _leftRailCommands;
            ProcessingViewModel  _mainWorkSpace;
            private bool _fileIsLoading;
            private string _fileProgress;
            private bool _isProcessing;
            private BackgroundWorker worker;
#endregion 

#region properties

       public ProcessingViewModel  MainWorkSpace
        {
            get
            {
                return _mainWorkSpace;
            }

            set
            {
                _mainWorkSpace = value;
                OnPropertyChanged("MainWorkSpace");
            }
        }
       public ReadOnlyCollection<CommandViewModel> LeftRailCommands
        {
            get
            {
                if (_leftRailCommands == null)
                {
                    List<CommandViewModel> cmds = this.CreateLeftRailCommands();
                    _leftRailCommands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                
                return _leftRailCommands;
            }
        }
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
       public bool IsProcessing
       {
           get
           {
               return _isProcessing;
           }
           private set
           {
               _isProcessing = value;
               OnPropertyChanged("IsProcessing");
           }

       }
       public string FileProgressText
       {
           get
           {
               return _fileProgress;
           }
           private set
           {
               _fileProgress = value;
               OnPropertyChanged("FileProgressText");
           }
       }


 #endregion

#region commands

      public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand== null)
                    _loadFileCommand= new RelayCommand(param => this.LoadFile(),param=>this.CanLoadFile ());

                return _loadFileCommand;
            }
        }
      public ICommand SaveConfigCommand
      {
          get
          {
              if (_saveConfigCommand == null)
                  _saveConfigCommand = new RelayCommand(param => this.SaveConfig(), param => this.CanSaveConfig());

              return _saveConfigCommand;
          }
      }
      public ICommand LoadPersistedConfigCommand
      {
          get
          {
              if (_loadPersistedConfigCommand == null)
                  _loadPersistedConfigCommand = new RelayCommand(param => this.LoadPersistedConfig(), param => this.CanLoadPersistedConfig());

              return _loadPersistedConfigCommand;
          }
      }

#endregion

#region methods


       List<CommandViewModel> CreateLeftRailCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                   Resources.Show_OutPut_Command , new RelayCommand (param=>this.ShowOutputView())),

                new CommandViewModel (Resources.Show_Processing_Command , 
                    new RelayCommand(param=>this.ShowProcessingView()))
            };
        }

        void ShowProcessingView()
        {
            ProcessingViewModel processingview = new ProcessingViewModel();
            MainWorkSpace = processingview;
        }
        void ShowOutputView()
        {

        }
        void LoadFile()
        {

            string filename = Utils.GetFile();
            if (filename.Length < 1)
                return;

            FileIsLoading = true;

            try
            {
                   worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;

                DataRepository repository
                      = new DataRepository(filename, DataSourceType.CSV);


                repository.FileOpening += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoading += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);
                repository.FileLoaded += new DataRepository.FileProgressEventHandler(repository_UpdateProgress);


                worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    
                    DataTable data = repository.GetAllData();
                    
                    SessionColumnCollection collection= new SessionColumnCollection(data);
                    if (!sessionCollections.Contains(collection))
                    {
                        sessionCollections.Add(collection);
                    }
                    //need a better way to convert this collection to multithreaded oibservable 
                    ObservableCollection<ISessionColumn> colClxn = new MultiThreadObservableCollection<ISessionColumn>();
                    foreach  (SessionColumn sc in collection.Columns)
                        
                        colClxn.Add(sc);
                    collection.Columns = colClxn;
                    if (MainWorkSpace != null)
                    {
                        this.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action
                            (
                            delegate()
                            {
                                MainWorkSpace = new ProcessingViewModel(sessionCollections);
                            }));
                        
                    }
                                
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
        void repository_UpdateProgress(string msg)
        {
            FileProgressText = msg;
        }
        void SaveConfig()
        {
            string filename = Utils.GetFile();
            if (filename.Length < 1)
                return;
            //PersistSettings.Save(LiveCollection, filename);
        }
        bool CanSaveConfig()
        {
            return true;
        }
        void LoadPersistedConfig()
        {
            string filename = Utils.GetFile();
            if (filename == null)
                return;

            object loadedobject = PersistSettings.Load(filename);
            if (loadedobject != null)
            {
                //LiveCollection = (ObservableCollection<ISessionColumn>)loadedobject;
            }

        }
        bool CanLoadPersistedConfig()
        {
            //if (data == null || _isProcessing) return false;
            //else
                return true;
        }

#endregion

 }
}
