using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class BuildJulianDate:IDateOrder 
    {
        
        //fields
        private DateOrderIndexValue _hoursminutes = new DateOrderIndexValue();
        private DateOrderIndexValue _year = new DateOrderIndexValue();
        private DateOrderIndexValue _doy = new DateOrderIndexValue();
       
        //properties
        //TODO: add validation to property set
        public DateOrderIndexValue hoursminutes
        {
            get
            {
                return _hoursminutes;
            }
            set
            {
                _hoursminutes = value;
            }
        }
        public DateOrderIndexValue year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
            }
        }
        public DateOrderIndexValue doy
        {
            get
            {
                return _doy;
            }
            set
            {
                _doy = value;
            }
        }

        //methods
        public DateTime AssembleDate()
        {
            try
            {   
                
                if (string.IsNullOrEmpty (_year.value) || string.IsNullOrEmpty (_hoursminutes.value  ) 
                    || string.IsNullOrEmpty (_doy.value))
                { throw new Exception("Not all parts of julian date were provided. can not construct date value "); }

                DateTime dtOut = new DateTime(int.Parse(_year.value ), 1, 1);
                dtOut = dtOut.AddDays(int.Parse(_doy.value) - 1);

                switch (_hoursminutes.value.Length)
                {
                    case 2:
                        dtOut = dtOut.AddMinutes(double.Parse(_hoursminutes.value));
                        break;
                    case 3:
                        dtOut = dtOut.AddHours(double.Parse(_hoursminutes.value.Substring(0, 1)));
                        dtOut = dtOut.AddMinutes(double.Parse(_hoursminutes.value.Substring(1, 2)));
                        break;
                    case 4:
                        dtOut = dtOut.AddHours(double.Parse(_hoursminutes.value.Substring(0, 2)));
                        dtOut = dtOut.AddMinutes(double.Parse(_hoursminutes.value.Substring(2, 2)));
                        break;
                }

                return dtOut;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
 
    }
}
