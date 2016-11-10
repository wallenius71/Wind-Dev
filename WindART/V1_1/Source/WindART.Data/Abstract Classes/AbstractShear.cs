using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractAlpha
    {
        protected IAxis _xAxis;
        protected IAxis _yAxis;
        protected AbstractAlphaCalculation _calcMethod;

        public bool UpperAvgCalculated { get; private set; }
        public bool LowerAvgCalculated { get; private set; }
        public bool AlphaCalculated { get; private set; }
        public bool ShouldRecalculate { get; set; }
        public ShearCalculationGridCollection GridCollection { get; set; }
        

        public IAxis Xaxis
        {
            get { return _xAxis; }
            set { _xAxis = value; }
        }
        public IAxis Yaxis
        {
            get { return _yAxis; }
            set { _yAxis = value; }
        }

        public virtual void CalculateAlpha(AlphaCalculationMethod calcType)
        {

        }

    }
}
