using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class DateTimeAxisFactory
    {
        private AxisType _type;
        public DateTimeAxisFactory(AxisType type)
        {
            _type=type;
        }
        public IAxis CreateAxis()
        {
            switch (_type)
            {
                case AxisType.Month:
                    return new MonthAxis();
                    
                case AxisType.Hour:
                    return new HourAxis();
                default:
                    return null;

            }
        }
    }
}
