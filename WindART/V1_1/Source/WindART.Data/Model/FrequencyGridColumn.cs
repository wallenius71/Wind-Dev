using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class FrequencyGridColumn:CalculateGridColumnAlgorithm 
    {
        double _numerator;
        double _denominator;
            
            public FrequencyGridColumn(double numerator,double denominator )
            {
                _numerator = numerator;
                _denominator = denominator;
            }

            public override object CreateValue()
            {
                try
                {
                    
                    return (object)Math.Round(_numerator / _denominator, 3);                }
                catch
                {
                    throw;
                }
            }
        
    }
}
