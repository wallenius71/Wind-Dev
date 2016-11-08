using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace WindART
{
    public class MonthAxisValueGridColumn:CalculateGridColumnAlgorithm 
    {
        
        int _element;

        public MonthAxisValueGridColumn( int element)
        {
            
            _element = element;
        }
        public override object CreateValue()
        {
            //Console.WriteLine(" requesting y val " + (int)_axis.AxisValues[_element]);
            return (object)CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[_element];

        }
    }
}
