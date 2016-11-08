using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART.DAL ;
using System.Data;

namespace TurbineDataUtility.Model
{
    public class PIDownloader:IDisposable 
    {
        
        private  DataRepository _repository;
       
        
        public PIDownloader(DataSourceType selectedDataSource)
        {
            //since there will only be two choices on the view we don't need to worry about 
            //figuring out if there is a password and uid being passed. it will  be a predefined datasource 
            //(for now)
            _repository = new DataRepository(selectedDataSource);

        }
        public PIDownloader()
        {
        }

        public SortedDictionary<DateTime,double> Download(string tag, DateTime start, DateTime end)
        {
            //if the date range is longer than 2 months divide the download into two month chunks 
            SortedDictionary <DateTime ,double> PiResultSet = new SortedDictionary<DateTime ,double>();
            DateTime workstart = start;
            DateTime workend = end;

            foreach(DateTime[] dt in Utils.DividedDateRange (start, end))
            {
                workstart = dt[0];
                workend = dt[1];

                string cmd;
               
                    cmd= string.Format(String.Format("SELECT [time], [value] FROM piarchive..piavg WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
               
                DataTable pidata= _repository.GetData(cmd);

                foreach (DataRow row in pidata.Rows)
                {
                    if(!PiResultSet .ContainsKey ((DateTime)row["time"]))
                    {
                        double outval=0.0;
                        if (!double.TryParse(row["value"].ToString(),out outval))
                        {
                            outval = -9999.0; 
                        }
                        PiResultSet.Add((DateTime)row["time"], outval);
                    }
                }
                pidata = null;

            }
            
            return PiResultSet;
        
        }

        public Dictionary<string, string> Download(string tag, DateTime start, DateTime end, bool compressed)
        {
            //if the date range is longer than 2 months divide the download into two month chunks 
            Dictionary<string, string > PiResultSet = new Dictionary<string, string>();
            DateTime workstart = start;
            DateTime workend = end;

            foreach (DateTime[] dt in Utils.DividedDateRange(start, end))
            {
                workstart = dt[0];
                workend = dt[1];

                string cmd;
                if (!compressed)
                    cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..piavg WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                else
                {
                    if (tag.Contains("Normalized.State") || tag.Contains("GCU1.State") || tag.Contains("GCU2.State")
                        || tag.Contains("Turbine.State")|| tag.Contains("Yaw.State")
                        || tag.Contains(".Desc") || tag.Contains(".desc")
                        || tag.Contains("Turbine.Status") & !tag.Contains ("Turbine.Status.Code") || tag.Contains ("Curtailment.Status")
                        || tag.Contains(".Curtail.Status")||tag.Contains("Majeure.Status") || tag.Contains("Fault.Status")
                        || tag.Contains("Maintenance.Status") || tag.Contains("Key.Status") || tag.Contains("Warning.Status")
                        || tag.Contains(".Available")
                        || tag.Contains(".Alarm")
                        || tag.Contains(".Clock")
                        || tag.Contains(".Supervisory.Control")
                        || tag.Contains(".Relay.Enabled")
                        || tag.Contains(".Communication.Fail")
                        || tag.Contains(".Com.Fail")
                        || tag.Contains(".Spare.")
                        || tag.Contains(".Transparent")
                        )
                    cmd = string.Format(String.Format("SELECT [time], DIGSTRING(status) [value] FROM piarchive..picomp WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                    else
                    cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..picomp2 WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                }
                DataTable pidata = _repository.GetData(cmd);

                foreach (DataRow row in pidata.Rows)
                {
                    if (!PiResultSet.ContainsKey(row["time"].ToString ()))
                    {
                        PiResultSet.Add(row["time"].ToString(), row["value"].ToString ().Replace (',',' '));
                    }
                }
                pidata = null;

            }

            return PiResultSet;

        }
        

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
