using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IWindSpeedConfig:ISensorConfig 
    {

        Sector GoodSector
        { get; }
        Sector ShadowSector
        { get; }

        void ExtendGoodRangeStart(double extendVal);
        void ExtendGoodRangeEnd(double extendVal);
        InSector BelongsToSector(double WD);
        InSector BelongsToSector(double WD,string boundary,double extendamnt);
    }
}
