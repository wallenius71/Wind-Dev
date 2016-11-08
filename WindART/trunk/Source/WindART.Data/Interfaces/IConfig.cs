using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IConfig
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        
    }
}
