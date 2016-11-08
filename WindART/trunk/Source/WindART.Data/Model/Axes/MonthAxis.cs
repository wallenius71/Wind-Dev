using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WindART
{
    
    public class MonthAxis:IAxis
    {
        private double[] _axisValues;
        private AxisType _axisType;

        //constructor
        public MonthAxis()
        {
            _axisType = AxisType.Month;
            setAxisValues();
        }
        public  double BinWidth
        {
            get { return 1.0; }
            
        }
        //properties 
        public double Incrementor
        {
            get { return 1; }
        }
        public double[] AxisValues
        {
            get { return _axisValues; }
        }
        public AxisType AxisType
        {
            get
            {
                return _axisType;
            }
        }
        public int SessionColIndex { get; set; }

       
        public virtual double GetAxisValue(object element)
        {
            
            if (element is DateTime)
            {
                DateTime d = (DateTime)element;
                return d.Month-1;
            }
            else if (element is int)
            {
                int i = (int)element;
                return _axisValues[i];
            }
            return default(double);
            
        }
        
        protected virtual  void setAxisValues()
        {
            try
            {
                double[] result = null;
                result = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                _axisValues = result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public double GetRange(int element, Range startorend)
        {
            throw new NotImplementedException();
        }
    }
}
