using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public abstract class AbstractRowFilter
    {
        protected IAxis _xAxis;
        protected IAxis _yAxis;
        protected List<DataRow> _data;
        public abstract List<double> Filter(object xAxisElement, object yAxisElement, int xCol, int yCol, int retCol);
        //to create an alpha set on a date range less than the entire config 
        public abstract List<double> Filter(object xAxisElement, object yAxisElement, int xCol, int yCol, int retCol, DateTime start, DateTime end,int dateCol);
       //single axis element limit dates to other than full dataset
        public abstract List<double> Filter(object xAxisElement,  int xCol, DateTime start, DateTime end,int dateCol,int retCol);
        //filter only on dates
        public abstract List<double> Filter( DateTime start, DateTime end, int dateCol,  int retCol);


    }
}
