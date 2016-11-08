using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class MonthSummaryGrid:AbstractSummaryGrid 
    {
        
            public MonthSummaryGrid (ISessionColumnCollection collection, DataTable data, SessionColumnType sheartype
                , MonthAxis axis)
            {
                _collection = collection;
                _rowdata = data.AsDataView();
                setCols();
                FilterData();

            }

            public override void CreateGrid()
            {

            }

        }
    
}
