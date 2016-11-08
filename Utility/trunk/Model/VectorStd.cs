using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public class VectorStd:IAggregate
    {
         List <double> sourceValues=new List<double> ();
         List<double> workVals = new List<double>();
        public VectorStd(List<Double> Vals)
        {
            sourceValues = Vals;
        }

        public double Calculate()
        {
            double result;
            if (sourceValues.Count > 0)
            {
                double mean = new VectorAverage(sourceValues).Calculate();
                int N = sourceValues.Count();
                
                foreach (double d in sourceValues)
                {
                    //normalize values that cross 360 
                    double a=d-mean<0?(d-mean)+360:d-mean;
                    double b=mean-d<0?(mean-d)+360:mean-d;
                    //find the squared error between value and mean 
                    workVals.Add(Math.Pow((a<b?a:b),2));
                    
                }
                
                //take square root of Mean Squared Error 
                result = Math.Sqrt (workVals.Average ());
                

            }
            else
            {
                result = -9999;
            }
            return result;
        }
    }
}
