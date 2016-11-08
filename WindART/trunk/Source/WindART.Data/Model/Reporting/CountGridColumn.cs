using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class CountGridColumn:CalculateGridColumnAlgorithm 
    {
        List<XbyYCoincidentRow > _list;
        
        public CountGridColumn(List<XbyYCoincidentRow > list)
        {
            _list = list;
        }
        public override object CreateValue()
        {
            
            return (object)_list.Count;
           
        }
    }
}
