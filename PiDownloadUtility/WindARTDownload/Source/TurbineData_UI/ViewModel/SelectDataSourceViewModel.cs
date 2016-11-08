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
                        "Fujitsu-Ops","Houston Ops", "Goshen","Cedar_Creek-Site","Flat_Ridge-Site", "Edom_Hills-Site","Titan-Site","Silver_Star-Site","Fowler_01-Site","Fowler_02-Site", "Trinity", "Sherbino_Mesa_2"
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
                        
                        case "Fujitsu-Ops":
                            DataSourceViewModel = new FujitsuOpsServerViewModel();
                            break;
                        case "Houston Ops":
                            DataSourceViewModel = new HoustonOpsServerViewModel();
                            break;
                        case "Cedar_Creek-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Cedar_Creek );
                            break;
                        case "Silver_Star-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Silver_Star);
                            break;
                        case "Goshen":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Goshen);
                            break;
                        case "Titan-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Titan  );
                            break;
                        case "Edom_Hills-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Edom_Hills );
                            break;
                        case "Fowler_01-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Fowler_01 );
                            break;
                        case "Fowler_02-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Fowler_02);
                            break;
                        case "Flat_Ridge-Site":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Flat_Ridge );
                            break;
                        case "Trinity":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Trinity);
                            break;
                        case "Sherbino_Mesa_2":
                            DataSourceViewModel = new HoustonOpsServerViewModel(DataSourceType.Sherbino_Mesa_2);
                            break;

                    }

                }
            }
            
        #endregion 


    }
}
