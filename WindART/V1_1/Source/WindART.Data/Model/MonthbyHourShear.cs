using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public class MonthbyHourShear
    {
        private ISessionColumnCollection _collection;
        private DataView _data;
        IAxis _xaxis;
        IAxis _yaxis;

        public delegate void ProgressEventHandler(string msg);
        public event ProgressEventHandler AxisLengthsDetermined;
        public event ProgressEventHandler AddingShearValues;
        public event ProgressEventHandler AddedShearValues;
        

        public MonthbyHourShear(ISessionColumnCollection collection, DataView data)
        {
            _collection = collection;
            _data = data;

        }

        public IAxis Xaxis
        {
            set
            {
                _xaxis = value;
            }

        }
        public IAxis Yaxis
        {
            set
            {
                _yaxis = value;
            }
        }
        public bool CalculateWindSpeed(double shearht, out ShearCalculationGridCollection gridcollection)
        {
            try
            {
                //make sure there are axis vals set
                if(_xaxis==null || _yaxis==null)
                {
                    //Console.WriteLine(" shear not calculated because no axes were provided");
                    gridcollection = null;
                    return false;
                }
                //Console.WriteLine(" x axis " + _xaxis.AxisValues.Length + " y axis " + _yaxis.AxisValues.Length);
                HourAxis hourAxis=null;
                MonthAxis  monthAxis=null;
                WindDirectionAxis  wdAxis=null;

                //add axes to a list for easier parsing
                List<IAxis> AssignedAxis=new List<IAxis>();
                AssignedAxis .Add(_xaxis);
                AssignedAxis.Add(_yaxis);
                bool useWD=false;

                foreach(IAxis a in AssignedAxis )
                {
                    switch(a.AxisType )
                    {
                        case AxisType .Hour :
                            hourAxis=(HourAxis)a;
                            break;
                        case AxisType.Month :
                            monthAxis =(MonthAxis)a;
                            break;
                        case AxisType.WD :
                            wdAxis =(WindDirectionAxis)a;
                            useWD=true;

                            throw new NotImplementedException("shear by WD not yet developed");
                           
                        default:
                            break;
                    }
                }
                
                //Console.WriteLine("assigned axis types");

                _data.Sort = Settings.Default.TimeStampName;
                DateTime t = DateTime.Now;
                double missing = Settings.Default.MissingValue;

                //get a date at the beginning of the dataset provided 
                DateTime start = DateTime.Parse(_data[2][_collection.DateIndex].ToString());

                //get a list of the columns for this config sorted ascending by height
                SortedDictionary<double, ISessionColumn> orderedWsCols
                    = _collection.GetColumnsByType(SessionColumnType.WSAvg, start, true);

                if (orderedWsCols.Count == 0)
                    throw new ApplicationException("No WS Composite columns were found in MonthHourShear class. Config boundary dates may have been passed incorrectly");
                
                //sort columns descending by height
                orderedWsCols.Reverse();
                
                //put the column indexes into a list for coincident values constructor
                SortedDictionary<double, int> TopCols = new SortedDictionary<double, int>();

                foreach (KeyValuePair<double, ISessionColumn> kv in orderedWsCols)
                {
                    TopCols.Add(kv.Key , _collection[kv.Value.ColName ].ColIndex );
                }
                List<double> SensorHts = TopCols.Keys.ToList();
                
                SensorHts.Sort(); 

                //Console.WriteLine("first row after sort " + _data[1][0] + " " + _[SensorHts [1]] + " " + _data[1][SensorHts[0]]);

                //pass the column list and the dataview to get only  coincident rows for the columns passed
                XbyYShearCoincidentRowCollection  coincident 
                    = new XbyYShearCoincidentRowCollection (_collection.DateIndex,TopCols, _data);

                int wdColIndex = _collection["WDAvgComposite"].ColIndex;

                //Console.WriteLine("Shear.CalculateWS:wd comp index=" + wdColIndex );

                List<XbyYCoincidentRow> coincidentValues = coincident.GetRows(wdColIndex );

                //Console.WriteLine("coincident rows count " + coincidentValues.Count);

                //collection to hold averages at each x y point
                 gridcollection= new ShearCalculationGridCollection ();
                
                //get number of elements in each axis
                int hour=0;
                int month=0;
                int x = _xaxis.AxisValues.Length;
                int y = _yaxis.AxisValues.Length;
                //Console.WriteLine("assigned axis lengths " + x + ", " + y);
                if(AxisLengthsDetermined !=null)
                AxisLengthsDetermined("Assigned Alpha Grid Axis lengths " + x + " by " + y);
                //arrays to hold upper and lower avgs at each x,y point 
                    double[,] UpperAvgGrid = new double[x, y];
                    double[,] LowerAvgGrid = new double[x, y];

                    //list to hold avg values for each sensor grid at each x and y bin
                    List<double> upperWsVals=new List<double>();
                    List<double> lowerWSVals=new List<double>();
                    
                   for (int a = 0; a < x; a++)
                    {
                        for (int j = 0; j < y; j++)
                        {
                            int TestCount = 0;
                            if (useWD)
                            {
                                
                            }
                            else
                            {   
                                if (_xaxis.AxisType == AxisType.Hour)
                                {
                                     hour = a;  month = j;
                                }
                                else
                                {
                                    hour = j;  month = a;
                                }

                                hour = (int)hourAxis.AxisValues[hour];
                                month = (int)monthAxis.AxisValues[month];
                                //Console.WriteLine("hour " + hour + ", Month " + month);
                                //find the row collection for this x,y point
                                var  r= from s in coincidentValues.AsEnumerable()
                                        where s.Date.Month == month && s.Date.Hour == hour
                                        && s.WD >=0
                                        select s;
                                
                                //Console.WriteLine(month + " " + hour + " value: " + r.Count());

                                double useUpperWSVal;
                                double useLowerWSVal;
                                
                                    foreach (var row in r)
                                    {
                                        //if (month == 11)
                                        //{
                                        //    Console.WriteLine("date " + row.Date + " upper ws " + (double)row.UpperWS);
                                        //}
                                        if (Double.TryParse(row.UpperWS.ToString(), out useUpperWSVal) &&
                                            Double.TryParse(row.LowerWS.ToString(), out useLowerWSVal))
                                        {
                                            upperWsVals.Add(useUpperWSVal);
                                            lowerWSVals.Add(useLowerWSVal);
                                        }
                                        TestCount++;
                                    }
                                
                                
                            }
                            //Console.WriteLine("upper count at " + month + ", " + hour + ": " + TestCount);
                            //add each collection  avg to the avg grids 
                            
                            //Console.WriteLine("lower average at " + month + ", " + hour + ": " + lowerWSVals.Average() + "/n");
                            if (upperWsVals.Count > 0 && lowerWSVals.Count  > 0)
                            {
                                UpperAvgGrid[a, j] = upperWsVals.Average();
                                LowerAvgGrid[a, j] = lowerWSVals.Average();
                            }
                            else
                            {
                                UpperAvgGrid[a, j] = 0;
                                LowerAvgGrid[a, j] = 0;
                            }

                                upperWsVals.Clear();
                                lowerWSVals.Clear();
                            
                            //Console.WriteLine("Upper average at " + a + ", " + hour + " "+ UpperAvgGrid[a, j]);
                            //Console.WriteLine("Lower average at " + month + ", " + hour + " " + LowerAvgGrid[a, j]);
                        }
                      
                }
                    gridcollection.LowerAvg= LowerAvgGrid;
                    gridcollection .UpperAvg=UpperAvgGrid;
                   
                
                //calculate each alpha value and assign to  a single grid 
                    double[,] alphaGrid = new double[x, y];
                   
                //fill alpha table
                   
                   for (int a = 0; a < x; a++)
                   {
                       for (int j = 0; j < y; j++)
                       {
                           //Console.WriteLine(" x=" + a.ToString() + " y=" + j.ToString());
                           alphaGrid[a, j] = Math.Log(gridcollection .UpperAvg [a, j] / gridcollection .LowerAvg [a, j]) 
                               / Math.Log(SensorHts[1] / SensorHts[0]);
                           //Console.WriteLine("Alpha in grid=" + alphaGrid[a, j].ToString() +
                           //    " upper ws =" + gridcollection.UpperAvg[a, j].ToString() +
                           //    " Lower ws =" + gridcollection.LowerAvg[a, j].ToString() +
                           //    " upper ht " + SensorHts[1] + " lower ht " + SensorHts[0] +
                           //    " month=" + monthAxis.AxisValues[a] + " hour=" + hourAxis.AxisValues[j]);
                           if (Double.IsNaN(alphaGrid[a, j]))
                           {
                               alphaGrid[a, j] = 0;
                           }

                       }
                   }
                    
                  gridcollection .Alpha =alphaGrid;

                  //create a single multi-d array from the original dictionary of values 
                  //values must stay in order
                List<int> CombineCols = TopCols.Values.ToList();
                
                //height and value of the valid ws comp at each row ordered by datetime
                double[,] result = CombineSensorVals(CombineCols );
                
                //Console.WriteLine("Rows in full data " + _data.Count + " rows in combined dataset " + result.GetLength (1));
                //array to store sheared ws 
                List<double> ShearResult = new List<double>(result.GetLength(1));

                //add colummn to dataset and capture colindex 
                string localcolname = shearht.ToString();
                localcolname += Enum.GetName(typeof(SessionColumnType), SessionColumnType.WSAvgShear   );
                localcolname += x + "by" + y;
                //Console.WriteLine("X by Y shear column  will be named " + localcolname);

                int shearIndex = AddShearColumnToTable(localcolname, shearht);

                double thisAlpha;
                int xVal;
                int yVal;
                DateTime thisDate;
                double thisWD;
                if(AddingShearValues !=null)
                AddingShearValues("Calculating Sheared Windspeed");
                
                //*****derive new windspeed 
                //loop combined values 
                for (int i = 0; i < result.GetLength(1); i++)
                {
                    thisDate =(DateTime)_data[i][_collection .DateIndex ];

                    if (useWD)
                    {
                        thisWD=(double)_data[i][wdColIndex ];
                        
                    }
                    else
                    {
                        xVal=  Convert.ToInt16(monthAxis.GetAxisValue(thisDate));
                        yVal = Convert.ToInt16(hourAxis.GetAxisValue (thisDate));
                        thisAlpha = gridcollection .Alpha [xVal, yVal];
                        if (result[1, i] >= 0)
                        {
                            _data[i][shearIndex] = (result[1, i] * Math.Pow((shearht / result[0, i]), thisAlpha));
                        }
                        else
                        {
                            _data[i][shearIndex] = missing;
                        }
                    }
                }
                _data.Table.AcceptChanges();

                DateTime t_end = DateTime.Now;
                TimeSpan dur = t_end - t;
                Console.WriteLine("Calculating and adding " + x + " by " + y + " shear ws takes  " + dur);
                if(AddedShearValues !=null)
                AddedShearValues("Shear Calculation Complete");

                return true;
                           

            }
            catch (Exception e)
            {
                throw e;
            }
        }

      private double[,] CombineSensorVals(List<int> cols)
        {
            try
            {
                //values must stay in order 
                double missing = Settings.Default.MissingValue;
                //from a dictionary of sensor vals key column index  return one array

                //create array to store our results 
                int recCount = _data.Count;
                double[,] results = new double[2, recCount];
                //default item value
                double workVal;
                double workHt = 0;
                DateTime start = DateTime.Parse(_data[0][Settings.Default.TimeStampName].ToString());
                //sort the 
                SortedDictionary<double, int> workvals = new SortedDictionary<double, int>();
                foreach (int i in cols)
                {
                    double ht = _collection[i].getConfigAtDate(start).Height;
                    workvals.Add(ht, i);
                }

                //create dummy double for tryparse
                double dummy;
                string rowval;
                int j = 0;
                foreach (DataRowView row in _data)
                {
                    workHt = 0;
                    workVal = missing;
                    //if the top value is not missing take it, next take lower, next assign missing
                    foreach (KeyValuePair<double, int> kv in workvals.Reverse())
                    {
                        rowval = row[kv.Value].ToString();

                        if (Double.TryParse(rowval, out dummy))
                        {
                            if (dummy != missing)
                            {
                                workVal = dummy;
                                workHt = kv.Key;

                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //Console.WriteLine("count " + j + "    ht" + workHt + " Val" + workVal);
                    results[0, j] = workHt;
                    results[1, j] = workVal;
                    j++;
                }
                Console.WriteLine("combine vals count " + j);
                return results;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
      private int AddShearColumnToTable(string colname, double shearht)
      {
          //add column to datatable and column collection if necessary
          try
          {
              int shearIndex = 0;
              if (!_data.Table.Columns.Contains(colname))
              {
                  // add to datatable
                  DataColumn newcol = new DataColumn(colname);
                  newcol.DataType = typeof(double);
                  _data.Table.Columns.Add(newcol);
                  shearIndex = _data.Table.Columns[colname].Ordinal;

                  //add to col colection 
                  ISessionColumn newSessCol = new SessionColumn(shearIndex, colname);
                  newSessCol.ColumnType = SessionColumnType.WSAvgShear ;
                  Console.WriteLine("adding shear column " + colname);
                  _collection.Columns.Add(newSessCol);

                  //****metadata collection 
                  ISensorConfig config = new SensorConfig();
                  config.Height = shearht;
                  DateTime start = DateTime.Parse(_data[0][_collection.DateIndex].ToString());
                  DateTime end = DateTime.Parse(_data[_data.Count - 1][_collection.DateIndex].ToString());
                  config.StartDate = start;
                  config.EndDate = end;
                  _collection[colname].addConfig(config);

              }
              else
              {
                  shearIndex = _collection[colname].ColIndex;
              }


              return shearIndex;
          }
          catch (Exception e)
          {
              throw e;
          }
      }
      
    }
}
