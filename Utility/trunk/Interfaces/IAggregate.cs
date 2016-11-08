using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public interface IAggregate
    {
        double Calculate(List<double> sourceValues);
    }
}
