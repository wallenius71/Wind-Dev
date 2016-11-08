using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsApplication1
{
    public class JaggedData
    {
        string _folder;
        public List<string> PRNs { get; set; }

        public void CalculateAndOutput(string folder,int secondInterval, Utils.aggregate aggType )
        {
            DateTime workDT=default(DateTime);
            double workDBL=default(double);
            int idx=default(int);
            //folder = @"P:\JesseRichards\Operations\Optimization\Fault Detection\Goshen\EventCodes\work";
            StreamWriter writer = new StreamWriter(folder +"\\Aggregated.csv", false);
            

            //find all files in the selected directory 
            foreach (string s in Directory.EnumerateFiles(folder, "*.csv"))
            {

                if (File.Exists(s) & Path.GetFileName(s) != "Aggregated.csv")
                {
                    StreamReader readerRawValues = new StreamReader(s);
                    Dictionary<string, Dictionary<DateTime, double>> values = new Dictionary<string, Dictionary<DateTime, double>>();
                    Dictionary<string, Dictionary<DateTime, double>> AvgValues = new Dictionary<string, Dictionary<DateTime, double>>();
                    List<string> line = new List<string>();
                    
                    string filename = Path.GetFileName(s);
                   
                    bool firstline=true;
                    while (!readerRawValues.EndOfStream)
                    {


                        if (firstline)
                        {
                            //put the headers into the dictionary 
                            line = (readerRawValues.ReadLine().Split(',').ToList());
                            line.Where(c => c.Length > 0 & c !="DateTime").ToList().ForEach(c => values.Add(c, new Dictionary<DateTime, double>()));
                            line.Where(c => c.Length > 0 & c != "DateTime").ToList().ForEach(c => AvgValues.Add(c, new Dictionary<DateTime, double>()));
                            firstline = false;
                        }
                        else
                        {
                            //load data for each column 
                            line = (readerRawValues.ReadLine().Split(',').ToList());
                           
                            try
                            {
                                //put each tags raw dates and values into a dictionary 
                                foreach (string cell in line.Where(c=>(line.IndexOf(c) % 2) != 0).ToList())
                                {
                                    if (DateTime.TryParse(line[line.IndexOf(cell) - 1], out workDT) & Double.TryParse(line[line.IndexOf(cell)],out workDBL ))
                                        {
                                         idx = (((line.IndexOf(cell) + 1) / 2) - 1);

                                       
                                           while (values.ElementAt(idx).Value.Keys.Contains(workDT)) workDT=workDT.AddMilliseconds  (1); 
                                           values.ElementAt(idx).Value.Add(workDT,workDBL);
                                        }
                                  }
                               
                                
                               
                               
                                                               
                            }

                            catch (Exception e)
                            {
                                values.ElementAt(idx).Value.Keys.ToList().ForEach(c => Debug.Print(c.ToString ()));
                                Debug.Print(idx.ToString());
                                Debug.Print(workDT.ToString());
                                Debug.Print(workDBL.ToString());
                                Debug.Print(e.Message);
                            }
                        }
                    }

                    //find the min and max dates for all data in the first dictionary
                    BoundaryDates bd = new BoundaryDates(values);
                    DateTime workStartTenMin = bd.TenMinStart;
                    DateTime workEndTenMin = bd.TenMinStart.AddSeconds(secondInterval);
                    List<double> workVals = new List<double>();
                    double thisVal = default(double);
                    double lastVal = default(double);
                    IAggregate agg = null;
                    switch (aggType)
                    {
                        case Utils.aggregate.VectorAvg:
                            agg = new VectorAverage(workVals);
                            break;
                        case Utils.aggregate.Min:
                            agg = new Minimum();
                            break;
                        case Utils.aggregate.Max:
                            agg = new Maximum();
                            break;
                        case Utils.aggregate.Avg:
                            agg = new Average();
                            break;
                        default:
                            agg = new Average();
                            break;


                    }
                    while (workEndTenMin <= bd.TenMinEnd)
                    {
                        //iterate each tag in source vals 
                        foreach (KeyValuePair<string, Dictionary<DateTime, double>> kv in values)
                        {
                            //get 10 min worth of vals 
                            workVals = kv.Value.Where(c => c.Key >= workStartTenMin & c.Key < workEndTenMin).Select(c => c.Value).ToList();
                            if (workVals.Count > 0) { lastVal = workVals.Last(); }

                            
                            //embedded factory
                           
                            //calculate and assign the aggregate value. if the value calculated is -9999 then take the last good value
                            thisVal = agg.Calculate(workVals);
                            if (thisVal == -9999) thisVal = lastVal;
                            AvgValues[kv.Key].Add(workStartTenMin, thisVal);

                        }
                            //iterate the BoundaryDates datetimes 
                            workStartTenMin = workStartTenMin.AddSeconds(secondInterval);
                            workEndTenMin = workEndTenMin.AddSeconds(secondInterval);
                            Debug.Print(workEndTenMin.ToString());
                        
                    }
                               

                    //column heads 
                    foreach (string k in AvgValues.Keys)
                    {
                        writer.Write("," + k);
                        
                    }
                    writer.Write(writer.NewLine);
                    //iterate each datetime
                    
                    foreach (DateTime dt in AvgValues.ElementAt(0).Value.Keys )
                    {
                        writer.Write(dt);
                        //iterate each tag and write valus 
                        foreach (KeyValuePair<string, Dictionary<DateTime, double>> kv in AvgValues)
                        {
                            writer.Write( "," + kv.Value[dt]);
                        }
                        writer.Write(writer.NewLine);
                    }
                     
                }
                else
                {
                    Console.WriteLine(s + "does not exist");
                }
                
               
            }
            writer.Close();
            MessageBox.Show("append header finished");
        }


    }

    public class BoundaryDates
    {
        private DateTime _start;
        private DateTime _end;
        private DateTime _tenMinStart;
        private DateTime _tenMinEnd;

        public DateTime Start
        {
            get
            { return _start; }
            set
            { 
                _start = value;
            //set even tenmin value 
                string startMinVal=_start.Minute.ToString();

                if (startMinVal.Length > 1)
                    startMinVal = startMinVal.Substring(0, 1) + "0";
                else
                    startMinVal = "00";
                _tenMinStart=DateTime.Parse(_start.Month.ToString()+"/"+ _start.Day.ToString() + "/" + _start.Year.ToString() + " " +  _start.Hour.ToString() + ":" +  startMinVal );

             }

        }
        public DateTime End
        { get { return _end; }
          set 
          { 
              _end = value;
              string endMinVal = ((_end.Minute % 10 == 0) ? _end.Minute.ToString() : ((_end.Minute).ToString().Substring(0, 1) + "0"));
              _tenMinEnd = DateTime.Parse(_end.Month.ToString() + "/" + _end.Day.ToString() + "/" + _end.Year.ToString() + " " + _end.Hour.ToString() + ":" + endMinVal).AddMinutes (10);
          }
        }
        public DateTime TenMinStart { get { return _tenMinStart; } set { _tenMinStart = value; } }
        public DateTime TenMinEnd { get { return _tenMinEnd; } set { _tenMinEnd = value; } }

        public  BoundaryDates(Dictionary<string, Dictionary<DateTime, double>> dates)
        {           

            List<DateTime> LocalMaxList = new List<DateTime>();
            List<DateTime> LocalMinList = new List<DateTime>();


            foreach (KeyValuePair<string, Dictionary<DateTime, Double>> kv in dates)
            {
                if (kv.Value.Keys.Count > 0)
                {
                    LocalMinList.Add(kv.Value.Keys.Min());
                    LocalMaxList.Add(kv.Value.Keys.Max());
                }
            }
            if (LocalMaxList.Count > 0 & LocalMinList.Count > 0)
            {
                End = LocalMaxList.Max();
                Start = LocalMinList.Min();
            }
        }

    }
}
