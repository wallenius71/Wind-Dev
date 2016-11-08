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
            _wsindex = wsindex;  //the column used to determine the max ws and calculate the axis values
            _binWidth = binwidth;
            
            setAxisValues();
        }
        public override double BinWidth
        {
            get { return _binWidth; }
            set { _binWidth = value; }
        }

        protected virtual void setAxisValues()
        {
            //get the max ws val

            //create the bins 
            if (_binWidth <= 0)
            {
                throw new ApplicationException("Bin width must be greater than 0");
            }
            int numVals = Convert.ToInt16(Math.Truncate(((GetMaxWS () + 4) / _binWidth)))+1;
            //Console.WriteLine("array length will be " + numVals);
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
                if (row[_wsindex].GetType() != typeof(DBNull))
                {

                    workvals.Add(Convert.ToDouble(row[_wsindex]));
                }
            }
            //Console.WriteLine("max ws=" + workvals.Max());
            return workvals.Max()+1;
        }

        public override double GetRange(int element,Range startorend)
        {
            if (startorend == Range.start)
            {
                //return the start range 
                return _axisValues[element];
               
            }
            else
            {
                double result = _axisValues[element] + (_binWidth);
                return result;
            }

        }
        
        public override  double GetAxisValue(object element)
        {
            int idx;

            if ((double)element < 0)
                return -9999.99;
            else
             idx= (int)Math.Truncate((double)element / _binWidth);



            return idx;
        }
        public override double Incrementor
        {
            get {return  _binWidth; }
        }

        public override int SessionColIndex { get; set; } //column to evaluate at run time 
        
    }
}
