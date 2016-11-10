using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class RowFilterFactory
    {
        public AbstractRowFilter CreateRowFilter(IAxis xAxis,IAxis yAxis, List<DataRow> data)
        {
            return new RowFilter(xAxis,yAxis,data);
        }
        public AbstractRowFilter CreateRowFilter( List<DataRow> data)
        {
            return new RowFilter( data);
        }
    }
}
