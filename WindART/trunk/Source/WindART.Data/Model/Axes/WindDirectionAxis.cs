using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class WindDirectionAxis : IAxis
    {

        private AxisType _axisType;
        private double _binWidth;
        private double[] _axisValues;

        //constructor
        public WindDirectionAxis(double binwidth)
        {
            _axisType = AxisType.WD;
            if (360 % _binWidth > 0)
            {
                throw new ApplicationException ("Bin width not valid because it would cause a partial bin to be created");

            }
            else
            {
                _binWidth = binwidth;
            }
            setAxisValues();
        }

        //properties 
        public double BinWidth
        {
            get
            {
                return _binWidth;
            }
            set
            {
                if (360 % value > 0)
                {
                    throw new Exception("Bin width not valid because it would cause a partial bin to be created");

                }
                else
                {
                    _binWidth = value;
                }
                setAxisValues();
            }
        }
        public AxisType AxisType
        {
            get { return _axisType; }
        }
        public double Incrementor
        {
            get { return _binWidth; }
        }
        public double[] AxisValues
        {
            get
            {
               return  _axisValues;
            }
        }
        public int SessionColIndex { get; set; }

        //methods 
       
        private void setAxisValues()
        {
            
                if (360 % _binWidth > 0)
                {
                    throw new ApplicationException("Bin width not valid because it would cause a partial bin to be created");
                }
                if ( _binWidth <= 0)
                {
                    throw new ApplicationException("Bin width must be greater than 0");
                }
                double[] result;

                int numVals = Convert.ToInt16(360 / _binWidth);

                result = new double[numVals];
                double y = 0;
                for (int i = 0; i < numVals; i++)
                {
                    result[i] = y;
                    y += _binWidth;
                }



                _axisValues = result;
            


        }
        
        public double GetRange(int element,Range startorend)
        {
            if (startorend == Range.start)
            {
                //return the start range of the interval centered bin
                double result = 0;
                if (_axisValues[element] - (BinWidth / 2) < 0)
                {
                    result = 360 - (BinWidth / 2);
                }
                else
                {
                    result = _axisValues[element] - (BinWidth / 2);
                }
                return result;
            }
            else
            {
                //return the start range of the interval centered bin
                double result = 0;
                if (_axisValues[element] + (BinWidth / 2) > 360)
                {
                    result = 0 + (BinWidth / 2);
                }
                else
                {
                    result = _axisValues[element] + (BinWidth / 2);
                }
                return result;
            }
            
            
        }
        public double GetAxisValue(object element)
        {
            //Console.WriteLine(element.ToString ());
            if ((double)element >= (double)(360.0-(BinWidth/2.0)) ) return 0;

            //if ((double)element / BinWidth == 2.5)
            //{
            //    Console.WriteLine(Math.Round((double)element / BinWidth,MidpointRounding.AwayFromZero ));

            //}
            return Math.Round((double)element / BinWidth,MidpointRounding.AwayFromZero );
        }
    }

}
