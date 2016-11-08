using System;
using System.Collections.Generic;
using WindART;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Data;
using System.Windows.Forms;


namespace MultiConfigTool_UI
{
    public class SingleConfigShearViewModel:ViewModelBase
    {
        RelayCommand _deriveNewWSCommand;
        RelayCommand _outPutStationSummaryCommand;
        RelayCommand _outPutDataCommand;

        public delegate void UpdatedDerivedWSCollectionHandler(SingleConfigViewModel shearedws);
        public event UpdatedDerivedWSCollectionHandler DerivedWSCollectionUpdated;

       private ShearedWS _shearedWS;
       private SingleConfigViewModel _collection;
       
        private double? _upperWS;
        private double? _lowerWS;
        private string _sourceDataSetName;
        private double? _upperRecovery;
        private double? _lowerRecovery;
        private double? _derivedRecovery;
        private int? _derivedWSColIndex;
        private ObservableCollection<ShearFromViewModel> _shearFrom = new ObservableCollection<ShearFromViewModel >();
        private ObservableCollection<SessionColumn> _possibleComposites = new ObservableCollection<SessionColumn>();
        
        bool _isProcessingDerived;
        bool _isOutputtingSummary;
        bool _isOutPuttingData;
        DateTime? _start;
        DateTime? _end;

        //todo: avalailable comp heights 

       public SingleConfigShearViewModel(SingleConfigViewModel scvm,string datasetname)
       {
           _collection = scvm;
           _shearedWS = new ShearedWS(scvm.ColumnCollection);
           _sourceDataSetName = datasetname;
       }
       public SingleConfigShearViewModel()
       {
           _shearedWS = new ShearedWS();
           SensorConfig sc = new SensorConfig();
          
       }

        #region properties

        public bool IsProcessingDerived
       {
           get { return _isProcessingDerived; }
           set
           {
               _isProcessingDerived = value;
               OnPropertyChanged("IsProcessingDerived");
           }
       }
        public bool IsOutputtingSummary
        {
            get { return _isOutputtingSummary; }
            set
            {
                _isOutputtingSummary = value;
                OnPropertyChanged("IsOutputtingSummary");
            }
        }
        public bool IsOutputtingData
        {
            get { return _isOutPuttingData; }
            set
            {
                _isOutPuttingData = value;
                OnPropertyChanged("IsOutPuttingData");
            }
        }
        public string OutPutFileLocation { get; set; }
        public ShearedWS ShearedWS
       {
           get { return _shearedWS; }
           set
           {
               _shearedWS = value;
               if (_shearedWS != null)
               {
                   _start = _shearedWS.DateStart;
                   _end = _shearedWS.DateEnd;
               }
               OnPropertyChanged("ShearedWS");

               
               
           }
       }
        public string SourceDataSetName
       {
           get { return _sourceDataSetName; }
           set
           {
               _sourceDataSetName = value;
               OnPropertyChanged("SourceDataSetName");
           }
       }
        public DateTime? Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }
        public SingleConfigViewModel Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                if (_collection != null)
                {
                    _shearedWS.ColumnCollection = _collection.ColumnCollection;
                    Start = _collection.ColumnCollection.DataSetStart;
                    End = _collection.ColumnCollection.DataSetEnd;
                }
                else
                {
                    Start = null;
                    End = null;
                    _shearFrom.Clear();
                    ShearedWS = null;


                }
                OnPropertyChanged("Collection");
                OnPropertyChanged("UpperWS");
                OnPropertyChanged("LowerWS");
                OnPropertyChanged("UpperRecovery");
                OnPropertyChanged("LowerRecovery");
                OnPropertyChanged("ShearFrom");
                OnPropertyChanged("PossibleComposites");
                
                
            }
        }
        public DateTime? End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
                OnPropertyChanged("End");
            }
        }
        public double? UpperWS
        {
            get
            {
                //get the upper comp height

               

                if (_shearedWS !=null && _shearedWS.ColumnCollection !=null & _collection !=null)
                {
                    _upperWS = _collection.ColumnCollection[_collection.ColumnCollection.UpperWSComp((DateTime )_start)].getConfigAtDate((DateTime)_start).Height;
                    return _upperWS;
                }

                else
                {
                    return null;
                }
                
                
                    
                
            }

            set
            {
                _upperWS = value;
                OnPropertyChanged("UpperWS");
            }
            
        }
        public double? LowerWS
        {
            get {

                    if (_shearedWS !=null && _shearedWS.ColumnCollection != null & _collection !=null)
                    {
                        //get the lower comp height
                        _lowerWS = _collection.ColumnCollection[_collection.ColumnCollection.LowerWSComp((DateTime)Start)].getConfigAtDate((DateTime)Start).Height;
                        return _lowerWS;
                    }
                    else
                    {
                        return null;

                    }
                }

            set
            {
                _lowerWS = value;
                OnPropertyChanged("LowerWS");
            }
            
        }
        public double? UpperRecovery
        {
            get
            {
                if (((_upperRecovery == null & Start != null) || _upperRecovery != null) & _collection != null)
            {
                DateTime start = _collection.ColumnCollection.DataSetStart;
                    //get the lower comp height
                    _upperRecovery = _collection.ColumnCollection[_collection.ColumnCollection.UpperWSComp(start)].RecoveryRate ;

                }
                else
                {
                    return null ;
                }
                    return _upperRecovery; 
                }
        }
        public double? LowerRecovery
        {
            get
            {
                if (((_lowerRecovery == null & Start != null || _lowerRecovery != null) || _lowerRecovery !=null) & _collection != null)
                {
                    //get the lower comp height
                    _lowerRecovery = _collection.ColumnCollection[_collection.ColumnCollection.LowerWSComp((DateTime)Start)].RecoveryRate;

                }
                else
                {
                    return null;
                }
                return _lowerRecovery;
            }
        }
        public double? ShearHt
        {
            get {
            
                if (_shearedWS ==null || _shearedWS.ShearHt == default(double) )
                    return null;
                else
                    return _shearedWS.ShearHt; 
                }
            set {
                double usevalue=default(double);
                if (value == null)
                    usevalue = default(double);
                else
                    usevalue = (double)value;
                _shearedWS.ShearHt = usevalue;
                    OnPropertyChanged("ShearHt");
                }
        }
        public ObservableCollection<SessionColumn> PossibleComposites
        {
             
            get
            {
                if (_possibleComposites.Count == 0 & _shearedWS!=null)
                {
                  _shearedWS.ColumnCollection.CompositeWSCols().ConvertAll<SessionColumn>(c=>(SessionColumn)c).ForEach 
                      (c=>_possibleComposites.Add(c));
                }
                return _possibleComposites;
            }

           
        
        }
        public ObservableCollection<ShearFromViewModel> ShearFrom
        {
            get {
                if (_shearedWS !=null && _shearedWS.ColumnCollection != null && _shearFrom.Count==0)
                {
                    for (int i = 1; i <= _shearedWS.ColumnCollection.CompositeWSCols().Count; i++)
                    {   
                        SessionColumn newSC=new SessionColumn ();
                        ShearFromViewModel sfvm=new ShearFromViewModel() { Key = i, Value = newSC };
                        sfvm.PropertyChanged +=new PropertyChangedEventHandler(sfvm_PropertyChanged);
                        _shearFrom.Add(sfvm);
                        //also add this to the shearfrom property in shearedws
                        _shearedWS.ShearFrom .Add(i,newSC);
                        OnPropertyChanged("ShearFrom");
                    }
                }
                        return _shearFrom;
                }

            
                }

        void  sfvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
 	        if(e.PropertyName =="Value")
            {
                //update the underlying shearfromcollection in the model
                ShearFromViewModel thissfvm=(ShearFromViewModel)sender;
                ShearedWS.ShearFrom[thissfvm.Key]=thissfvm.Value ;
            }
        }
        
        public double? DerivedRecovery
        {
            get
            {
                if (_derivedWSColIndex !=null )
                {
                    _derivedRecovery = _collection.ColumnCollection[(int)_derivedWSColIndex].RecoveryRate;
                    return (double)_derivedRecovery;
                }
                else
                {
                    return null;
                }
            }
        }
        public AbstractAlpha Alpha
        {
            get
            {
                if (_shearedWS != null)
                    return _shearedWS.Alpha;
                else
                    return null;

            }
            set
            {
                _shearedWS.Alpha = value;
                OnPropertyChanged("Alpha");
            }
        }
        public bool DerivedWSCalculated { get; set; }

        #endregion

        #region commands

        public ICommand DeriveNewWSCommand
        {
            get
            {
                if (_deriveNewWSCommand == null)
                    _deriveNewWSCommand = new RelayCommand(param => this.ShearUpWS(), param => this.CanShearUpWS());

                return _deriveNewWSCommand;
            }
        }
        public ICommand OutPutDataCommand
        {
            get
            {
                if (_outPutDataCommand == null)
                    _outPutDataCommand = new RelayCommand(param => this.OutPutData(), param => this.CanOutPutData());

                return _outPutDataCommand;
            }
        }
        public ICommand OutPutStationSummaryCommand
        {
            get
            {
                if (_outPutStationSummaryCommand == null)
                    _outPutStationSummaryCommand = new RelayCommand(param => this.OutPutStationSummary(), param => this.CanOutPutStationSummary());

                return _outPutStationSummaryCommand;
            }
        }

        #endregion

        #region methods

        public void ShearUpWS()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                DerivedWSCollectionUpdated(Collection);
                OnPropertyChanged("DerivedRecovery");
            };

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    IsProcessingDerived = true;
                    _shearedWS.CreateNewWS();
                    _derivedWSColIndex = _shearedWS.Index;
                    DerivedWSCalculated = true;
                    IsProcessingDerived = false;
                    
                };
            worker.RunWorkerAsync();
        }
        public bool CanShearUpWS()
        {
            if (Alpha == null || ShearHt == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        void OutPutData()
        {
            BackgroundWorker worker = new BackgroundWorker();
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "save data";
            sf.Filter="CSV|*.csv";
            sf.DefaultExt = ".csv";
            sf.FileName = this.Collection .DisplayName  + "_" +  DateTime.Now.ToShortDateString().Replace(@"/", "");
            sf.ShowDialog();
            if (sf.FileName != "")
            {
                OutPutFileLocation = sf.FileName;
                
            }
            else
            {
                return;
            }
            try
            {
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
           {
               IsOutputtingData = true;
               string filename = OutPutFileLocation;
               // Create the CSV file to which grid data will be exported.
               StreamWriter sw = new StreamWriter(filename, false);

               //column heads
               foreach (DataColumn col in _collection.DownloadedData[0].Columns)
               {
                   sw.Write(col.ColumnName.Replace("#", "_"));

                   if (_collection.DownloadedData[0].Columns.IndexOf(col) < _collection.DownloadedData[0].Columns.Count - 1)

                       sw.Write(",");
               }



               sw.Write(sw.NewLine);


               //Now write all the rows.
               DataTable Data = _collection.DownloadedData[0];

               DataView dv = Data.AsDataView();
               dv.Sort = "DateTime ASC";
               foreach (DataRowView dr in dv)
               {
                   for (int i = 0; i < Data.Columns.Count; i++)
                   {
                       if (!Convert.IsDBNull(dr[i]) & dr[i].ToString().Length > 0)
                       {
                           sw.Write(dr[i].ToString());
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
               IsOutputtingData = false;

           };
                worker.RunWorkerAsync();
            }



            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                IsOutputtingData = false;
            }
        }
        bool CanOutPutData()
        {
            if (DerivedWSCalculated & _shearedWS !=null)
                return true;
            else
                return false;
        }
        void OutPutStationSummary()
        {

            BackgroundWorker worker = new BackgroundWorker();
            string filename = string.Empty;
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "save data";
            sf.Filter = "Excel|*.xlsx";
            sf.DefaultExt = ".xlsx";
            sf.FileName = this.Collection.DisplayName + "_StnSummary_" + DateTime.Now.ToShortDateString().Replace(@"/", "");
            sf.ShowDialog();

            if (sf.FileName != "")
            {
                filename = sf.FileName;

            }
            else
            {
                return;
            }

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                XbyYShearStationSummary summary = null;
                try
                {
                    IsOutputtingSummary = true;
                      summary = new XbyYShearStationSummary(_collection.ColumnCollection,
                           _collection.DownloadedData[0].AsDataView(), 30, 10, 2, (int)_derivedWSColIndex, _shearedWS.Alpha);
                    summary.CreateReport(filename);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error calculating station summary: " + e.Message + " " + e.Source);
                    StreamWriter fs = new StreamWriter(Path.GetDirectoryName(filename) + @"\LOG_" + Path.GetFileNameWithoutExtension(filename) + ".txt", false);
                    summary.log.ForEach(c => fs.WriteLine(c));
                    fs.Close();
                }
                finally
                {
                    //MessageBox.Show("done!");
                    IsOutputtingSummary = false;
                }
                
               
               
            };
            worker.RunWorkerAsync();
            
           
            
        }
        bool CanOutPutStationSummary()
        {
            if (_shearedWS !=null && DerivedWSCalculated && _shearedWS.Alpha != null)
            {
                return true;
            }
            else 
                return false;
        }
        

        #endregion
    }
}
