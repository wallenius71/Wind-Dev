using System;
using System.Collections.Generic;
using System.Collections.ObjectModel ;
using System.Linq;
using System.Text;

namespace Analysis_UI
{
    public class MultipleDataSetViewModel:ViewModelBase 
    {
        ObservableCollection<ViewModelBase> _viewModels;

        public MultipleDataSetViewModel(ObservableCollection<ViewModelBase> viewModels)
        {
            _viewModels = new ObservableCollection<ViewModelBase>();
            _viewModels = viewModels;
        }

        public ObservableCollection<ViewModelBase> ViewModels
        {
            get
            {
                return _viewModels;
            }
            set
            {
                _viewModels = value;
            }
        }

    }
}
