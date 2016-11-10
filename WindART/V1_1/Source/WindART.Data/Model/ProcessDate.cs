using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace WindART
{
    public class ProcessDate:IProcessDateTime 
    {
        private DataTable _data;

        public ProcessDate(DataTable data)
        {
            _data = data;
        }
        public DataSetDateStatus FindDateTimeColumn(out int dateindex)
        {
            //search columns
            //find all datetypes and set datasetstatus

            try
            {

                DataSetDateStatus _dateStatus = DataSetDateStatus.NotSet;
                int DateCount = 0;
                int thisIndex = 0;
                dateindex = 0;
                foreach (DataColumn dc in _data.Columns)
                {
                    
                    if (dc.DataType == typeof(DateTime))
                    {
                        thisIndex = dc.Ordinal;
                        DateCount ++;
                        
                    }
                }
                
                switch (DateCount)
                {
                    case 0:
                        _dateStatus = DataSetDateStatus.FoundNone;
                        break;
                    case 1:
                        dateindex = thisIndex;
                        _dateStatus = DataSetDateStatus.Found;
                        break;
                    case 2:
                        _dateStatus = DataSetDateStatus.FoundMultiple;
                        break;
                    default:
                        _dateStatus = DataSetDateStatus.NotSet;
                        break;

                }
              return _dateStatus; 
            }
            
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool IsDateTime(int dateindex)
        {
            //rely on ado.net fill method to determine datatype 
            //works better than parsing columns manually
            try
            {
                return _data.Columns[dateindex].DataType == typeof(DateTime);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<DateTime> BuildDateTimeList(IDateOrder dateorder)
        {
            
            //depends on IDateOrder object parameters provided elsewhere 
            //the datepart  column indexes will need to be provided by the user at run time
            try
            {
                
                Type type = dateorder .GetType ();
                PropertyInfo [] properties = type.GetProperties();

                List<DateOrderIndexValue> dateidx = new List<DateOrderIndexValue>();
                foreach (PropertyInfo property in properties)
                {
                    //Console.WriteLine(property.GetValue(dateorder,null).GetType ());
                    dateidx.Add(((DateOrderIndexValue ) property.GetValue(dateorder, null)));
                }

                Type datetype = typeof(DateOrderIndexValue);
                                
                Dictionary<string, string> values = new Dictionary<string, string>();
                Dictionary<string, int> colindexes = new Dictionary<string, int>();
                
                List<DateTime> dateList = new List<DateTime>();
                foreach (DataRow row in _data.Rows)
                {
                    //populate dictionary of values and indexes
                    foreach (DateOrderIndexValue property in dateidx)
                    {   
                        //Console.WriteLine(property.index);
                        property.value = row[property.index].ToString();
                    }

                  DateTime resultdate=  dateorder.AssembleDate();
                  dateList.Add(resultdate);
                }
                return dateList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
    }
}
