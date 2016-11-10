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
        protected virtual void setAxisValues()
        {
            int months=Utils.TimeSpanMonths(_start, _end);
            _axisValues =new double[months];
            for (int i = 0; i < months; i++)
            {   //axis values will be the number of months from the start date 
                _axisValues[i] = i;
            }
        }
        public override double BinWidth { get { return 1.0; } set{} }
        public virtual string ReturnAxisValueHeader(int element)
        {
            DateTime workdate = _start.AddMonths(element);
            return workdate.Month + "/" + workdate.Year;
        }
       
        


        public override double GetAxisValue(object element)
        {
            throw new NotImplementedException ("Get axis value on month year axis not implemented");
                //DateTime d = (DateTime)element;
                //return d.Year;
        }

        public override int SessionColIndex
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override double GetRange(int element, Range startorend)
        {
            throw new NotImplementedException();
        }
    }
}
