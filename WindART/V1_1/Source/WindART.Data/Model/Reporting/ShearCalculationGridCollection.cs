using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class ShearCalculationGridCollection
    {
       
        public double[,] UpperAvg { get; set; }
        public double[,] LowerAvg { get; set; }
        public double[,] Alpha    { get;set;  }
        public int[,] UpperAvgCount { get; set; }
        public int[,] LowerAvgCount { get; set; }

    }
}
