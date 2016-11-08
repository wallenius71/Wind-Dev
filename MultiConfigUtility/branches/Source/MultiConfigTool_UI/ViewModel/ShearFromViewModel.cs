using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART;

namespace MultiConfigTool_UI
{
    public class ShearFromViewModel:ViewModelBase 
    {
        
        SessionColumn _value;
        public int Key { get; set; }
        public SessionColumn Value {
            get { return _value; }
            set {
                    _value = value;
                    OnPropertyChanged("Value");
                }

            
        }
        
    }
}
