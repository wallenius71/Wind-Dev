using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TurbineDataUtility.Model;
using System.Data;
using WindART.DAL;
using System.ComponentModel;
using WindART;
using System.Windows.Threading;

namespace Analysis_UI
{
    public class MainWindowViewModel:ViewModelBase
    {
        #region fields
        ReadOnlyCollection<CommandViewModel> _commands;
        
        
        
        
        
        DataSourceType _datasource;
        ViewModelBase _selectDataSource;
       
        ViewModelBase _workspace;
        private bool _fileIsLoading;
        
        BackgroundWorker worker;

        public MainWindowViewModel()
        {
            ShowSelectDataSource();
        }
        

        #endregion 

        #region properties

            public string Filename { get; set; }
           
            public DataSourceType Datasource
            {
                get
                {
                    return _datasource;
                }
                set
                {
                    _datasource = value;
                    OnPropertyChanged("Datasource");
                }
            }
            public ViewModelBase Workspace
            {
                get { return _workspace; }
                set
                {
                    _workspace = value;
                    OnPropertyChanged("Workspace");
                }
            }
           

        #endregion 

        #region commands
       
        
        public ReadOnlyCollection<CommandViewModel> Commands
            {
                get
                {
                    if (_commands == null)
                    {
                        List<CommandViewModel> cmds = this.CreateCommands();
                        _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                    }
                    return _commands;
                }
            }
        List<CommandViewModel> CreateCommands()
            {
                return new List<CommandViewModel>
                {

                     
                    new CommandViewModel("Select Data Source", 
                        new RelayCommand(param=>this.ShowSelectDataSource()))
                   
                };
            }
         
        
        


        #endregion 


        #region methods 
        
        void ShowSelectDataSource()
        {
            
            if(_selectDataSource ==null)
                _selectDataSource =new SelectDataSourceViewModel();

            Workspace = _selectDataSource;
        }
        
        
        
        
        


        #endregion 

    }
}
