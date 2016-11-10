using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART;

namespace WindART_UI
{
    public class ShearGridViewModel:ViewModelBase 
    {
        private ShearCalculationGridCollection  _gridCollection;
        
        public ShearGridViewModel(ShearCalculationGridCollection gridcollection, string gridname)
        {
            DisplayName = gridname;
            GridCollection = gridcollection;
        }

        public ShearCalculationGridCollection GridCollection
        {
            get
            {
                return _gridCollection;
            }
            set
            {
                _gridCollection = value;
                OnPropertyChanged("GridCollection");
            }
        }

        
    }
}
