using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractMergeWS
    {
        public abstract double[,] Merge(Dictionary<int,Dictionary<double,List<double>>> sourceWS);
    }
}
