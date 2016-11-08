using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindART.Properties;
using System.Data;
using System.IO;

namespace WindART
{
    [Serializable ]
    public enum DataSetDateStatus
    {
        NotSet=0,
        Found=1,
        FoundMultiple=2,
        FoundNone=3
    }

    [Serializable]
    public enum Range
    {
        start,
        end
    }
    public enum SessionColumnType
    {
        Select=0,
        WSAvg=70,
        WSMin=1,
        WSMax=2,
        WSStd=3,
        WSAvgBulkShear=4,
        WSAvgShear=5,
        WSAvgSingleAxisShear=6,
        WDAvg=10,
        WDMax=11,
        WDMin=12,
        WDStd=13,
        TempAvg=20,
        TempMin=21,
        TempMax=22,
        TempStd=23,
        BPAvg=30,
        BPMin=31,
        BPMax=32,
        BPStd=33,
        BVAvg=40,
        BVMin=41,
        BVMax=42,
        BVStd=43,
        RHAvg=50,
        RHMin=51,
        RHMax=52,
        RHStd=53,
        AirDenAvg=71,
        DateTime=60
        
    }
    public enum DatePartType
    {
        hour = 0,
        minute = 1,
        second = 2,
        hourminute = 3,
        julianday = 4,
        year = 5,
        month = 6,
        milliseconds = 7,
        monthdayyear = 8,
        hourminutesecond = 9
    }
    public enum InSector
    {
        //where a wd value falls in the sensors area 
        Not_Shadowed,
        Shadowed,
        Neither

    }
    public enum CircleDirection
    {
        Clockwise,
        CounterClockwise
    }
    public enum AxisType
    {
        None=0,
        Month=1,
        Hour=2,
        WD=3,
        WS=4, 
        MonthYear=5
    }
    public enum SummaryType
    {
        Month = 1,
        Hour = 2,
        WD = 3,
        WS = 4,
        WDRose=5,
        DataRecovery=6,
        XbyYShear=7,
        SingleAxisShear=8
        
    }
    public enum StationSummarySheetType
    {
        ShearGrid,
        MonthHourSheet,
        WD_WSSheet,
        WindRoseSheet,
        DataRecovery


    }
    public enum AlphaCalculationMethod
    {
        PowerLaw,
        LogLaw
    }
    public enum AlphaFilterMethod
    {
        Coincident,
        NonCoincident
    }

    public static  class Utils
    {
        public static string GetFolder()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();

            fd.Description = "Select Folder";
            DialogResult result = fd.ShowDialog();

            if (result == DialogResult.OK)
            {
                return fd.SelectedPath;
            }
            else
            {

                return "";
            }

        }
        public static string GetFile()
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Title = "Select File";
            fd.ShowDialog();
            return fd.FileName;
        }
        public static DataTable MakeDataTable<T>(T[,] array)
        {
            DataTable table = new DataTable();

            for (int i = 0; i <= array.GetUpperBound (1); i++) 
            { 
                table.Columns.Add(i.ToString(), typeof(T));
            }
 
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                DataRow dr = table.NewRow();
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    dr[j] = array[i,j];
                }
                table.Rows.Add(dr);
            }

            return table;
        }
        public static DataTable MakeDataTable<T>(T[] array)
        {
            DataTable table = new DataTable();

            table.Columns.Add(" ", typeof(T));

            
                
                for (int j = 0; j <= array.GetUpperBound(0); j++)
                {   
                    DataRow dr = table.NewRow();
                    
                    dr[0] = array[j];
                    table.Rows.Add(dr);
                }
                
           

            return table;
        }

        public static bool AddColtoDataTable<T>(string AddColName, List<T> valArray, DataTable data)
        {
            //add a column of type T to datatable and return true if successful

            try
            {
                //add the new column if it does not exist 

                if (!data.Columns.Contains(AddColName))
                {
                    DataColumn newCol = new DataColumn();
                    newCol.DataType = typeof(T);
                    newCol.ColumnName = AddColName;
                    data.Columns.Add(newCol);
                }
                //add new rows if none yet exist
                if (data.Rows.Count == 0)
                {
                    data.BeginLoadData ();
                    foreach (T val in valArray)
                    {
                        DataRow newRow = data.NewRow();
                        newRow[AddColName] = val;
                        data.Rows.Add(newRow);
                    }
                    data.EndLoadData ();
                }
                else  //AddColName to existing rows
                {
                    
                    data.BeginLoadData();
                    int i = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        if (i < valArray.Count)
                        {
                            row.BeginEdit();
                            row[AddColName] = valArray[i];
                            row.AcceptChanges();
                            i++;
                        }
                        else
                            break;
                    }
                    
                    data.EndLoadData();
                }
                
               }

                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                    return false;
                }
            return true;

        }
        public static List<T> ExtractDataTableColumn<T>(string ColName, DataTable ParamDT)
        {
            try
            {
                //return a list of values of type determined at run time 
                //collected from a the session datatable 
                List<T> list = new List<T>();
                //Console.WriteLine("DT rows=" + ParamDT.Rows.Count.ToString());
                int RowCount = ParamDT.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    T temp;
                    if (ParamDT.Rows[i][ColName].GetType().ToString() == "System.DBNull")
                    {
                        temp = default(T);
                    }
                    else
                    {
                        temp = (T)Convert.ChangeType(ParamDT.Rows[i][ColName].ToString(), typeof(T));
                    }
                    list.Add(temp);
                }
                //Console.WriteLine("Sensor Val Count=" + list.Count.ToString());
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public static List<T> ExtractDataTableColumn<T>(int ColIdx, DataTable ParamDT)
        {
            try
            {
                //return a list of values of type determined at run time 
                //collected from a the session datatable 
                List<T> list = new List<T>();
                //Console.WriteLine("DT rows=" + ParamDT.Rows.Count.ToString());
                //DataView paramDTView = ParamDT.AsDataView();
                //paramDTView.Sort = Properties.Settings.Default.TimeStampName;

                int RowCount = ParamDT.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    T temp;
                    if (ParamDT.Rows[i][ColIdx].GetType().ToString() == "System.DBNull")
                    {
                        temp = default(T);
                    }
                    else
                    {
                        temp = (T)Convert.ChangeType(ParamDT.Rows[i][ColIdx].ToString(), typeof(T));
                    }
                    list.Add(temp);
                }
                //Console.WriteLine("Sensor Val Count=" + list.Count.ToString());
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public static double SectorRange(double SectValA, double SectValB, CircleDirection Dir)
        {

            //returns the difference in degrees between 2 points on a circle
            //specify the direction 
            try
            {
                double retVal = 0;
                switch (Dir)
                {
                    case CircleDirection.Clockwise:

                        if (SectValB - SectValA < 0)
                        {
                            retVal = (360 - SectValA) + SectValB;
                        }

                        else
                        {
                            retVal = SectValB - SectValA;
                        }
                        break;

                    case CircleDirection.CounterClockwise:

                        if (SectValA - SectValB < 0)
                        {
                            retVal = (360 - SectValA) + SectValB;
                        }
                        else
                            retVal = SectValA - SectValB;

                        break;
                    default:
                        retVal = 0;
                        break;
                }


                return retVal;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                return 0;
            }



        }
        public static int TimeSpanMonths(DateTime startdate, DateTime enddate)
        {
            int months = (13 - startdate.Month) + (((enddate.Year-1) - (startdate.Year))  * 12) + (enddate.Month);
            return months;
        }
        public static List<DateTime> GetExpectedSequence(DateTime first, DateTime last, TimeSpan interval)
        {

            List<DateTime> result = new List<DateTime>();
            DateTime workdate = first;
            result.Add(workdate);
            while (workdate <= last)
            {
                workdate = workdate + interval;
                result.Add(workdate);
            }

            return result;


        }
        public static void OutputFile(DataTable data,string filename)
        {
            
            
                
                    //filename = filename + @"\TestOutput_" +   DateTime.Now.ToLongTimeString ().Replace(@"/", "") + ".csv";
                    
                    // Create the CSV file to which grid data will be exported.
                    StreamWriter sw = new StreamWriter(filename, false);

                    //column heads
                    foreach (DataColumn col in data.Columns)
                    {
                        sw.Write(col.ColumnName.Replace("#", "_"));
                        // Console.WriteLine(col.ColumnName.Replace("#", "_"));
                        if (data.Columns.IndexOf(col) < data.Columns.Count - 1)

                            sw.Write(",");
                    }



                    sw.Write(sw.NewLine);


                    //Now write all the rows.
                    lock (data)
                    {
                        DataView view = data.AsDataView();
                        //view.Sort = "DateTime";

                        foreach (DataRowView dr in view)
                        {
                            for (int i = 0; i < data.Columns.Count; i++)
                            {
                                if (!Convert.IsDBNull(dr[data.Columns[i].Ordinal]) & dr[data.Columns[i].Ordinal].ToString().Length > 0)
                                {
                                    sw.Write(dr[data.Columns[i].Ordinal].ToString());
                                }
                                else
                                {
                                    sw.Write("-9999.99");
                                }
                                if (i < data.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                            sw.Write(sw.NewLine);
                        }
                    }
                    sw.Close();

               
            
        }
        public static void OutputFile(List<List<string>> File, string filename)
        {
            //assummes filename is the full path of the source file

            string filenameonly = Path.GetFileNameWithoutExtension(filename);
            string newfilename = Path.GetDirectoryName(filename) + @"\" + filenameonly + "_ProcessedForWAEI_" + DateTime.Now.ToShortDateString ().Replace("/","") + ".csv";
            
            // Create the CSV file to which grid data will be exported.
            StreamWriter sw = new StreamWriter(newfilename, false);

            //column heads
            foreach (string s in File[0])
            {
                sw.Write(s);
                // Console.WriteLine(col.ColumnName.Replace("#", "_"));
                if (File[0].IndexOf (s) < File[0].Count - 1)

                    sw.Write(",");
            }
            File.RemoveAt(0);


            sw.Write(sw.NewLine);


            //Now write all the rows.
            lock (File)
            {
                
                //view.Sort = "DateTime";

                foreach (List<string> row in File)
                {
                    for (int i = 0; i < row.Count ; i++)
                    {
                        if (row[i].Length > 0)
                        {
                            sw.Write(row[i]);
                        }
                        else
                        {
                            sw.Write("-9999.99");
                        }
                        if (i < row.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
            }
            sw.Close();



        }
        public static void OutputFile(List<List<SummaryGridColumn >> File, string filename)
        {
            //assummes filename is the full path of the source file

            

            // Create the CSV file to which grid data will be exported.
            StreamWriter sw = new StreamWriter(filename, false);

            //column heads
            foreach (SummaryGridColumn s in File[0])
            {
               
                    if (s.Value  == null)
                    {
                        sw.Write("");
                        sw.Write(",");
                    }
                    else
                    {
                        sw.Write(s.Value.ToString());
                        // Console.WriteLine(col.ColumnName.Replace("#", "_"));
                        if (File[0].IndexOf(s) < File[0].Count - 1)
                            sw.Write(",");

                    }
                
                    
            }
            File.RemoveAt(0);


            sw.Write(sw.NewLine);


            //Now write all the rows.
            lock (File)
            {

                //view.Sort = "DateTime";

                foreach (List<SummaryGridColumn > row in File)
                {
                    for (int i = 0; i < row.Count; i++)
                    {
                        if (row[i].Value.ToString().Length > 0)
                        {
                            sw.Write(row[i].Value.ToString());
                        }
                        else
                        {
                            sw.Write("-9999.99");
                        }
                        if (i < row.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
            }
            sw.Close();



        }

       
    }
}
