using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WindART;
using WindART.DAL;
using System.Data;
using System.ComponentModel;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Reflection;

namespace WindART_UI
{
    public class SingleConfigViewModel:ViewModelBase,IDropTarget,INotifyCollectionChanged 
    {
        #region fields
        
        RelayCommand _processCommand;
        RelayCommand _LoadConfigCommand;
        RelayCommand _OutputFileCommand;
        RelayCommand _setOutputFileLocationCommand;
        RelayCommand _bulkEditColTypeCommand;
        RelayCommand _closeCommand;


        DataTable data;
        ObservableCollection<ISessionColumn> _liveCollection;
        ISessionColumnCollection _sessionColumnCollection;
        private DateTime _dataSetStartDate;
        private  DateTime _dataSetEndDate;
        private bool _hasColumnCollection;
        private double _heightToShearTo;
        private string _outputFileLocation;
        private string _OutputSummaryFileLocation;
        private string _NameOfThisSite;
        private ObservableCollection<ShearGridViewModel> _shearGridCollection;
        
        
        
        bool _isProcessing;
        
        

        #endregion

        #region properties

        public ISessionColumnCollection ColumnCollection
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
        public SingleConfigViewModel(ISessionColumnCollection columnCollection)
        {
            
            ColumnCollection = columnCollection; 
            
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
        public bool HasColumnCollection
        {
            get { return _hasColumnCollection; }
            private set 
            {
                _hasColumnCollection = value;
                OnPropertyChanged("HasColumnCollection");
            }
        }
        public double HeightToShearTo
        {
            get
            {
                return _heightToShearTo;
            }
            set
            {
                _heightToShearTo = value;
                OnPropertyChanged("HeightToShearTo");
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
        public ObservableCollection<ShearGridViewModel> ShearGridCollection
        {
            get
            {
                return _shearGridCollection;
            }
            set
            {
                _shearGridCollection = value;
                OnPropertyChanged("ShearGridCollection");
            }
        }
        
        
        public SelectionList<SelectionItem <StationSummarySheetType>> SummarySheets { get; private set; }
                       
        #endregion

        
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
        public ICommand LoadConfigCommand
        {
            get
            {
                if (_LoadConfigCommand == null)
                    _LoadConfigCommand = new RelayCommand( param   => this.LoadConfig(param));
                
                return _LoadConfigCommand ;
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
        public ICommand SetOutputFileLocationCommand
        {
            get
            {
                if (_setOutputFileLocationCommand == null)
                    _setOutputFileLocationCommand = new RelayCommand( param   => this.SetOutputFileLocation());
                
                return _setOutputFileLocationCommand ;
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

        #endregion 

        #region methods

        void SaveConfig() 
        {
            string filename = Utils.GetFile();
            if (filename.Length < 1)
                return;
            PersistSettings.Save(ColumnCollection.Columns  , filename);
        }
        bool CanSaveConfig() 
        {
            if(ColumnCollection.Data ==null)return false;
            else
            return true;
        }
        void LoadPersistedConfig()
        {
            string filename=Utils.GetFile();
            if (filename == null)
                return;

            object loadedobject = PersistSettings.Load(filename);
            if (loadedobject != null)
            {
                ColumnCollection.Columns   = (ObservableCollection<ISessionColumn>)loadedobject;
            }

        }
        bool CanLoadPersistedConfig()
        {
            if (data == null||_isProcessing ) return false;
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
            
                    WindDirectionComposite  wdcomposite = new WindDirectionComposite(_sessionColumnCollection, data);
                    WindSpeedComposite wscomposite = new WindSpeedComposite(_sessionColumnCollection, data);
                    MonthbyHourShear Shear = new MonthbyHourShear(_sessionColumnCollection, data.AsDataView());
                    Shear.Xaxis = new MonthAxis();
                    Shear.Yaxis = new HourAxis();
            try
            {
                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Done Processing!");
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
                                ColumnCollection.Columns  = new MultiThreadObservableCollection<ISessionColumn>(_sessionColumnCollection.Columns.ToList ());
                            }
                            else
                                WDCompositeExists = false;
                        }
                        wdcomposite.CompletedWindDirectionCompositeValues += new WindDirectionComposite.ProgressEventHandler(repository_UpdateProgress );


                        if (!WSCompositeExists)
                        {
                            wscomposite.DeterminingWindSpeedCompositeValues += new WindSpeedComposite.ProgressEventHandler(repository_UpdateProgress);
                            if (wscomposite.CalculateComposites())
                            {
                                WSCompositeExists = true;
                                ColumnCollection.Columns  = new MultiThreadObservableCollection<ISessionColumn>(_sessionColumnCollection.Columns.ToList ());
                            }
                            else
                                WSCompositeExists = false;
                        }
                        wscomposite .CompletedWindSpeedCompositeValues +=new WindSpeedComposite.ProgressEventHandler(repository_UpdateProgress);



                        if (HeightToShearTo > 0)
                        {
                            Shear.AddingShearValues +=new MonthbyHourShear.ProgressEventHandler(repository_UpdateProgress);

                            ShearCalculationGridCollection _ogrid;
                            Shear.CalculateWindSpeed(HeightToShearTo, out _ogrid);
                            ShearGridCollection.Add(new ShearGridViewModel ( _ogrid,"generic grid"));

                            ColumnCollection.Columns = new MultiThreadObservableCollection<ISessionColumn>(_sessionColumnCollection.Columns.ToList ());




                        }
                        Shear.AddedShearValues +=new MonthbyHourShear.ProgressEventHandler(repository_UpdateProgress);
                        IsProcessing = false;
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
        bool CanRunProcessing()
        {
            
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
                SourceItem = tv.DataContext ;
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
                    _sessionColumnCollection [col.ColName].ColumnType = col.ColumnType ;
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
        void SetOutputFileLocation()
        {
            string workFileLocation = Utils.GetFolder();
            string outputDate = DateTime.Now.ToShortDateString();
            outputDate = outputDate.Replace("/", "");
            OutputFileLocation = workFileLocation + "\\" + NameOfThisSite + "_WindART_AllData_12by24" + outputDate + ".csv";
            OutputSummaryFileLocation =  workFileLocation + "\\" + NameOfThisSite + "_StnSummary_" + outputDate + ".xlsx";
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
        bool CanOutputFile()
        {
            //if (HasColumnCollection ==false|| OutputFileLocation == string.Empty || OutputSummaryFileLocation == string.Empty
            //    || FileIsLoading || IsProcessing)
            //    return false;
            //else
                return true;
        }
        void repository_UpdateProgress(string msg)
        {
            //FileProgressText = msg;
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
            ISessionColumn source = (ISessionColumn)dropInfo.DragInfo.SourceItem;
            if (dropInfo.InTarget & source.ColName !="DateTime" )
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                
            }
            else
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                
            }
            dropInfo.Effects = DragDropEffects.Move;

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
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(targetItem);
            return parent !=null & parent.GetType() !=typeof ( TreeView );
            
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


