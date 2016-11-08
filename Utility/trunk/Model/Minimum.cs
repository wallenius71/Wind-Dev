using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public class Minimum:IAggregate 
    {
       
        public Minimum()
        {
        }

        public double Calculate(List<double> sourceValues)
        {
            double result;
            if (sourceValues.Count > 0)
            {
                result = sourceValues.Min();

            }
            else
            {
                result = -9999;
            }
            return result;
        }
    }
}
