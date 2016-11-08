using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TurbineDataUtility.Model;
using WindART.DAL;
using System.Data;

namespace Analysis_UI
{
    public class SelectDataSourceViewModel : ViewModelBase
    {
        #region fields
        ObservableCollection<string> _sites=new ObservableCollection<string>();
        BaseDataSourceView  _dataSourceViewModel;
        string _selectedDataSource;
        #endregion

        #region constructor
        public SelectDataSourceViewModel()
        {
            DisplayName = "Select Data Source";
        }
        #endregion

        #region properties
            
            public List<string> servers
            {
                get
                {
                    return new List<string>
                    {
                        "Fujitsu-WindART","Fujitsu-Ops","Houston Ops", "File"
                    };
                }
            }
            public BaseDataSourceView  DataSourceViewModel
            {
                get
                {
                    return _dataSourceViewModel;
                }
                set
                {
                    
                    _dataSourceViewModel = value;
                    OnPropertyChanged("dataSourceViewModel");
                }
            }
            public string SelectedDataSource
            {
                get
                {
                    return _selectedDataSource;

                }

                set
                {
                    _selectedDataSource = value;
                    OnPropertyChanged("SelectedDataSource");
                    switch (_selectedDataSource)
                    {
                        case "Fujitsu-WindART":
                            DataSourceViewModel = new WindARTServerViewModel();
                            break;
                        case "Fujitsu-Ops":
                            DataSourceViewModel = new FujitsuOpsServerViewModel();
                            break;
                        case "Houston Ops":
                            DataSourceViewModel = new HoustonOpsServerViewModel();
                            break;
                        case "File":
                            DataSourceViewModel=new FileViewModel();
                            break;

                    }

                }
            }
            
        #endregion 


    }
}
