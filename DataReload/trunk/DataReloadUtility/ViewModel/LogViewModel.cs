using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel ;
using WindART.DAL;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace DataReloadUtility
{
    public class LogViewModel:LogListBase 
    {
        
        public LogViewModel(IDataRepository repository)
        {
            _repository = repository;
            _displayName = "Log";
        }
        
        public void StartLog()
        {

            BackgroundWorker worker = new BackgroundWorker();
            string sql = "select distinct top 100 * from log order by logid desc";
            
            
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                   
                    do
                    {
                        try
                        {
                            

                            DataTable data = _repository.GetData(sql);
                            Queue<LogItem> templist = new Queue<LogItem>();

                                if (data != null)
                                {
                                   
                                    foreach (DataRow dr in data.Rows)
                                    {
                                        LogItem val = new LogItem();
                                        val.Date = dr["time"].ToString();
                                        val.Process = dr["Process"].ToString();
                                        val.Message = dr["Message"].ToString();

                                            
                                    
                                        Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            //get first element
                                            if (templist.Count()>100)
                                            {

                                               templist.Dequeue();
                                            }
                                           
                                            
                                            templist.Enqueue (val);
                                            LogList.Clear();
                                            foreach (LogItem l in templist)
                                            {
                                                LogList.Enqueue(l);
                                            }
                                                                 
                                           
                                        }), DispatcherPriority.Normal );
                                       
                                     }   

                                    

                                    


                                    Thread.Sleep(3000);
                                    //Console.WriteLine("loaded " + LogList.Count + " records");
                                }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine ("error downloading log " + e.Message );
                        }
                    
                    }
                    while (0 == 0);

                };
            worker.RunWorkerAsync();
        }
    }
}
