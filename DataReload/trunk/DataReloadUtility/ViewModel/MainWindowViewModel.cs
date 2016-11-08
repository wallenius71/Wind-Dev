using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel ;
using WindART.DAL;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;



namespace DataReloadUtility
{
    class MainWindowViewModel:ViewModelBase 
    {

        
        private RelayCommand _startLogCommand;
        private LogViewModel _logvm;
        private DataRepository _repository;

        public LogViewModel  LogVM
        {
            get { 
                
                    return _logvm; 
                }
        }
      

        

        public MainWindowViewModel()
        {
            _repository = new DataRepository("10.254.13.3", DataSourceType.SQL2005, "WindArt_JesseRichards", "metteam555");
           // _listCollection.Add(new ParkedFileViewModel (_repository));
           _logvm=new LogViewModel(_repository );
            
        }

        
        public ICommand StartLogCommand
        {
            get
            {
                if (_startLogCommand == null)
                    _startLogCommand = new RelayCommand(param => this.StartLog());

                return _startLogCommand;
            }
        }
       
        
       
      
        private void StartLog()
        {

            
            LogVM.StartLog();
        }
      
    }
}
