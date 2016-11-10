using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IWindDirectionAxis:IAxis
    {
        double BinWidth { get; set; }
        
    }
}
