using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractAxis:IAxis
    {
        protected  double[] _axisValues;
        protected AxisType _axisType;

        public virtual double[] AxisValues
        {
            get { return _axisValues; }
        }
        public abstract double Incrementor
        {
            get;
        }
        public abstract double BinWidth { get; set; }

        public virtual AxisType AxisType
        {
            get { return _axisType; }
        }
        public abstract int SessionColIndex { get; set; }
        public abstract double GetRange(int element, Range startorend);
        public abstract double GetAxisValue(object element);
        
    }
}
