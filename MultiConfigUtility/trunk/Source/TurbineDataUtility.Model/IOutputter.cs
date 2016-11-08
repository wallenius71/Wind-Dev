using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurbineDataUtility.Model
{
    public interface IOutputter
    {
        bool Output(string filename);
    }
}
