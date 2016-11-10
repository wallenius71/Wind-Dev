using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    [Serializable ]
    public class SensorConfig:ISensorConfig 
    {
        DateTime _startDate;
        DateTime _endDate;
        double _height;
        string _heightUnits;
        double _orientation;
        string _orientationUnits;
        double _multiplier;
        double _offset;

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }

        }
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        public string HeightUnits
        {
            get
            {
               return  _heightUnits;
            }
            set
            {
                _heightUnits=value;
            }
        }
        public double Orientation
        {
            get
            {
               return _orientation;
            }
            set
            {
                if (value < 0 || value > 360)
                {
                    throw new Exception("Invalid Orientation");
                }
                _orientation=value;
            }
        }
        public string OrientationUnits
        {
            get
            {
                return _orientationUnits;
            }
            set
            {
                _orientationUnits =value;
            }
        }
        public double Multiplier
        {
            get
            {
                return _multiplier;
            }
            set
            {
                _multiplier =value;
            }
        }
        public double Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset=value;
            }
        }
       
    }
}
