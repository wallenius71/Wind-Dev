using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public class VectorAverage:IAggregate 
    {
        
        public VectorAverage(List<Double> Vals)
        {
           
        }

        public double Calculate(List<double> sourceValues)
        {
            double result;
            if (sourceValues.Count > 0)
            {
                result =(180/Math.PI)* Math.Atan2(sourceValues.Sum(c => Math.Sin((Math.PI * c) / 180)), sourceValues.Sum(c => Math.Cos((Math.PI * c )/ 180)));
                result = result >= 0 ? result : result + 360;
            
            }
            else
            {
                result = -9999;
            }
            return result;
        }
    }
}
