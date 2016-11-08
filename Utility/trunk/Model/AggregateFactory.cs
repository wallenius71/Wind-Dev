using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsApplication1
{
    public class AggregateFactory
    {
        IAggregate agg;

        public IAggregate Create (string type, List<double> vals)
        {
            switch (type)
            {
                case "Std":
                    agg = new VectorStd(vals);
                    break;

                case "Avg":
                    agg = new VectorAverage(vals);
                    break;
                
                default:
                    agg = new VectorAverage(vals);
                   break;
            }

            return agg; 
        }
    }
}
