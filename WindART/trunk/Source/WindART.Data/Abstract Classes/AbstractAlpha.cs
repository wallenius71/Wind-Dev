using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace WindART
{
    public abstract class AbstractAlpha : INotifyPropertyChanged
    {

        

        protected IAxis _xAxis;
        protected IAxis _yAxis;
        protected ISessionColumn _upperws;
        protected ISessionColumn _lowerws;
        protected List<DataRow> _coincident;
        protected string _name;

        
        protected DataTable _data;
        protected DateTime _dataSetStart;
        protected DateTime _endDate;

        public bool UpperAvgCalculated { get; private set; }
        public bool LowerAvgCalculated { get; private set; }
        public bool AlphaCalculated { get; private set; }
        public bool ShouldRecalculate { get; set; }
        
        

        public DateTime End
        {
            get { return _endDate; }
            set {
                    _endDate=value;
                    OnPropertyChanged("End");
                }
        }
        public DateTime Start
        {
            get { return _dataSetStart; }
            set
            {
                _dataSetStart = value;
                OnPropertyChanged("Start");
            }
        }
        public String SourceDataSet { get;  set; }
        public String SourceConfig { get; set; }
        public abstract string Name { get; }
        public double[,] UpperAvg { get; set; }
        public double[,] LowerAvg { get; set; }
        public double[,] Alpha    { get;set;  }
        public int[,] UpperAvgCount { get; set; }
        public int[,] LowerAvgCount { get; set; }
        public int[,] AlphaCount { get; set; }
        

        public IAxis Xaxis
        {
            get { return _xAxis; }
            set { _xAxis = value; }
        }
        public IAxis Yaxis
        {
            get { return _yAxis; }
            set { _yAxis = value; }
        }

        public abstract void CalculateAlpha();
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        
            
        
    }
}
