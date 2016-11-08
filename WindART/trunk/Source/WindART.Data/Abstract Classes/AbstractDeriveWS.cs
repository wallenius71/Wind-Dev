using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public abstract class AbstractDeriveWS
    {
        public abstract List<double> DeriveNewWS(ISessionColumnCollection collection, AbstractAlpha alpha, double[,] sourceWS, DataTable data, double derivedHt);
        public abstract List<double> DeriveNewWS(ISessionColumnCollection collection, AbstractAlpha alpha, double[,] sourceWS, Dictionary<string,List<double>> axisSourceData, double derivedHt);

    }
}
