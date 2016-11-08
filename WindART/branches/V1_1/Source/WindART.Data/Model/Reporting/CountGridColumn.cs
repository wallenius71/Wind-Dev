using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class CountGridColumn:CalculateGridColumnAlgorithm 
    {
        List<object > _list;
        
        public CountGridColumn(List<object> list)
        {
            _list = list;
        }
        public override object CreateValue()
        {
            
            return (object)_list.Count;
           
        }
    }
}
