using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class MonthYearAxis:AbstractAxis 
    {
        
        private DateTime _start;
        private DateTime _end;
        public MonthYearAxis(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            _axisType = AxisType.MonthYear;
            setAxisValues();
        }
        public override double Incrementor
        {
            get
            {
                return 1;
            }
        }
        protected override void setAxisValues()
        {
            int months=Utils.TimeSpanMonths(_start, _end);
            _axisValues =new double[months];
            for (int i = 0; i < months; i++)
            {   //axis values will be the number of months from the start date 
                _axisValues[i] = i;
            }
        }
        public virtual string ReturnAxisValueHeader(int element)
        {
            DateTime workdate = _start.AddMonths(element);
            return workdate.Month + "/" + workdate.Year;
        }
        public virtual int ReturnAxisValueYear(int element)
        {
            DateTime workdate = _start.AddMonths(element);
            return workdate.Year;
        }
    }
}
