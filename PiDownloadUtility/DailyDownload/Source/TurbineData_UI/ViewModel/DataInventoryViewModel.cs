using System;
using System.Collections.Generic;
using System.Collections .ObjectModel ;
using System.Linq;
using System.Text;
using System.Data;
using TurbineDataUtility.Model;

namespace Analysis_UI
{
    public class DataInventoryViewModel:ViewModelBase 
    {
        ObservableCollection<Tag> _tags;

        
        public DataInventoryViewModel(List<Tag> tags)
        {
            Tags = new ObservableCollection<Tag>(tags);
        }

        public ObservableCollection<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                
            }
        }
        

        
    }

}
