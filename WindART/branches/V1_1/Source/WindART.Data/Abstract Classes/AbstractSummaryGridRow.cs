using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public abstract class AbstractSummaryGridRow
    {

        public virtual int Count { get; set; }
        public virtual double Frequency { get; set; }
        public virtual double UpperWSAvg { get; set; }
        public virtual double LowerWSAvg { get; set; }
        public virtual double ShearedWSAvg { get; set; }
        public virtual double Alpha { get; set; }

    }
}
