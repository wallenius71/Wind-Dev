using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace WindART
{
    public class WDSummaryGrid:AbstractSummaryGrid 
    {
        public WDSummaryGrid(ISessionColumnCollection collection, DataTable data, SessionColumnType sheartype)
        {
            _collection = collection;
            _rowdata = _data.AsDataView();
            setCols();
            FilterData();

        }

        public override void CreateGrid()
        {
            
        }
        
    }
}
