using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractAlphaCalculation
    {
        public abstract double[] Calculate(double[] UpperWSAvgGrid, double[] LowerWSAvgGrid);
    }
}
