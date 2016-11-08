﻿using System;
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
            public double BinWidth
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
            public int SeessionColIndex { get; set; }

            //methods
            public double GetAxisValue(object inputval)
            {
                if (inputval is DateTime)
                {
                    DateTime result = (DateTime)inputval;
                    return result.Hour;
                }
                else if (inputval is int)
                {
                    return _axisValues[(int)inputval];
                }
                else return default(double);

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


            public int SessionColIndex { get; set; }
            

            public double GetRange(int element, Range startorend)
            {
                throw new NotImplementedException();
            }
        }
    
}
