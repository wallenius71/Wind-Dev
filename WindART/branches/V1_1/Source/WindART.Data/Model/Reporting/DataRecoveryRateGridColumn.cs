using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WindART
{
    public class DataRecoveryRateGridColumn:CalculateGridColumnAlgorithm 
    {
        private List<double> _vals;
        public DataRecoveryRateGridColumn(List<double> vals)
        {
            _vals = vals;
        }

        public override object CreateValue()
        {
            //Console.WriteLine(" vals count =" + _vals.Count);
            var result = from c in _vals.AsEnumerable ()
                         where c > 0
                         select c;
            double num;
            if (result.DefaultIfEmpty ().Count () == 0)
            { 
                num = 0; 
            }
            else
            {
                num = result.Count();
            }
                double rate = Math.Round(100 * (num / _vals.Count), 2);
                Console.WriteLine("recovery rate " + rate);
                return (object)rate;
            

            
        }
    }
}
