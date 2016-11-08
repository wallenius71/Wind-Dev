using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace WindART
{
    public class AxisFactory
    {
        
        
        public IAxis CreateAxis(AxisType type)
        {
            switch (type)
            {
                case AxisType.Month:
                    return new MonthAxis();
                    
                case AxisType.Hour:
                    return new HourAxis();
                default:
                    return null;

            }
        }

        public IAxis CreateAxis(double binwidth)
        {
            return new WindDirectionAxis(binwidth);
        }

        public IAxis CreateAxis(double binwidth, DataView data, int wsindex)
        {
            return new WindSpeedAxis(binwidth, data, wsindex);
        }
    }
}
