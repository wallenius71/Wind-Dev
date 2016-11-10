using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART.Properties;

namespace WindART
{
    [Serializable ]
    public class WindSpeedConfig:IWindSpeedConfig 
    {

        DateTime _startDate;
        DateTime _endDate;
        double _height;
        string _heightUnits;
        double _orientation;
        string _orientationUnits;
        double _multiplier;
        double _offset;
        Sector _goodSector;
        Sector _shadowSector;
        double ShadowInterval = Settings.Default.ShadowInterval;
        double IntervalBoundary = Settings.Default.IntervalBoundary;
        bool _goodRangeStartWasExtended;
        bool _goodRangeEndWasExtended;

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
                return _heightUnits;
            }
            set
            {
                _heightUnits = value;
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
                if (value != _orientation)
                {
                    if (value < 0 || value > 360)
                    {
                        throw new Exception("Invalid Orientation");
                    }
                    AssignSector(value);
                    _orientation = value;
                }

               
                
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
                _orientationUnits = value;
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
                _multiplier = value;
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
                _offset = value;
            }
        }
        public Sector GoodSector
        {
            get
            {
                return _goodSector;
            }
        }
        public Sector ShadowSector
        {
            get 
            {
                return _shadowSector;
            }
        }

        public void ExtendGoodRangeStart(double extendVal)
        {
            //if comp is run more than once be sure the range is only extended once
            if (_goodRangeStartWasExtended)
            {
                return;
            }
            double workVal = _goodSector.SectorStart;
            //Console.WriteLine("sector start before extend " + _goodSector .SectorStart );
            //Console.WriteLine("start + extend value " + (_goodSector.SectorStart - extendVal));
            //extends this sensor's good range start boundary by the param value
            //if subtracting the extend val crosses 0 then subtract from 360
            if (workVal - extendVal < 0  )
            {

                workVal = 360 - Math.Abs((workVal - extendVal));
            }
            else
            {
                workVal = workVal - extendVal;
            }

            
            _goodSector.SectorStart = workVal;

            //handle the shadow sector change
            if ((workVal - IntervalBoundary) < 0)
            {
                _shadowSector.SectorEnd = 360 - Math.Abs((workVal - IntervalBoundary));
            }

            else
            {
                _shadowSector .SectorEnd  = workVal - IntervalBoundary;
            }
            _goodRangeStartWasExtended = true;
        }
        public void ExtendGoodRangeEnd(double extendVal)
        {
            //if comp is run more than once be sure the range is only extended once
            if (_goodRangeEndWasExtended)
            {
                return;
            }
            double workVal = _goodSector.SectorEnd;
            //extends this sensor's good range start boundary by the param value
            //Good Interval end Point

            if (workVal + extendVal > 360)
            {
                workVal = ((workVal + extendVal) - 360);
            }
            else
            {
                workVal = workVal + extendVal;
            }

            _goodSector.SectorEnd = workVal;

            //handle the shadow sector change
            if ((workVal - IntervalBoundary) > 360)
            {
                _shadowSector.SectorStart  = ((workVal + IntervalBoundary)-360);
            }

            else
            {
                _shadowSector.SectorStart  = workVal + IntervalBoundary;
            }

            _goodRangeEndWasExtended = true;
        }
        public InSector BelongsToSector(double WD)
        {
            //figure out the distance from the orientation

            if ((this._goodSector.SectorStart + (2 * ShadowInterval)) >= 360)
            {
                if ((WD >= this._goodSector.SectorStart && WD <= 360) |
                    (WD >= 0 && WD <= this._goodSector.SectorEnd))
                {
                    return InSector.Not_Shadowed;
                }
                else
                {
                    return InSector.Shadowed;

                }
            }
            //doesn't cross 0
            if (WD >= this._goodSector.SectorStart &&
                WD <= this._goodSector.SectorEnd)
            {
                return InSector.Not_Shadowed;
            }
            else
            {
                return InSector.Shadowed;
            }
        }
        public InSector BelongsToSector(double WD, string boundary, double extendamnt)
        {
            try
            {
                //figure out the distance from the orientation
                //boundary='end' or 'start' which boundary to extend 
                //extend amnt is the amount to extend the good range

                //temporarily reset the good range
                InSector result;
                if (boundary == "end")
                {
                    _goodSector.SectorEnd += extendamnt;
                }
                else
                {
                    _goodSector.SectorStart += -extendamnt;
                }

                if ((this._goodSector.SectorStart + (2 * ShadowInterval)) >= 360)
                {
                    if ((WD >= this._goodSector.SectorStart && WD <= 360) |
                        (WD >= 0 && WD <= this._goodSector.SectorEnd))
                    {
                        result= InSector.Not_Shadowed;
                    }
                    else
                    {
                        result= InSector.Shadowed;

                    }
                }
                //doesn't cross 0
                if (WD >= this._goodSector.SectorStart &&
                    WD <= this._goodSector.SectorEnd)
                {
                    result= InSector.Not_Shadowed;
                }
                else
                {
                    result= InSector.Shadowed;
                }

                //return the good sector to the way it was found
                if (boundary == "end")
                {
                    _goodSector.SectorEnd += -extendamnt;
                }
                else
                {
                    _goodSector.SectorStart += +extendamnt;
                }
                return result;
            }
            catch
            {
                throw;
            }
        }
        private void AssignSector(double orientation)
        {
            try
            {
                
                //Good Interval Start Point
                if ((orientation - ShadowInterval)  < 0)
                {

                    _goodSector.SectorStart = 360 - Math.Abs((orientation - ShadowInterval) );
                }
                else
                {
                    _goodSector.SectorStart = (orientation - ShadowInterval) ;
                }
                //Good Interval end Point

                if ((orientation + ShadowInterval)  > 360)
                {
                    _goodSector.SectorEnd = ((orientation + ShadowInterval) - (360));
                }
                else
                {
                    _goodSector.SectorEnd = (orientation + ShadowInterval) ;
                }

                //shadowed interval Start + goodsect end + 001
                if (_goodSector.SectorStart - IntervalBoundary < 0)
                {
                    _shadowSector.SectorStart = 360 + IntervalBoundary;
                }
                else
                {
                    _shadowSector.SectorStart = _goodSector.SectorEnd + IntervalBoundary;
                }
                //Shadow Interval end Point

                if (_goodSector.SectorStart + IntervalBoundary > 360)
                {
                    _shadowSector.SectorEnd = ((_goodSector.SectorStart - IntervalBoundary) - 360);
                }
                else
                {
                    _shadowSector.SectorEnd = _goodSector.SectorStart - IntervalBoundary;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}
