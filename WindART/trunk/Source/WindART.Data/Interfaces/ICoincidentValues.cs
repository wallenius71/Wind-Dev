using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public interface ICoincidentValues
    {
        Dictionary<int,List<double>> GetValues();
        
    }
}
