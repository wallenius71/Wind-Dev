using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    class ExplicitValueGridColumn:CalculateGridColumnAlgorithm 
    {
        object _value;

        public ExplicitValueGridColumn(object value)
        {
            _value = value;
        }
        public override object CreateValue()
        {
            return _value;
        }
    }
}
