using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IAxis
    {
        double[] AxisValues { get; }
        double Incrementor { get;}
        AxisType AxisType { get;}
        int SessionColIndex { get; set; }
        double BinWidth { get;  }

       
        double GetAxisValue(object element);
        double GetRange(int element, Range startorend);
        
        
        
    }
}
