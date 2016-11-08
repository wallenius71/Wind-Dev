using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public  class DuplicateTimeStamps:IDuplicateTimeStamps 
    {
        private DataTable _data;
        int _dateindex;

        //constructor
        public DuplicateTimeStamps(DataTable data,int dateindex)
        {
            _data = data;
            _dateindex = dateindex;
        }
        //methods
        public DataView GetDuplicateDateView()
        {
            try
            {//build duplicate date list
                List  <DateTime> worklist = new List<DateTime>();
                List <DateTime> duplicates = new List<DateTime>();
                //sort data
                DataView view=_data.AsDataView ();
                view.Sort=Settings.Default .TimeStampName + " asc" ;
                
                
                for(int i=0;i<view.Count;i++)
                {
                    DateTime thisDate=(DateTime)view[i][_dateindex];
                    int j=worklist.BinarySearch (thisDate);
                    if (j > 0)
                    {
                        duplicates.Add(thisDate);
                    }
                    else
                    {
                        worklist.Add(thisDate);
                    }

                }
                //use duplicate list to find all rows with duplicate timestamps  in dataset

                List<DateTime> searchList = duplicates.AsEnumerable().Distinct().ToList ();
                var dupRows = from d in _data.AsEnumerable()
                             where searchList.Contains((DateTime)d[_dateindex])
                             select d;

                DataView result = dupRows.AsDataView();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DateTime> GetDistinctDuplicates()
        {
            try
            {//build duplicate date list
                List<DateTime> worklist = new List<DateTime>();
                List<DateTime> duplicates = new List<DateTime>();
                //sort data
                DataView view = _data.AsDataView();
                view.Sort = Settings.Default.TimeStampName + " asc";


                for (int i = 0; i < view.Count; i++)
                {
                    DateTime thisDate = (DateTime)view[i][_dateindex];
                    int j = worklist.BinarySearch(thisDate);
                    if (j > 0)
                    {
                        duplicates.Add(thisDate);
                    }
                    else
                    {
                        worklist.Add(thisDate);
                    }

                }
                
                List<DateTime> searchList = duplicates.AsEnumerable ().Distinct().ToList ();
                return searchList;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        
    }
}
