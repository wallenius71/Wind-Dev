using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class SummaryGridColumn
    {
        
        object _value;
        //properties
       
        public object Value 
        { 
            get
            {
                return (object)_value;
            }
        }

        //constructor
        public SummaryGridColumn(CalculateGridColumnAlgorithm algorithm)
        {
            
            _value = algorithm.CreateValue();
        }
    
        

        

    }
}
