using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;

namespace Analysis_UI
{
    public class ViewPiDataViewModel:ViewModelBase 
    {
        ObservableCollection<DataTable> _data=new ObservableCollection<DataTable> ();
        

        public ViewPiDataViewModel(DataTable data)
        {
            Data = new ObservableCollection<DataTable>();
            Data.Add(data);
            DisplayName = "Data";
        }
        public ViewPiDataViewModel(ObservableCollection<DataTable> data)
        {
            data.ToList().ForEach(delegate(DataTable d)
            {
                Data.Add(d);
            });
            
        }

        public ObservableCollection<DataTable> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
                
            }

        }
    }
}
