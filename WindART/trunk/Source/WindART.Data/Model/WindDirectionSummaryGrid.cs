using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace WindART
{
    public class WindDirectionSummaryGrid:AbstractSummaryGrid 
    {
        public WindDirectionSummaryGrid(ISessionColumnCollection collection, 
            DataTable data, SessionColumnType sheartype,WindDirectionAxis  axis)
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
