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

        public SortedDictionary<DateTime,double> Download(string tag, DateTime start, DateTime end, bool Std, bool Max, bool Min)
        {
            //if the date range is longer than 2 months divide the download into two month chunks 
            SortedDictionary <DateTime ,double> PiResultSet = new SortedDictionary<DateTime ,double>();
            DateTime workstart = start;
            DateTime workend = end;

            
               

                string cmd=string.Empty;

                if (Std)
                    cmd= string.Format(String.Format("SELECT [time], [value] FROM piarchive..pistd WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                if(Max)
                    cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..pimax WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                if (Min)
                    cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..pimin WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                if (!Std & !Max & !Min)
                    cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..piavg WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));

                 DataTable pidata = _repository.GetData(cmd);

                   
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

            
            
            return PiResultSet;
        
        }

        public Dictionary<string, string> Download(string tag, DateTime start, DateTime end, bool compressed, bool sampled,string Sampleinterval,
            bool Std, bool Max, bool Min)
        {
            //if the date range is longer than 2 months divide the download into two month chunks 
            Dictionary<string, string > PiResultSet = new Dictionary<string, string>();
            DateTime workstart = start;
            DateTime workend = end;

            
                workstart = start;
                workend = end;

                string cmd=string.Empty;

                if (!compressed & !sampled)
                {
                    if (!Std & !Max & !Min)
                    {
                        cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..piavg WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10s' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                    }
                    else
                    {//if an  aggregate is  selected 
                        if (Std)
                            cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..pistd WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                        if (Max)
                            cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..pimax WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                        if (Min)
                            cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..pimin WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));

                    }
                }
                else
                {
                    if (compressed)
                    {
                        if (   tag.Contains("Normalized.State")
                            || tag.Contains("GCU1.State")
                            || tag.Contains("GCU2.State")
                            || tag.Contains("Turbine.State")
                            || tag.Contains("Yaw.State")
                            || tag.Contains(".Desc")
                            || tag.Contains(".desc")
                            || tag.Contains("Turbine.Status")
                            || tag.Contains("Curtailment.Status")
                            || tag.Contains(".Curtail.Status")
                            || tag.Contains("Majeure.Status")
                            || tag.Contains("Fault.Status")
                            || tag.Contains("Maintenance.Status")
                            || tag.Contains("Key.Status")
                            || tag.Contains("Warning.Status")
                            || tag.Contains(".Available")
                            || tag.Contains(".Derate.Cause")
                            || tag.Contains("Comm.Status.Code")
                            || tag.Contains("Operational.Status.Code")
                            || tag.Contains("Operational.Sub.Code")
                            || tag.Contains("Operational.Status.Code")
                            || tag.Contains(".Flags")
                            || tag.Contains("Meter.Label")
                            || tag.Contains("Fault.Level")
                            || tag.Contains("Yaw.Mode")
                            || tag.Contains("Crew.Status")
                            || tag.Contains(".Status")
                            || tag.Contains(".Status.Code")
                            || tag.Contains(".out.of.Service")
                            || tag.Contains(".in.Programming")
                            || tag.Contains(".Cycle.State")
                            || tag.Contains(".Lockout.State")
                            || tag.Contains(".Reset.State")
                            || tag.Contains(".Loss.Voltage")
                            || tag.Contains(".Trip")
                            || tag.Contains(".TRIPLED")
                            || tag.Contains(".Zone1")
                            || tag.Contains(".Zone2")
                            || tag.Contains(".Zone3")
                            || tag.Contains(".ZONE")
                            || tag.Contains(".Blocked")
                            || tag.Contains(".5kv.EN")
                            || tag.Contains(".Breaker.Fail")
                            || tag.Contains(".Flash.Maintenance")
                            || tag.Contains(".Low.Ring.Charge")
                            || tag.Contains(".Gas.And.Motors")
                            || tag.Contains(".52AA1")
                            || tag.Contains(".52AA2")
                            || tag.Contains(".Com.CH.Failure")
                            || tag.Contains(".Received.IN301")
                            || tag.Contains(".Vdc.Monitor")
                            || tag.Contains(".PBILED")
                            || tag.Contains(".Alarm")
                            || tag.Contains(".Clock")
                            || tag.Contains(".Supervisory.Control")
                            || tag.Contains(".Relay.Enabled")
                            || tag.Contains(".Communication.Fail")
                            || tag.Contains(".Com.Fail")
                            || tag.Contains(".Spare.IN101")
                            || tag.Contains(".Transparent")
                            || tag.Contains(".Dropout")
                            || tag.Contains(".Failure.Initiate")
                            || tag.Contains(".Occurred")
                            || tag.Contains(".Of.Potential")
                            )
                            cmd = string.Format(String.Format("SELECT [time], DIGSTRING(status) [value] FROM piarchive..picomp WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                        else
                            cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..picomp2 WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString()));
                    }
                    else if (sampled)
                    {
                        string sampleRate = Sampleinterval ;
                        if (   tag.Contains("Normalized.State")
                            || tag.Contains("GCU1.State")
                            || tag.Contains("GCU2.State")
                            || tag.Contains("Turbine.State")
                            || tag.Contains("Yaw.State")
                            || tag.Contains(".Desc")
                            || tag.Contains(".desc")
                            || tag.Contains("Turbine.Status")
                            || tag.Contains("Curtailment.Status")
                            || tag.Contains(".Curtail.Status")
                            || tag.Contains("Majeure.Status")
                            || tag.Contains("Fault.Status")
                            || tag.Contains("Maintenance.Status")
                            || tag.Contains("Key.Status")
                            || tag.Contains("Warning.Status")
                            || tag.Contains(".Available")
                            || tag.Contains(".Derate.Cause")
                            || tag.Contains("Comm.Status.Code")
                            || tag.Contains("Operational.Status.Code")
                            || tag.Contains("Operational.Sub.Code")
                            || tag.Contains("Operational.Status.Code")
                            || tag.Contains(".Flags")
                            || tag.Contains("Meter.Label")
                            || tag.Contains("Fault.Level")
                            || tag.Contains("Yaw.Mode")
                            || tag.Contains("Crew.Status")
                            || tag.Contains(".Status")
                            || tag.Contains(".Status.Code")
                            || tag.Contains(".out.of.Service")
                            || tag.Contains(".in.Programming")
                            || tag.Contains(".Cycle.State")
                            || tag.Contains(".Lockout.State")
                            || tag.Contains(".Reset.State")
                            || tag.Contains(".Loss.Voltage")
                            || tag.Contains(".Trip")
                            || tag.Contains(".TRIPLED")
                            || tag.Contains(".Zone1")
                            || tag.Contains(".Zone2")
                            || tag.Contains(".Zone3")
                            || tag.Contains(".ZONE")
                            || tag.Contains(".Blocked")
                            || tag.Contains(".5kv.EN")
                            || tag.Contains(".Breaker.Fail")
                            || tag.Contains(".Flash.Maintenance")
                            || tag.Contains(".Low.Ring.Charge")
                            || tag.Contains(".Gas.And.Motors")
                            || tag.Contains(".52AA1")
                            || tag.Contains(".52AA2")
                            || tag.Contains(".Com.CH.Failure")
                            || tag.Contains(".Received.IN301")
                            || tag.Contains(".Vdc.Monitor")
                            || tag.Contains(".PBILED")
                            || tag.Contains(".Alarm")
                            || tag.Contains(".Clock")
                            || tag.Contains(".Supervisory.Control")
                            || tag.Contains(".Relay.Enabled")
                            || tag.Contains(".Communication.Fail")
                            || tag.Contains(".Com.Fail")
                            || tag.Contains(".Spare.IN101")
                            || tag.Contains(".Transparent")
                            || tag.Contains(".Dropout")
                            || tag.Contains(".Failure.Initiate")
                            || tag.Contains(".Occurred")
                            || tag.Contains(".Of.Potential")

                            )
                            cmd = string.Format(String.Format("SELECT [time], DIGSTRING(status) [value] FROM piarchive..piinterp WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' and timestep='{9}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString(), sampleRate));

                        else if (tag.Contains("Event.Code") || tag.Contains("State.Fault"))

                            cmd = string.Format(String.Format("SELECT [time], [value] as value  FROM piarchive..piinterp WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' and timestep='{9}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString(), sampleRate));
                        else
                            cmd = string.Format(String.Format("SELECT [time], [value] FROM piarchive..piinterp WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' and timestep='{9}' ORDER BY  [time]", tag, workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year, workend.Month, workend.Day, workend.ToLongTimeString(), sampleRate));
                    }
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

            

            return PiResultSet;

        }
        

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
