using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using Analysis_UI.Properties;
using System.ComponentModel;
using WindART.DAL;
using WindART;
using System.Windows.Threading;

namespace Analysis_UI
{
    public class FileViewModel :BaseDataSourceView 
    {
        #region fields
        RelayCommand _loadFileCommand;
        RelayCommand _saveConfigCommand;
        RelayCommand _loadPersistedConfigCommand;

        List<SessionColumnCollection> sessionCollections = new List<SessionColumnCollection>();
        
        ProcessingViewModel _mainWorkSpace;
        private bool _fileIsLoading;
        private string _fileProgress;
        private bool _isProcessing;

        
        #endregion

        #region properties
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
        public ProcessingViewModel MainWorkSpace
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

        public FileViewModel()
        {
            MainWorkSpace = new ProcessingViewModel();
        }

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

                    SessionColumnCollection collection = new SessionColumnCollection(data);
                    if (!sessionCollections.Contains(collection))
                    {
                        sessionCollections.Add(collection);
                    }
                    //need a better way to convert this collection to multithreaded oibservable 
                    ObservableCollection<ISessionColumn> colClxn = new MultiThreadObservableCollection<ISessionColumn>();
                    foreach (SessionColumn sc in collection.Columns)

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

        

    }
}
