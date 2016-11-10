using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class HeightConfig:IConfig 
    {

        

        DateTime _startDate;
        DateTime _endDate;
        Dictionary<double, IList<ISessionColumn>> _columns = new Dictionary<double, IList<ISessionColumn>>();

        public HeightConfig()
        { }
        public DateTime StartDate
        {
            get { return _startDate;}
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            get {return _endDate;}
            set { _endDate = value; }
        }

        public Dictionary<double,IList<ISessionColumn>> Columns
        {
            //Columns held here are guarnateed to belong to the same config according to height 
            get { return _columns; }
            set { _columns = value; }
        }

       
    }
}
