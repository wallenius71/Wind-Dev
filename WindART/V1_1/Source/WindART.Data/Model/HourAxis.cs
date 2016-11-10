using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    
        public class HourAxis : IAxis
        {
            private double[] _axisValues;
            private AxisType _axisType;

            //constructor
            public HourAxis()
            {
                _axisType = AxisType.Hour;
                setAxisValues();
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

            //methods
            public int ReturnAxisValue(DateTime inputval)
            {
                return inputval.Hour;
            }
            private void setAxisValues()
            {
                try
                {
                    double[] result = null;
                    result = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
                    _axisValues = result;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    
}
