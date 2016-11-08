using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART.DAL;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Windows.Input;

namespace DataReloadUtility
{
    class ParkedFileViewModel:LogListBase
    {
        private List<string> _files = new List<string>();

        public List<string> Files
        {
            get
            {return _files;}
            set
            {
                _files = value;
                OnPropertyChanged("Files");
            }

        }
        public string AltSiteID
        {
            get
            {
                return _altSiteID;
            }
            set
            {
                _altSiteID = value;
                OnPropertyChanged("AltSiteID");
            }
        }
        public ParkedFileViewModel(IDataRepository repository)
        {
            _repository = repository;
            _displayName = "Parked Files";
            
        }

       

        
    }
}
