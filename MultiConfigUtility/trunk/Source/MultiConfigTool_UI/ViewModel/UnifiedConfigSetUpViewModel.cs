using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel ;
using WindART;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using System.Threading;


namespace MultiConfigTool_UI
{
    public class UnifiedConfigSetUpViewModel:ViewModelBase 
    {

        RelayCommand _outPutSummaryCommand;
        RelayCommand _outPutUnifiedCommand;

        ObservableCollection<double> _possibleUnifiedHeights;
        AllConfigsViewModel _acvm;
        private double _hubHeight;
        private double _upperHeight;
        private double _lowerHeight;
        ObservableCollection<string> _configHeaderRow = new ObservableCollection<string>();
        ObservableCollection<UnifiedHeightViewModel> _hubHeightRow;
        ObservableCollection<UnifiedHeightViewModel> _upperHeightRow;
        ObservableCollection<UnifiedHeightViewModel> _lowerHeightRow;
        bool _dataIsProcessing;
        bool _summaryIsProcessing;
        private AxisType _xShearAxis;
        private AxisType _yShearAxis;
        private double _xBinWidth;
        private double _yBinWidth;
        private SessionColumnCollection ColumnCollection;
        private DataTable _unifiedData;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _unifiedDataHasBeenCreated;

        public ObservableCollection<string> ConfigHeaderRow
        {
            get
            {
                if (_configHeaderRow == null) _configHeaderRow = new ObservableCollection<string>();
                foreach (SingleConfigViewModel vm in _acvm.Configs)
                {   
                    if (!_configHeaderRow.Contains(vm.DisplayName))
                        _configHeaderRow.Add(vm.DisplayName);

                }
                return _configHeaderRow;
            }
            set 
            { 
                _configHeaderRow = value;
                OnPropertyChanged("ConfigHeaderRow");
            }
        }
        public ObservableCollection<UnifiedHeightViewModel> HubHeightRow
        {
            get
            {
                if (_hubHeightRow == null) _hubHeightRow = new ObservableCollection<UnifiedHeightViewModel>();
                return _hubHeightRow;
            }
            set
            {
                _hubHeightRow = value;
                OnPropertyChanged("HubHeightRow");
            }
        }
        public ObservableCollection<UnifiedHeightViewModel> UpperHeightRow
        {
            get { return _upperHeightRow; }

            set
            {
                _upperHeightRow = value;
                OnPropertyChanged("UpperHeightRow");
            }
        }
        public  ObservableCollection<UnifiedHeightViewModel> LowerHeightRow
        {
            get { return _lowerHeightRow; }
            set { _lowerHeightRow = value;
            OnPropertyChanged("LowerHeightRow");
            }
        }

        public double HubHeight
        {
            get
            {
                return _hubHeight;
            }

            set
            {
                _hubHeight = value;
                List<UnifiedHeightViewModel> uhvm = new List<UnifiedHeightViewModel>();
                foreach (SingleConfigViewModel scvm in _acvm.Configs)
                {
                    UnifiedHeightViewModel thisu = new UnifiedHeightViewModel(scvm,_acvm, _hubHeight);
                   uhvm.Add(thisu);
                }
                HubHeightRow = new ObservableCollection<UnifiedHeightViewModel>(uhvm);
                OnPropertyChanged("HubHeight");
                OnPropertyChanged("HubHeightRow");
                OnPropertyChanged("ConfigHeaderRow");
            }
        }

        void PossibleColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HubHeightRow");
        }
        public double UpperHeight
        {
            get
            {
                return _upperHeight;
            }

            set
            {
                _upperHeight = value;
                List<UnifiedHeightViewModel> uhvm = new List<UnifiedHeightViewModel>();
                foreach (SingleConfigViewModel scvm in _acvm.Configs)
                {
                    uhvm.Add(new UnifiedHeightViewModel(scvm,_acvm, _upperHeight));
                }
                UpperHeightRow = new ObservableCollection<UnifiedHeightViewModel>(uhvm);
                OnPropertyChanged("UpperHeight");
                OnPropertyChanged("UpperHeightRow");
            }
        }
        public double LowerHeight
        {
            get
            {
                return _lowerHeight;
            }

            set
            {
                _lowerHeight = value;
                List<UnifiedHeightViewModel> uhvm = new List<UnifiedHeightViewModel>();
                foreach (SingleConfigViewModel scvm in _acvm.Configs)
                {
                    uhvm.Add(new UnifiedHeightViewModel(scvm,_acvm, _lowerHeight));
                }
                LowerHeightRow = new ObservableCollection<UnifiedHeightViewModel>(uhvm);
                OnPropertyChanged("LowerHeight");
                OnPropertyChanged("LowerHeightRow");
            }
        }
        public bool DataIsProcessing
        {
            get { return _dataIsProcessing; }
            set
            {
                _dataIsProcessing = value;
                OnPropertyChanged("DataIsProcessing");
            }
        }
        public bool SummaryIsProcessing
        {
            get { return _summaryIsProcessing; }
            set
            {
                _summaryIsProcessing = value;
                OnPropertyChanged("SummaryIsProcessing");
            }
        }
        public string RunShearText
        {
            get { return "Output Station Summary"; }
        }
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
        public double XBinWidth
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
        public List<AxisType> AxisTypes
        {
            get
            {
                List<AxisType> vals = new List<AxisType>();
                foreach (AxisType a in Enum.GetValues(typeof(AxisType)))
                {
                    vals.Add(a);
                }

                var result = vals.Where(c => c != AxisType.MonthYear);
                return result.ToList();
            }
        }
        
        public UnifiedConfigSetUpViewModel(AllConfigsViewModel allconfigs)
        {

            _acvm = allconfigs;
            
        }

        public ICommand OutPutUnifiedCommand
        {
            get
            {
                if (_outPutUnifiedCommand == null)
                    _outPutUnifiedCommand = new RelayCommand(param => this.OutPutUnified(), param => this.CanOutPutUnified());

                return _outPutUnifiedCommand;
            }
        }
        public ICommand RunShearCommand
        {
            get
            {
                if (_outPutSummaryCommand == null)
                    _outPutSummaryCommand = new RelayCommand(param => this.OutPutSummary(), param => this.CanOutPutSummary());

                return _outPutSummaryCommand;
            }
        }
        
        void OutPutUnified()
        {
            string filename = string.Empty;
            SaveFileDialog sf = new SaveFileDialog();

            sf.Title = "save data";
            sf.Filter = "CSV|*.csv";
            sf.DefaultExt = ".csv";
            sf.FileName = "Unified" + "_" + DateTime.Now.ToShortDateString().Replace(@"/", "") + ".csv";

            sf.ShowDialog();
            if (sf.FileName != "")
            {
                filename = sf.FileName;

            }
            else
            {
                return;
            }
             

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {

                DataIsProcessing = false;

            };
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    DataIsProcessing = true;
                    if (!_unifiedDataHasBeenCreated)
                        BuildDataTable();

                    Utils.OutputFile(_unifiedData, filename);
               
                };
            worker.RunWorkerAsync ();
                    }
        bool CanOutPutUnified()
        {
            
            if (HubHeightRow == null || UpperHeightRow == null || LowerHeightRow == null)
                return false;
            if (IsAllSelected())
                return true;
            else
                return false;
            
        }
        void OutPutSummary()
        {
            BackgroundWorker worker = new BackgroundWorker();
            string filename = string.Empty;
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "save data";
            sf.Filter = "Excel|*.xlsx";
            sf.DefaultExt = ".xlsx";
            sf.FileName = "Unified" + "_StnSummary_" + DateTime.Now.ToShortDateString().Replace(@"/", "");
             DialogResult result= sf.ShowDialog();
             if (result == DialogResult.Cancel)
             {
                 return;
             }

            if (sf.FileName != "" )
            {
                filename = sf.FileName;

            }
            else
            {
                return;
            }



            
                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    SummaryIsProcessing=false;
                };
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    XbyYShearStationSummary summary = null;

                    try
                    {
                        SummaryIsProcessing = true;

                       //create the DataTable
                        if (_unifiedData != null)
                        {
                            _unifiedData.Clear();
                        }
                        BuildDataTable ();
                        //create column collection 
                        ColumnCollection =new SessionColumnCollection (_unifiedData );


                        //add column def and add configs 

                        ISessionColumn hubws = ColumnCollection[HubHeight.ToString() + "m"];
                        hubws.ColumnType = SessionColumnType.WSAvgShear;
                        hubws.IsCalculated = true;
                        hubws.IsComposite = true;
                        SensorConfig config = new SensorConfig() { StartDate = _startDate, EndDate = _endDate, Height = HubHeight };
                        hubws.Configs.Add(config);

                        ISessionColumn upperws = ColumnCollection[UpperHeight.ToString () + "m"];
                        upperws.ColumnType = SessionColumnType.WSAvg;
                        upperws.IsComposite = true;
                        config = new SensorConfig() { StartDate = _startDate, EndDate = _endDate, Height = UpperHeight };
                        upperws.Configs.Add(config);
                        
                        ISessionColumn lowerws = ColumnCollection[LowerHeight.ToString() + "m"];
                        lowerws.ColumnType = SessionColumnType.WSAvg;
                        lowerws.IsComposite = true;
                        config = new SensorConfig() { StartDate = _startDate, EndDate = _endDate, Height = LowerHeight  };
                        lowerws.Configs.Add(config);

                        ISessionColumn wd = ColumnCollection["WD"];
                        wd.ColumnType = SessionColumnType.WDAvg;
                        wd.IsComposite = true;
                        config = new SensorConfig() { StartDate = _startDate, EndDate = _endDate };
                        wd.Configs.Add(config);


                        //get axis selections from UI
                        IAxis Xaxis = GetAxis(_xShearAxis, _xBinWidth);
                        IAxis Yaxis = GetAxis(_yShearAxis, _yBinWidth);

                        //calculate alpha 
                        AlphaFactory afactory = new AlphaFactory();
                        Alpha alpha = (Alpha)afactory.CreateAlpha(_unifiedData,AlphaFilterMethod .Coincident , upperws, lowerws, Xaxis, Yaxis);
                        alpha.SourceDataSet = this.DisplayName;
                        alpha.CalculateAlpha();

                        string xBin = string.Empty;
                        string yBin = string.Empty;

                        if (Xaxis.AxisType == AxisType.WD)
                        {
                            var wdaxis = (WindDirectionAxis)Xaxis;
                            xBin = " " + wdaxis.BinWidth.ToString() + " deg";
                        }
                        if (Xaxis.AxisType == AxisType.WS)
                        {
                            var wsaxis = (WindSpeedAxis)Xaxis;
                            xBin = " " + wsaxis.BinWidth.ToString() + " m/s";
                        }
                        if (Yaxis.AxisType == AxisType.WD)
                        {
                            var wdaxis = (WindDirectionAxis)Yaxis;
                            yBin = " " + wdaxis.BinWidth.ToString() + " deg";
                        }
                        if (Yaxis.AxisType == AxisType.WS)
                        {
                            var wsaxis = (WindSpeedAxis)Yaxis;
                            yBin = " " + wsaxis.BinWidth.ToString() + " m/s";
                        }

                        //Set up column metadata for the summary run 
                        summary = new XbyYShearStationSummary(ColumnCollection,_unifiedData.AsDataView(), 30, 10, 2, alpha);

                        summary.CreateReport(filename);
                        SummaryIsProcessing = false;
                    }
                    catch (ApplicationException e)
                    {
                        MessageBox.Show("Error calculating station summary: " + e.Message + " " + e.Source);
                        StreamWriter fs = new StreamWriter(Path.GetDirectoryName(filename) + @"\LOG_" + Path.GetFileNameWithoutExtension(filename) + ".txt", false);
                        summary.log.ForEach(c => fs.WriteLine(c));
                        fs.Close();
                    }
                    finally
                    {
                        
                        SummaryIsProcessing = false;
                    }

                };
                worker.RunWorkerAsync();
            
           
        
        }
        bool CanOutPutSummary()
        {
            if (HubHeightRow ==null || UpperHeightRow==null || LowerHeightRow ==null)
                return false;
            if (XShearAxis != AxisType.None  & YShearAxis != AxisType.None & IsAllSelected ())
            {
                return true;
            }
            return false;
        }
        void BuildDataTable()
        {
           //build list of datetime for complete daterange of all configs 

                _unifiedData = new DataTable();
                List<double> HubHtValues = new List<double>();
                List<double> UpperHtValues = new List<double>();
                List<double> LowerHtValues = new List<double>();
                List<double> WDValues = new List<double>();
                List<DateTime> DateValues = new List<DateTime>();

                _startDate = _acvm.Configs.Select(c => c.DataSetStartDate).Min();
                _endDate = _acvm.Configs.Select(c => c.DataSetEndDate).Max();
                
                for (int i = 0; i < _acvm.Configs.Count; i++)
                {

                    
                    DataTable Data = _acvm.Configs[i].DownloadedData[0];
                   //build a list for upper,lower,and hub height ws
                    int wdindex = _acvm.Configs[i].ColumnCollection.WDComp(_startDate);
                    int DateIndex = _acvm.Configs[i].ColumnCollection.DateIndex;


                    foreach (DataRow dr in Data.Rows)
                    {
                        
                        int j = HubHeightRow[i].SelectedColumn.ColIndex;
                        {
                            if (!Convert.IsDBNull(dr[j]) & dr[j].ToString().Length > 0)
                            {
                                HubHtValues.Add(double.Parse(dr[j].ToString()));
                            }
                            else
                            {
                                HubHtValues.Add(-9999.99);
                            }
                            
                        }

                        j = DateIndex;
                        {
                            if (!Convert.IsDBNull(dr[j]) & dr[j].ToString().Length > 0)
                            {
                                DateValues.Add(DateTime.Parse(dr[j].ToString()));
                            }
                            

                        }
                        j = wdindex;
                        {
                            if (!Convert.IsDBNull(dr[j]) & dr[j].ToString().Length > 0)
                            {
                                WDValues.Add(double.Parse(dr[j].ToString()));
                            }
                            else
                            {
                                WDValues.Add(-9999.99);
                            }

                        }

                        j = UpperHeightRow[i].SelectedColumn.ColIndex;
                        {
                            if (!Convert.IsDBNull(dr[j]) & dr[j].ToString().Length > 0)
                            {
                                UpperHtValues.Add(double.Parse(dr[j].ToString()));
                            }
                            else
                            {
                                UpperHtValues.Add(-9999.99);
                            }
                            
                        }
                        j = LowerHeightRow[i].SelectedColumn.ColIndex;
                        {
                            if (!Convert.IsDBNull(dr[j]) & dr[j].ToString().Length > 0)
                            {
                                LowerHtValues.Add(double.Parse(dr[j].ToString()));
                            }
                            else
                            {
                                LowerHtValues.Add(-9999.99);
                            }
                           
                            
                        }

                        
                    }

                }//end for i
               
                Utils.AddColtoDataTable<DateTime>("DateTime", DateValues, _unifiedData);
                Utils.AddColtoDataTable<double>(HubHeight.ToString () + "m", HubHtValues,_unifiedData );
                Utils.AddColtoDataTable<double>(UpperHeight.ToString() + "m", UpperHtValues, _unifiedData);
                Utils.AddColtoDataTable<double>(LowerHeight.ToString() + "m", LowerHtValues, _unifiedData);
                Utils.AddColtoDataTable<double>("WD", WDValues, _unifiedData);


                DateTimeSequence sequence = new DateTimeSequence(_unifiedData, 0);
                DataPrep prep = new DataPrep(_unifiedData, 0);
                try
                {
                    prep.FillMissingdates(sequence.GetMissingTimeStamps());
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("The file is too big. Volatile memory has been exceeded while filling missing dates.");
                }

            }
        bool IsAllSelected()
        {
            bool allSelected=true;
            foreach (UnifiedHeightViewModel uhvm in HubHeightRow)
            {

                if (uhvm.SelectedColumn == null)
                {
                    allSelected = false;
                    return allSelected;
                }

            }
            foreach (UnifiedHeightViewModel uhvm in UpperHeightRow)
            {

                if (uhvm.SelectedColumn == null)
                {
                    allSelected = false;
                    return allSelected;
                }
            }
            foreach (UnifiedHeightViewModel uhvm in LowerHeightRow)
            {

                if (uhvm.SelectedColumn == null)
                {
                    allSelected = false;
                    return allSelected;
                }
            }
            return allSelected;
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
                        axis.SessionColIndex = ColumnCollection.WDComp(ColumnCollection.DataSetStart);
                        return axis;

                    }
                case AxisType.WS:
                    {
                        int i = ColumnCollection.UpperWSComp(ColumnCollection.DataSetStart);
                        IAxis axis = factory.CreateAxis(2, ColumnCollection.Data.AsDataView(), i);
                        axis.SessionColIndex = i;
                        return axis;

                    }
                default:
                    return null;

            }
        }
        IAxis GetAxis(AxisType type, double binwidth)
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
        

        
    }
}
