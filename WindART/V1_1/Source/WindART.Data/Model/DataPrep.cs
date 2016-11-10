using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties ;

namespace WindART
{
    public class DataPrep:IFillMissingDate 
    {
        private DataTable _data;
        private int _dateIndex;

        public DataPrep(DataTable data,int dateindex)
        {
            _data = data;
            _dateIndex = dateindex;
        }
        public bool FillMissingdates(List<DateTime> missing)
        {
            try
            {
                DataRow templateRow = _data.NewRow();
                foreach (DataColumn dc in _data.Columns)
                {
                    if (dc.Ordinal != _dateIndex)
                    {
                        templateRow[dc.Ordinal] = Settings.Default.MissingValue;
                    }

                }
                lock (_data)
                {
                    _data.BeginLoadData();
                    foreach (DateTime date in missing)
                    {
                        DataRow newRow = _data.NewRow();
                        newRow.ItemArray = templateRow.ItemArray;
                        newRow[_dateIndex] = date;
                        _data.Rows.Add(newRow);
                    }

                    _data.EndLoadData();
                }

                return true;
            }
            catch (OutOfMemoryException)
            {
                throw new ApplicationException("The file is too large to be processed. There is not enough memory available");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
    }
}
