using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;
using System.Windows.Forms;

namespace WindART
{
    public class DateTimeSequence:IDateTimeSequence 
    {
        private DataTable _data;
        private int _dateIndex;
        public  DateTimeSequence(DataTable data,int dateindex)
        {
            _data = data;
            _dateIndex = dateindex;
        }
        
        //use to work with an existing datatable timestamp 

        public virtual TimeSpan DetectInterval()
        {
            //determines the interval by selecting the interval with the greatest occurrence
            try
            {
                DataView view = _data.AsDataView();
                view.Sort = Settings.Default.TimeStampName + " asc";
                //count each interval occurence
                TimeSpan gap=default(TimeSpan);
                DateTime prevVal=default(DateTime);
                DateTime curVal=default(DateTime);
                DataTableReader reader = new DataTableReader(_data);
                Dictionary<TimeSpan, int> resultCount = new Dictionary<TimeSpan, int>();

                reader.NextResult();
                //create table of counts grouped by gap
                while (reader.Read())
                {
                    curVal = (DateTime)reader[_dateIndex];
                    if (prevVal != default(DateTime))
                    {
                        gap = curVal - prevVal;
                        if (resultCount.ContainsKey(gap))
                        {
                            resultCount[gap] += 1;
                        }
                        else
                        {
                            resultCount.Add(gap, 1);
                        }
                    }
                    prevVal = curVal;
                }
                
                //determine the gap with the greatest count
                int workval=0;
                int maxval=0;
                TimeSpan greatest=default (TimeSpan );
                foreach (KeyValuePair<TimeSpan, int> kv in resultCount)
                {
                    
                    workval=kv.Value ;
                    if (workval > maxval)
                    {
                        maxval = workval;
                        greatest = kv.Key;
                    }
                }
                return greatest;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual List<DateTime> GetMissingTimeStamps()
        {
            try
            {
               
                List<DateTime> existing = GetExistingSequence ();
                List<DateTime> expected = GetExpectedSequence ();
                //MessageBox.Show(existing.Last().ToString());
                //MessageBox.Show(expected.Last().ToString());
                
                var result = expected.Except(existing);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual List<DateTime> GetExistingSequence()
        {
            
                DataView view = new DataView(_data);
                view.Sort = Settings.Default.TimeStampName + " asc";
                List<DateTime> result = new List<DateTime>();
                DateTime thisDate=default(DateTime);
                foreach (DataRowView rowview in view)
                {
                    if (DateTime.TryParse (rowview[_dateIndex].ToString(),out thisDate))
                    {
                    result.Add(thisDate);
                    }
                    else
                        result.Add(default(DateTime));
                }
                return result;
            
        }
        public virtual List<DateTime> GetExpectedSequence()
        {
           
                TimeSpan interval = DetectInterval();
                DateTime first = default(DateTime);
                DateTime last = default(DateTime);
                DataView view = new DataView(_data);
                List<DateTime> result = new List<DateTime>();
                view.Sort = Settings.Default.TimeStampName + " asc";

                for (int i = 0; i < view.Count; i++)
                {
                    if (DateTime.TryParse(view[i][_dateIndex].ToString (), out first))
                    { break; }
                }
                last = (DateTime)view[view.Count - 1][_dateIndex];

                DateTime workdate = first;
                result.Add(workdate);
                while (workdate < last)
                {
                    workdate = workdate + interval;
                    result.Add(workdate);
                }
                return result;
            
        }


    }
}
