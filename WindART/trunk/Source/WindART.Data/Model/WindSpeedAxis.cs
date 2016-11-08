using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class WindSpeedAxis:AbstractAxis 
    {
        
        private DataView _data;
        private int _wsindex;
        private double _binWidth;

        public WindSpeedAxis(double binwidth, DataView data,int wsindex)
        {
            _axisType = AxisType.WS;
            _data = data;
            _wsindex = wsindex;
            _binWidth = binwidth;
            
            setAxisValues();
        }
        protected override void setAxisValues()
        {
            //get the max ws val

            //create the bins 
            
            int numVals = Convert.ToInt16(Math.Truncate((GetMaxWS () / _binWidth)))+1;
            Console.WriteLine("array length will be " + numVals);
            double[] result = new double[numVals];
            double y = 0;
            for (int i = 0; i < numVals; i++)
            {
                result[i] = y;
                y += _binWidth;
            }

            _axisValues = result;
        }
        protected virtual double GetMaxWS()
        {
            List<double> workvals = new List<double>(_data.Count);
            foreach (DataRowView row in _data)
            {
                workvals.Add((double)row[_wsindex]);
            }
            Console.WriteLine("max ws=" + workvals.Max());
            return workvals.Max()+1;
        }
        public virtual double GetRangeStart(int element)
        {
            try
            {
                //return the start range of the interval centered bin
                double result = 0;
                if (_axisValues[element] - (_binWidth  / 2) < 0)
                {
                    result = 0;
                }
                else
                {
                    result = _axisValues[element] - (_binWidth  / 2);
                }
                return result;
            }
            catch
            {
                throw;
            }

        }
        public virtual double GetRangeEnd(int element)
        {
            
                //return the start range of the interval centered bin
                double  result = _axisValues[element] + (_binWidth / 2);
                return result;
            

        }


        public override double Incrementor
        {
            get {return  _binWidth; }
        }
    }
}
