using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    class Average:IAggregate 
   {
       
        public Average()
        {
           
        }

        public double Calculate(List<double> sourceValues)
        {
            double result;
            if (sourceValues.Count > 0)
            {
                result = sourceValues.Average ();

            }
            else
            {
                result = -9999;
            }
            return result;

        }
    }
}
