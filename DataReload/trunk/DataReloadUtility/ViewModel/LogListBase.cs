using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using WindART.DAL;

namespace DataReloadUtility
{
    public abstract class LogListBase:ViewModelBase 
    {
        protected IDataRepository _repository;
        protected string _displayName;
        protected string _altSiteID;
        
        private ObservableQueue<LogItem> _logList = new ObservableQueue<LogItem>();
        public ObservableQueue<LogItem> LogList
        {
            get { return _logList; }
            set
            {
                _logList = value;
                OnPropertyChanged("LogList");
               
                
                
                
            }
        }
        
    }
}
