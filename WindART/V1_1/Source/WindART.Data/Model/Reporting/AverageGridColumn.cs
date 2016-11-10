using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
   public class AverageGridColumn  : CalculateGridColumnAlgorithm
    {
       List<double> _list;
       public AverageGridColumn(List<double> list)
       {
           _list = list;
       }
   
        public override object CreateValue()
        {
            //no missing values 
            var result = from a in _list.AsEnumerable()
                         where a >= 0
                         select a;
            
            return (object)Math.Round(result.DefaultIfEmpty ().ToList ().Average(),2);
            
        }
    }
}
