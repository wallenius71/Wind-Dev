using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel ;
using System.Windows;

namespace WindART
{
    [Serializable]
    public class SessionColumn:ISessionColumn,INotifyPropertyChanged 
    {
        //metadata class for columns in the session dataset
        SessionColumnType _columnType;
        string _units;
        ObservableCollection <ISessionColumn> _childColumns=new ObservableCollection <ISessionColumn>();
        ObservableCollection<ISensorConfig> _configs = new ObservableCollection <ISensorConfig>();
        string _displayName = String.Empty;
        Int32 _possibleRecords = 0;
        Int32 _validRecords = 0;
        double _recoveryrate = 0;
        int _colIndex;
        string _colName;
        bool _isComposite=false;
        double _height;

        public SessionColumn(int colindex,string colname)
        {
            //set reference to datacolumn 
            _colIndex = colindex;
            _colName = colname;
        }
        public SessionColumn() { }

        public ObservableCollection  <ISessionColumn> ChildColumns
        {
            get
            {
                return _childColumns;
            }
            set
            {
                _childColumns = value;
                OnPropertyChanged("ChildColumns");
            }
        }
        
        public int ColIndex
        {
            get
            {
                return _colIndex;
            }
            
        }
        public string ColName
        {
            get
            {
                return _colName;
            }
            set
            {
                _colName = value;
            }

        }
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }
        public string Units
        {
            get
            {
                return _units;
            }
            set
            {
                _units = value;
            }
        }
        public int PossibleRecords
        {
            get
            {
                return _possibleRecords;
            }
            set
            {
                _possibleRecords = value;
            }

        }
        public int ValidRecords
        {
            get
            {
                return _validRecords;
            }
            set
            {
                _validRecords = value;
            }
        }
        public double RecoveryRate
        {
            get
            {
                return _recoveryrate;
            }
            set
            {
                _recoveryrate = value;
                OnPropertyChanged("RecoveryRate");
            }
        }
        public SessionColumnType ColumnType
        {
            get
            {
                return _columnType;
            }
            set
            {
                _columnType = value;
                OnPropertyChanged("ColumnType");
            }
        }

        public bool IsCalculated { get; set; }
        public bool IsComposite
        {
            get
            {
                return _isComposite;
            }
            set
            {
                _isComposite = value;
                OnPropertyChanged("IsComposite");
            }
        }
        public WindSpeedConfig TempGetSingleConfig
        {
            get
            {
                return (WindSpeedConfig)Configs[0];
            }
            set
            {
                Configs[0] = value;
                OnPropertyChanged("TempGetSingleConfig");
            }
        }
        public SensorConfig TempGetSingleWDConfig
        {
            get
            {
                return (SensorConfig)Configs[0];
            }
            set
            {
                Configs[0] = value;
                OnPropertyChanged("TempGetSingleWDConfig");
            }
        }
        public ObservableCollection <ISensorConfig> Configs
        {
            get
            {
                return _configs;
            }
        }
        public double Height
        {
            get {
                    if (Configs.Count > 0)
                        return this.Configs[0].Height;
                    else
                    {
                        return _height;
                    }
                }
            set
            {
                if (Configs.Count > 0)
                    this.Configs[0].Height = value;
                else
                    _height = value;
                    OnPropertyChanged("Height");
            }
        }
        
        //methods
        public ISensorConfig getConfigAtDate(DateTime date)
        {
            //Console.WriteLine("getconfigatDate config count " + _colIndex + ", " +  _configs.Count);
            //return the config of this column at a given date 
            for (int i = 0; i < _configs.Count; i++)
            {
                if (date >= _configs[i].StartDate && date <= _configs[i].EndDate)
                {
                    return _configs[i];
                }
            }
            return null;
        }
        public void addConfig(ISensorConfig config)
        {
            try
            {
                //if(!IsComposite &&( config.Height==default(double) || config.Orientation==default(double)))
                //{
                //    throw new ApplicationException ("Height or Orientation are missing");
                //}
                if (config.StartDate == default(DateTime) || config.EndDate == default(DateTime))
                {
                    throw new ApplicationException("Start or End date of a config can not be null");
                }
                else
                { 
                    //make sure it does not overlap an exiosting config 
                    ISensorConfig startConfig = getConfigAtDate(config.StartDate);
                    ISensorConfig endConfig = getConfigAtDate(config.EndDate);
                    if(startConfig !=null || endConfig !=null)
                    {
                        _configs.Remove(startConfig);
                        _configs.Add(config);
                    }
                    else
                    {
                        _configs .Add(config);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message );
            }
        }



        #region INotifyPropertyChanged Members
        [field:NonSerialized ]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
