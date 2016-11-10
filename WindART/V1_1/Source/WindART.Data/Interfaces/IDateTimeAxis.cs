using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IDateTimeAxis:IAxis
    {
        
        int ReturnAxisValue(DateTime InputVal);
       
    }
}
