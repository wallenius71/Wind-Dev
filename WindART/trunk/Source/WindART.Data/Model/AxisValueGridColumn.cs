using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class AxisValueGridColumn:CalculateGridColumnAlgorithm 
    {
        IAxis _axis;
        int _element;

        public AxisValueGridColumn(IAxis axis, int element)
        {
            _axis = axis;
            _element = element;
        }
        public override object CreateValue()
        {
            
            return (object)_axis.AxisValues[_element];
        }
    }
}
