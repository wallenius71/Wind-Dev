using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public class Maximum:IAggregate 
   {
      
        public Maximum()
        {
          
        }

        public double Calculate(List<double> sourceValues)
        {
            double result;
            if (sourceValues.Count > 0)
            {
                result = sourceValues.Max();

            }
            else
            {
                result = -9999;
            }
            return result;

        }
    }
}
