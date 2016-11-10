using System;
using System.Collections.Generic;
using System.Text;

namespace WindART
{
    [Serializable ]
    public struct Sector
    {
        //a degreee range
        private double _SectorStart;
        private double _SectorEnd;
        
        public double SectorStart
        {
            
            get
            {
                return _SectorStart;
            }
            set
            {

                _SectorStart = value;
               
                
            }

        }
        public double SectorEnd
        {
            get
            {
                return _SectorEnd;
            }
            set
            {
                _SectorEnd = value;
                
            }

        }

        
   }
}
