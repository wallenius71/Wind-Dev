using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class AlphaFactory
    {
        public AbstractAlpha CreateAlpha(DataTable data, ISessionColumn upperWS, ISessionColumn lowerWS, IAxis xAxis, IAxis yAxis)
        {
            return new Alpha(data, upperWS, lowerWS, xAxis, yAxis);
        }

        public AbstractAlpha CreateAlpha(List<DataRow> data, ISessionColumn upperWS, ISessionColumn lowerWS, IAxis xAxis, IAxis yAxis)
        {
            return new Alpha(data, upperWS, lowerWS, xAxis, yAxis);
        }
    }
}
