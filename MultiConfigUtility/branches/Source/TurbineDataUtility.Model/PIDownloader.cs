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

                string cmd=string.Format (String.Format("SELECT [time], [value] FROM piarchive..piavg WHERE tag =('{0}') AND [time] BETWEEN '{1}-{2}-{3} {4}' AND '{5}-{6}-{7} {8}' AND timestep='10m' ORDER BY  [time]",tag ,workstart.Year, workstart.Month, workstart.Day, workstart.ToLongTimeString(), workend.Year,workend.Month, workend.Day, workend.ToLongTimeString()));
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
