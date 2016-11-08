using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface ISensorConfig
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        Double Height{ get; set; }
        string HeightUnits { get; set; }
        double Orientation { get; set; }
        string OrientationUnits { get; set; }
        double Multiplier { get; set; }
        double Offset { get; set; }
        
    }
}
