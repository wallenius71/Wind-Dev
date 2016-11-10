using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class XbyYCoincidentRow
    {
        DateTime _date;
        double _upperws;
        double _lowerws;
        double _wd;
        double _shear;

        public DateTime Date 
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
        public double UpperWS
        {
            get
            {
                return _upperws;
            }
            set
            {
                _upperws = value;
            }
        }
        public double LowerWS
        {
            get
            {
                return _lowerws;
            }
            set
            {
                _lowerws = value;
            }
        }
        public double WD
        {
            get
            {
                return _wd;
            }
            set
            {
                _wd = value;
            }
        }
        public double Shear
        {
            get
            {
                return _shear;
            }
            set
            {
                _shear = value;
            }
        }

    }
}
