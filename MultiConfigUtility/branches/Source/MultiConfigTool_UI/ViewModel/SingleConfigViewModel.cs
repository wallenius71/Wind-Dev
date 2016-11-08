using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WindART;
using System.Data;
using System.ComponentModel;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Reflection;
using System.IO;

namespace MultiConfigTool_UI
{
    public class SingleConfigViewModel:BaseDataSourceView   ,IDropTarget,INotifyCollectionChanged 
    {
        #region fields
        
        RelayCommand _processCommand;
        RelayCommand _OutputFileCommand;
        RelayCommand _bulkEditColTypeCommand;
        RelayCommand _closeCommand;
        RelayCommand _LoadConfigCommand;
        RelayCommand _saveConfigCommand;
        RelayCommand _loadPersistedConfigCommand;
        RelayCommand _runShearCommand;

        ObservableCollection<ISessionColumn> _liveCollection;
        SessionColumnCollection _sessionColumnCollection;
        private DateTime _dataSetStartDate;
        private  DateTime _dataSetEndDate;
        private bool _hasColumnCollection;
        bool _isProcessing;
        private string _OutputSummaryFileLocation;
        private string _NameOfThisSite;
        private AllShearViewModel _shearGridCollection;
        private bool _fileIsLoading;
        private string _fileProgress;
        private ObservableCollection<SessionColumn> _possibleComposites = new ObservableCollection<SessionColumn>();
        private AxisType _xShearAxis;
        private AxisType _yShearAxis;
        private double _xBinWidth;
        private double _yBinWidth;
        private AlphaFilterMethod _alphaFilter;
        
        

        #endregion

        public delegate void AlphaUpdatedEventHandler(AbstractAlpha alpha);
        public event AlphaUpdatedEventHandler AlphaCollectionUpdated;

        public delegate void AlphaRemovedEventHandler(AbstractAlpha alpha);
        public event AlphaRemovedEventHandler AlphaRemoved;

        public delegate void CompositesUpdatedEventHandler(SingleConfigViewModel config);
        public event CompositesUpdatedEventHandler CompositesUpdated;

        #region properties

        public SessionColumnCollection ColumnCollection
        {
            get
            {
                return _sessionColumnCollection;
            }
            set
            {
                _sessionColumnCollection = value;
                OnPropertyChanged("ColumnCollection");
                if (_sessionColumnCollection!=null && _sessionColumnCollection.Columns != null)
                    HasColumnCollection = true;
                else
                    HasColumnCollection = false;

            }
        }
        public ObservableCollection<ISessionColumn> LiveCollection
        {
            get
            {

                return _liveCollection;
            }
            set
            {
                _liveCollection = value;

                _sessionColumnCollection.Columns.Clear();
                foreach (ISessionColumn sc in _liveCollection)
                    _sessionColumnCollection.Columns.Add(sc);

                OnPropertyChanged("LiveCollection");
                OnPropertyChanged("ColumnCollection");

            }

        }
        public bool WDCompositeExists { get; set; }
        public bool WSCompositeExists { get; set; }
        public DateTime DataSetStartDate
        {
            get { return _dataSetStartDate; }
            set { 
                _dataSetStartDate = value;
                OnPropertyChanged("DataSetStartDate");
            }
        }
        public DateTime DataSetEndDate
        {
            get { return _dataSetEndDate ;}
            set
            {
                _dataSetEndDate = value;
                OnPropertyChanged("DataSetEndDate");
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
        public bool HasColumnCollection
        {
            get { return _hasColumnCollection; }
            private set 
            {
                _hasColumnCollection = value;
                OnPropertyChanged("HasColumnCollection");
            }
        }
        public string OutputFileLocation
        {
            get
            { return _outputFileLocation; }
            set
            {
                if (value != null)
                {
                    _outputFileLocation = value;
                    OnPropertyChanged("OutputFileLocation");
                }
            }
        }
        public string OutputSummaryFileLocation
        {
            get
            {
                return _OutputSummaryFileLocation;
            }
            set
            {
                _OutputSummaryFileLocation = value;
                OnPropertyChanged("OutputSummaryFileLocation");
            }
        }
        public string NameOfThisSite
        {
            get
            {
                return _NameOfThisSite;
            }
            set
            {
                if (value != null)
                {
                    _NameOfThisSite = value;
                    OnPropertyChanged("NameOfThisSite");

                }
            }
        }
        public AllShearViewModel  ShearGridCollection
        {
            get
            {
                return _shearGridCollection;

            }
            set
            {
                if (_shearGridCollection == null && value !=null) _shearGridCollection = new AllShearViewModel();
                _shearGridCollection = value;
                _shearGridCollection.ShearCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(ShearCollection_CollectionChanged);
                OnPropertyChanged("ShearGridCollection");
            }
        }
        public string RunShearText
        {
            get { return "Run Shear"; }
        }

        void ShearCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;
            foreach (object item in e.OldItems)
            {
                ShearGridViewModel sgvm = (ShearGridViewModel)item;
                AlphaRemoved(sgvm.GridCollection);
            }
        }
        public SelectionList<SelectionItem <StationSummarySheetType>> SummarySheets { get; private set; }
        public string PersistFilename { get; set; }
        public override ObservableCollection<DataTable> DownloadedData
        {
            get
            {
                return _downloadedData;
            }
            set
            {
                _downloadedData = value;
            }
        }
        public string Filename { get; set; }
        
        public AxisType XShearAxis
        {
            get { return _xShearAxis; }
            set
            {
                _xShearAxis = value;
                OnPropertyChanged("XShearAxis");
            }
        }
        public AxisType YShearAxis
        {
            get { return _yShearAxis; }
            set
            {
                _yShearAxis = value;
                OnPropertyChanged("YShearAxis");
            }
        }
        public List<AxisType> AxisTypes
        {
            get {
                List<AxisType> vals = new List<AxisType>();
                foreach(AxisType a in Enum.GetValues(typeof(AxisType)))
                {
                    vals.Add(a);
                }

                var result = vals.Where(c => c != AxisType.MonthYear);
                return result.ToList();
             }
        }
        public double  XBinWidth
        {
            get
            {
                return _xBinWidth;
            }
            set
            {
                _xBinWidth = value;
                OnPropertyChanged("XBinWidth");
            }
        }
        public double YBinWidth
        {
            get
            {
                return _yBinWidth;
            }
            set
            {
                _yBinWidth = value;
                OnPropertyChanged("YBinWidth");
            }
        }
        public AlphaFilterMethod AlphaFilter
        {
            get
            {
                return _alphaFilter;
            }
            set
            {
                _alphaFilter = value;
                OnPropertyChanged("AlphaFilter");
            }
        }
        public List<AlphaFilterMethod> AlphaFilterTypes
        {
            get
            {
                List<AlphaFilterMethod> vals = new List<AlphaFilterMethod>();
                foreach (AlphaFilterMethod a in Enum.GetValues(typeof(AlphaFilterMethod )))
                {
                    vals.Add(a);
                }

                var result = vals;
                return result.ToList();
            }
        }
        #endregion
        
        //constructor
        public SingleConfigViewModel (SessionColumnCollection columnCollection, DataTable data)
        {
            ColumnCollection = columnCollection;
            DownloadedData.Add(data);
            DataSetStartDate = ColumnCollection.DataSetStart;
            DataSetEndDate = ColumnCollection.DataSetEnd;
        }
        public SingleConfigViewModel(SessionColumnCollection columnCollection)
        {
            
            ColumnCollection = columnCollection;
            DataSetStartDate = ColumnCollection.DataSetStart;
            DataSetEndDate = ColumnCollection.DataSetEnd;
            
        }
        public SingleConfigViewModel()
        {
           
        }
        
        #region commands
          
        public ICommand BulkEditColTypeCommand
        {
            get
            {
                if (_bulkEditColTypeCommand == null)
                    _bulkEditColTypeCommand = new RelayCommand(param => this.BulkEditColType(param), param => this.CanBulkEditColType(param));

                return _bulkEditColTypeCommand;
            }
        }
        public ICommand ProcessCommand
        {
            get
            {
                if (_processCommand == null)
                    _processCommand = new RelayCommand(param => this.RunProcessing(),param=>this.CanRunProcessing ());

                return _processCommand;
            }
        }
        public ICommand RunShearCommand
        {
            get
            {
                if (_runShearCommand == null)
                    _runShearCommand = new RelayCommand(param => this.RunShear(), param => this.CanRunShear());

                return _runShearCommand;
            }
        }
        public ICommand OutputFileCommand
        {
            get
            {
                if (_OutputFileCommand == null)
                    _OutputFileCommand = new RelayCommand(param => this.OutputFile(),param=>this.CanOutputFile ());

                return _OutputFileCommand;
            }
        }
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());

                return _closeCommand;
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
        public ICommand LoadConfigCommand
        {
            get
            {
                if (_LoadConfigCommand == null)
                    _LoadConfigCommand = new RelayCommand(param => this.LoadConfig(param));

                return _LoadConfigCommand;
            }

        }


        #endregion 

        #region methods

        void SaveConfig() 
        {
            
            
            PersistSettings.Save(LiveCollection);
        }
        bool CanSaveConfig() 
        {
            if (ColumnCollection == null) return false;
            if(ColumnCollection.Data ==null)return false;
            else
            return true;
        }
        void LoadPersistedConfig()
        {
            try
            {
                if (String.IsNullOrEmpty(PersistFilename))
                {
                    //Console.WriteLine("trying to load the persisted file");
                    PersistFilename = Utils.GetFile();
                }
                if (PersistFilename == null)
                    return;

                object loadedobject = PersistSettings.Load(PersistFilename);
                if (loadedobject != null)
                {
                    LiveCollection = (ObservableCollection<ISessionColumn>)loadedobject;
                }
                else
                {
                    PersistFilename = string.Empty;
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There was a problem loadeing the configuration: " + e.Message);
            }
            finally
            {
               
            }

        }
        bool CanLoadPersistedConfig()
        {
            if (_isProcessing || ! string.IsNullOrEmpty (PersistFilename) ) return false;
            else
            return true;
        }
        void BulkEditColType(Object sender)
        {
            DependencyObject o = (DependencyObject)sender;
            ComboBox cbo = VisualTreeExtensions.GetVisualDescendent<ComboBox>(o);
            List<TreeViewItem> tvi = VisualTreeExtensions.GetVisualDescendents<TreeViewItem>(o).ToList ();
            
            foreach (TreeViewItem item in tvi)
            {
                CheckBox chk = VisualTreeExtensions.GetVisualDescendent<CheckBox>(item);
                if (chk.IsChecked == true)
                { 
                    SessionColumn col = (SessionColumn)item.DataContext;
                    col.ColumnType = (SessionColumnType)cbo.SelectedValue ;
                    chk.IsChecked = false;
                }
            }
            
        }
        bool CanBulkEditColType(Object sender)
        {
            if (sender == null) return false;
            DependencyObject o = (DependencyObject)sender;
            List<TreeViewItem> tvi = VisualTreeExtensions.GetVisualDescendents<TreeViewItem>(o).ToList();

            foreach (TreeViewItem item in tvi)
            {
                if (VisualTreeExtensions.GetVisualDescendent<CheckBox>(item).IsChecked == true)
                {
                    return true;
                }
            }
            return false;

        }
        void RunProcessing()
        {
            BackgroundWorker worker = new BackgroundWorker();
            
                    WindDirectionComposite  wdcomposite = new WindDirectionComposite( ColumnCollection, DownloadedData[0]);
                    WindSpeedComposite wscomposite = new WindSpeedComposite(_sessionColumnCollection, DownloadedData[0]);
                    wscomposite.CompletedWindSpeedCompositeValues += new WindSpeedComposite.ProgressEventHandler(UpdateCompCollection);
            try
            {
                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    
                    FileProgressText = "";
                    
                };
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    try
                    {
                        IsProcessing = true;

                        if (!WDCompositeExists)
                        {
                            wdcomposite.NewCompositeColumnAdded+=new WindDirectionComposite.ProgressEventHandler(repository_UpdateProgress);
                            if (wdcomposite.CalculateComposites())
                            {
                                WDCompositeExists = true;
                                ColumnCollection.Columns = new MultiThreadObservableCollection<ISessionColumn>(_sessionColumnCollection.Columns.ToList());
                            }
                            else
                            {
                                WDCompositeExists = false;
                                System.Windows.MessageBox.Show("There was an error calculating WD Composites.");
                            }
                        }


                        wdcomposite.CompletedWindDirectionCompositeValues += new WindDirectionComposite.ProgressEventHandler(repository_UpdateProgress );
                                            
                        wscomposite.CompletedWindSpeedCompositeValues += new WindSpeedComposite.ProgressEventHandler(repository_UpdateProgress);
                           

                       

                        if (!WSCompositeExists)
                        {
                            wscomposite.DeterminingWindSpeedCompositeValues += new WindSpeedComposite.ProgressEventHandler(repository_UpdateProgress);
                            if (wscomposite.CalculateComposites())
                            {
                                WSCompositeExists = true;
                                ColumnCollection.Columns = new MultiThreadObservableCollection<ISessionColumn>(_sessionColumnCollection.Columns.ToList());
                            }
                            else
                            {
                                WSCompositeExists = false;
                                System.Windows.MessageBox.Show("There was an error calculating WS Composites");
                            }
                        }

                        
                       
                        IsProcessing = false;
                    }
                    finally
                    {
                        IsProcessing = false;
                    }
                    
                };
                worker.RunWorkerAsync();
            }
            catch 
            {
                throw;
            }
            

        }
        bool CanRunProcessing()
        {

            if (this.WSCompositeExists || IsProcessing )
                return false;
            else
                return true;

        }
        void LoadConfig(Object sender)
        {
            DependencyObject item = VisualTreeHelper.GetParent((DependencyObject)sender);
            
            while (item.GetType() != typeof(TreeViewItem ))
            {
                if (item == null) return;
                item = VisualTreeHelper.GetParent(item);
            }
            
            ItemsControl itemParent = ItemsControl.ItemsControlFromItemContainer(item);
            object SourceItem ;
            if (itemParent == null)
            {
                TreeViewItem tv = (TreeViewItem)item;
                SourceItem = tv.DataContext;
            }
            else
            {
                SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
            }
            if (SourceItem is SessionColumn)
            {
                //if the user has selected wsavg as the column type then create a windspeed sensor config
                //other wise create a regular sensor config 

                SessionColumn col = (SessionColumn)SourceItem;
                if (col.ColumnType == SessionColumnType.WSAvg)
                {
                    WindSpeedConfig config = new WindSpeedConfig();
                    config.StartDate = DataSetStartDate;
                    config.EndDate = DataSetEndDate;
                    _sessionColumnCollection [col.ColName].ColumnType = col.ColumnType;
                    _sessionColumnCollection [col.ColName].addConfig(config);

                }
                else
                {
                    SensorConfig config = new SensorConfig();
                    config.StartDate = DataSetStartDate;
                    config.EndDate = DataSetEndDate;
                    _sessionColumnCollection [col.ColName].ColumnType = col.ColumnType ;
                    _sessionColumnCollection [col.ColName].addConfig(config);
                }
                
            }
            else
            {
                throw new ApplicationException("Type passed in must be a SessionColumn. ViewModel1.LoadConfig");
            }
            
        }
        void OutputFile()
       {
       //     List<int> printcols=new List<int>();

       //     BackgroundWorker worker = new BackgroundWorker();
       //     worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
       //     {
       //         MessageBox.Show("File output complete");
       //     };
       //     worker.DoWork += delegate(object s, DoWorkEventArgs args)
       //         {
       //             printcols=GetPrintCols (ColumnCollection.Columns.ToList ());

       //             IExportFile file = new ExportFile(_sessionColumnCollection,
       //                 data.AsDataView(), printcols);

       //             file.OutputFile(OutputFileLocation);

                    
       //             List<StationSummarySheetType> runSheets = new List<StationSummarySheetType>();
       //             foreach (SelectionItem <StationSummarySheetType > Item in SummarySheets.selectedItems)
       //             {
       //                 runSheets.Add(Item.SelectedItem);
       //             }
                    
       //             XbyYShearStationSummary summary = new XbyYShearStationSummary(_sessionColumnCollection ,
       //                 data.AsDataView(), 30, 10, 2, _ogrid,runSheets  );

       //             summary.CreateReport(OutputSummaryFileLocation );
       //         };
       //     worker.RunWorkerAsync ();

        }
        void repository_UpdateProgress(string msg)
        {
            FileProgressText = msg;
        }
        void UpdateCompCollection(string msg)
        {
            //fire the comp updated event
            if(CompositesUpdated !=null)
            CompositesUpdated(this );
        }
        void UpdatedAlphaCollection(AbstractAlpha alpha)
        {
            //call the event from the view model 
            if(AlphaCollectionUpdated !=null)
            AlphaCollectionUpdated (alpha);
        }
        void RunShear()
        {
            BackgroundWorker worker = new BackgroundWorker();
            
            try
            {
                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    
                };
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {

                    try
                    {
                        IsProcessing = true;

                        int upperCompidx = ColumnCollection.UpperWSComp(DataSetStartDate);
                        int lowerCompidx = ColumnCollection.LowerWSComp(DataSetStartDate);

                        ISessionColumn upperws = ColumnCollection[upperCompidx];
                        ISessionColumn lowerws = ColumnCollection[lowerCompidx];


                        IAxis Xaxis = GetAxis(_xShearAxis, _xBinWidth);
                        IAxis Yaxis = GetAxis(_yShearAxis, _yBinWidth);

                        AlphaFactory afactory = new AlphaFactory();
                        Alpha alpha = (Alpha)afactory.CreateAlpha(DownloadedData[0],_alphaFilter, upperws, lowerws, Xaxis, Yaxis);

                        alpha.AlphaUpdated += new Alpha.AlphaUpdatedEventHandler(UpdatedAlphaCollection);
                        alpha.SourceDataSet = this.DisplayName;

                        ColumnCollection.AlphaCollection.Add(alpha);
                        alpha.CalculateAlpha();
                         
                        string xBin=string.Empty;
                        string yBin=string.Empty;

                        if(Xaxis.AxisType==AxisType .WD)
                        {
                            var wdaxis=(WindDirectionAxis)Xaxis;
                            xBin=" " + wdaxis.BinWidth.ToString () + " deg" ;
                        }
                        if (Xaxis.AxisType == AxisType.WS)
                        {
                            var wsaxis = (WindSpeedAxis)Xaxis;
                            xBin = " " + wsaxis.BinWidth.ToString() + " m/s";
                        }
                        if(Yaxis.AxisType==AxisType .WD)
                        {
                            var wdaxis=(WindDirectionAxis)Yaxis;
                            yBin= " " +wdaxis.BinWidth.ToString () + " deg" ;
                        }
                        if (Yaxis.AxisType == AxisType.WS)
                        {
                            var wsaxis = (WindSpeedAxis)Yaxis;
                            yBin = " " + wsaxis.BinWidth.ToString() + " m/s";
                        }

                        AllShearViewModel asvm = new AllShearViewModel();
                        string filter;
                        if(_alphaFilter ==AlphaFilterMethod.Coincident ) filter="CDNT";
                        else filter="NCDNT";

                        string gridname = alpha.SourceDataSet + "_" + filter + "_" + Xaxis.AxisType + xBin + " by " + Yaxis.AxisType + yBin;
                        ShearGridViewModel sgvm = new ShearGridViewModel(alpha,gridname  );
                        
                        if (ShearGridCollection == null) ShearGridCollection = new AllShearViewModel();
                       
                        if (ShearGridCollection.ShearCollection.Count == 0)
                            asvm.ShearCollection.Add(sgvm);



                        UpdatedAlphaCollection(alpha);
                        alpha.AlphaUpdated -= UpdatedAlphaCollection;
                        this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                        (
                         delegate()
                         {
                             if (ShearGridCollection.ShearCollection.Count == 0)
                                 ShearGridCollection = asvm;
                             else
                                 ShearGridCollection.ShearCollection.Add(sgvm);
                         }));
                        //add sheargridviewmodels to allsheargridviewmodel


                        IsProcessing = false;
                    }
                    catch (ApplicationException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        IsProcessing = false;
                    }

                };
                worker.RunWorkerAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
        bool CanRunShear()
        {
            if (WSCompositeExists & XShearAxis !=AxisType.None  & YShearAxis !=AxisType.None)
            {
                return true; 
            }
            return false;
        }

        IAxis GetAxis(AxisType type)
        {
            AxisFactory factory = new AxisFactory();

            switch (type)
            {
                case AxisType.Month:
                    {
                        IAxis axis = factory.CreateAxis(AxisType.Month);
                        axis.SessionColIndex = ColumnCollection.DateIndex;
                        return axis;
                       
                    }
                case AxisType.Hour:
                    {
                        IAxis axis = factory.CreateAxis(AxisType.Hour);
                        axis.SessionColIndex = ColumnCollection.DateIndex;
                        return axis;
                        
                    }
                case AxisType.WD:
                    {
                        IAxis axis = factory.CreateAxis(30);
                        axis.SessionColIndex = ColumnCollection.WDComp(ColumnCollection.DataSetStart );
                        return axis;
                       
                    }
                case AxisType.WS:
                    {
                        int i=ColumnCollection.UpperWSComp(ColumnCollection.DataSetStart );
                        IAxis axis = factory.CreateAxis(2,ColumnCollection.Data.AsDataView (), i);
                        axis.SessionColIndex = i;
                        return axis;
                        
                    }
                default:
                    return null;

            }
        }
        IAxis GetAxis(AxisType type,double binwidth)
        {
            AxisFactory factory = new AxisFactory();

            switch (type)
            {
                case AxisType.Month:
                    {
                        IAxis axis = factory.CreateAxis(AxisType.Month);
                        axis.SessionColIndex = ColumnCollection.DateIndex;
                        return axis;

                    }
                case AxisType.Hour:
                    {
                        IAxis axis = factory.CreateAxis(AxisType.Hour);
                        axis.SessionColIndex = ColumnCollection.DateIndex;
                        return axis;

                    }
                case AxisType.WD:
                    {
                        IAxis axis = factory.CreateAxis(binwidth);
                        axis.SessionColIndex = ColumnCollection.WDComp(ColumnCollection.DataSetStart);
                        return axis;

                    }
                case AxisType.WS:
                    {
                        int i = ColumnCollection.UpperWSComp(ColumnCollection.DataSetStart);
                        IAxis axis = factory.CreateAxis(binwidth, ColumnCollection.Data.AsDataView(), i);
                        axis.SessionColIndex = i;
                        return axis;

                    }
                default:
                    return null;

            }
        }
       
        public virtual  List<int> GetPrintCols(IList<ISessionColumn> cols)
        {//return the indexes of the columns to be output
            //a column can only have 1 child right now 
            
            List<int> result=new List<int>();
            
            foreach (SessionColumn sc in cols)
            {
                result.Add(sc.ColIndex);
                //get child columns
                foreach (SessionColumn innercol in sc.ChildColumns)
                {
                    result.Add(innercol.ColIndex );
                }

            }

            if (result.Count > 0)
                return result;
            else
                return null;
            
            
        }

        
        #endregion

        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #region IDropTarget Members

        public void DragOver(DropInfo dropInfo)
        {
            try
            {
                ISessionColumn source = (ISessionColumn)dropInfo.DragInfo.SourceItem;
                if (dropInfo.InTarget & source.ColName != "DateTime")
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

                }
                else
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

                }
                dropInfo.Effects = DragDropEffects.Move;
            }
            catch (NullReferenceException)
            {

            }


        }
        public void Drop(DropInfo dropInfo)
        {
                ISessionColumn source = (ISessionColumn)dropInfo.DragInfo.SourceItem;
                ISessionColumn target = (ISessionColumn)dropInfo.TargetItem;
                IList targetCollection = (IList)dropInfo.TargetCollection;
                IList sourceCollection=(IList)dropInfo.DragInfo.SourceCollection ;
                int index=dropInfo.InsertIndex;

                if (source.ColName  == "DateTime")
                {
                    sourceCollection.RemoveAt(sourceCollection.IndexOf(source));
                    targetCollection.Insert(index++, source);
                    
                }
                else
                {
                    if (!IsChild(dropInfo.VisualTargetItem) && (dropInfo.TargetItem !=dropInfo.DragInfo.SourceItem))
                    {
                        if (dropInfo.InTarget)
                        {
                            sourceCollection.RemoveAt(sourceCollection.IndexOf(source));
                            target.ChildColumns.Add(source);
                            
                        }
                        else
                        {
                            sourceCollection.RemoveAt(sourceCollection.IndexOf(source));
                            targetCollection.Insert(index++, source);

                        }

                        
                    }

                }
        }
        protected  bool IsChild(UIElement targetItem)
        {
            //if the itemscontrol of this item is a treeview then it is a root element 
            //if not and it is not null then it is assumed to be a child node
            if (targetItem != null)
            {
                ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(targetItem);
                return parent != null & parent.GetType() != typeof(TreeView);
            }
            return true;
            
        }
        protected  bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) =>
            {
                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            };

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            if (enumerableTypes.Count() > 0)
            {
                Type dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList;
            }
        }
        protected static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            else
            {
                return enumerable as IList;
            }
        }
        protected static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            else
            {
                return Enumerable.Repeat(data, 1);
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedAction action, ObservableCollection <ISessionColumn> columns)
        {

            if (CollectionChanged != null)
            {
                if(this.CollectionChanged !=null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs (action,columns));

            }

        }

        #endregion

        
    }

        public class CheckableSessionColumn :ViewModelBase 
    {
        //extends  session column to include a checkbox 
        //declared protected class because only intended for use in view model

        private bool? _isChecked;
        public SessionColumn SessCol{get;private set;}

        public CheckableSessionColumn (SessionColumn col)
        {
            SessCol = col;
        }
        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
    }


