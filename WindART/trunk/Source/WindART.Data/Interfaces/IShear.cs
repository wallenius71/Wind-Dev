using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IShear
    {
      // bool IsCalculated {get; set;}
       //  bool CalcMethod { get; set; }

        bool CalculateWindSpeed (double i,out ShearCalculationGridCollection grid);
    }
}
