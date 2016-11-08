using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public class HourShear
    {       
            private ISessionColumnCollection _collection;
            private DataView _data;
            IAxis _xaxis;

        public HourShear(ISessionColumnCollection collection, DataView data)
        {
            _collection = collection;
            _data = data;
        }

        //properties 
        public IAxis Xaxis
        {
            set
            {
                _xaxis = value;
            }

        }
        public bool CalculateWindSpeed(double shearht, out ShearCalculationGridCollection gridcollection)
        {
            //make sure there are axis vals set
            if (_xaxis == null )
            {
                Console.WriteLine(" shear not calculated because no axis was provided");
                gridcollection = null;
                return false;
            }
            Console.WriteLine(" x axis " + _xaxis.AxisValues.Length );
            HourAxis hourAxis = (HourAxis)_xaxis;
            
            _data.Sort = Settings.Default.TimeStampName;
            DateTime t = DateTime.Now;
            double missing = Settings.Default.MissingValue;

            //get a date at the beginning of the dataset provided 
            DateTime start = DateTime.Parse(_data[2][_collection.DateIndex].ToString());

            //get a list of the columns for this config sorted ascending by height
            SortedDictionary<double, ISessionColumn> orderedWsCols
                = _collection.GetColumnsByType(SessionColumnType.WSAvg, start, true);

            //sort columns descending by height
            orderedWsCols.Reverse();

            //put the column indexes into a list for coincident values constructor
            SortedDictionary<double, int> TopCols = new SortedDictionary<double, int>();

            foreach (KeyValuePair<double, ISessionColumn> kv in orderedWsCols)
            {
                TopCols.Add(kv.Key, kv.Value.ColIndex);
            }
            List<double> SensorHts = TopCols.Keys.ToList();
            SensorHts.Sort();
           
            //pass the column list and the dataview to get only  coincident rows for the columns passed
            XbyYShearCoincidentRowCollection coincident
                = new XbyYShearCoincidentRowCollection(_collection.DateIndex, TopCols, _data);

            int wdColIndex = _collection["WDAvgComposite"].ColIndex;

            Console.WriteLine("wd comp index=" + wdColIndex);

            List<XbyYCoincidentRow> coincidentValues = coincident.GetRows(wdColIndex);

            Console.WriteLine("coincident rows count " + coincidentValues.Count);

            //collection to hold averages at each x y point
            gridcollection = new ShearCalculationGridCollection();

            //get number of elements in each axis
            int hour = 0;
            
            int x = _xaxis.AxisValues.Length;
            
            Console.WriteLine("assigned axis length  1 by " + x );
            //arrays to hold upper and lower avgs at each x,y point 
            double[,] UpperAvgGrid = new double[1, x];
            double[,] LowerAvgGrid = new double[1, x];

            //list to hold avg values for each sensor grid at each x and y bin
            List<double> upperWsVals = new List<double>();
            List<double> lowerWSVals = new List<double>();
            
            //only 1 element because this is a 1 by x shear 
            for (int a = 0; a < 1; a++)
            {
                for (int j = 0; j < x; j++)
                {
                    int TestCount = 0;
                    //Console.WriteLine(" a " + a + " j " + j);
                    hour = (int)hourAxis.AxisValues[j];

                    //find the row collection for this x,y point
                    var r = from s in coincidentValues.AsEnumerable()
                            where s.Date.Hour == hour
                            && s.WD >= 0
                            select s;

                    //Console.WriteLine(month + " " + hour + " value: " + r.Count());

                    double useUpperWSVal;
                    double useLowerWSVal;
                    foreach (var row in r)
                    {

                        if (Double.TryParse(row.UpperWS.ToString(), out useUpperWSVal) &&
                            Double.TryParse(row.LowerWS.ToString(), out useLowerWSVal))
                        {
                            upperWsVals.Add(useUpperWSVal);
                            lowerWSVals.Add(useLowerWSVal);
                        }
                        TestCount++;
                    }
                    if (upperWsVals.Count > 0 && lowerWSVals.Count > 0)
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
                }

            }
            gridcollection.LowerAvg = LowerAvgGrid;
            gridcollection.UpperAvg = UpperAvgGrid;


            //calculate each alpha value and assign to  a single grid 
            double[,] alphaGrid = new double[1, x];

            //fill alpha table

            for (int a = 0; a < 1; a++)
            {
                for (int j = 0; j < x; j++)
                {
                    //Console.WriteLine(" x=" + a.ToString() + " y=" + j.ToString());
                    alphaGrid[a, j] = Math.Log(gridcollection.UpperAvg[a, j] / gridcollection.LowerAvg[a, j])
                        / Math.Log(SensorHts[1] / SensorHts[0]);
                    //Console.WriteLine("Alpha in grid=" + alphaGrid[a, j].ToString() +
                    //    " upper ws =" + gridcollection.UpperAvg[a, j].ToString() +
                    //    " Lower ws =" + gridcollection.LowerAvg[a, j].ToString() +
                    //    " upper ht " + SensorHts[1] + " lower ht " + SensorHts[0] +
                    //    " month=" + monthAxis.AxisValues[a] + " hour=" + hourAxis.AxisValues[j]);
                }
            }

            gridcollection.Alpha = alphaGrid;

            //create a single multi-d array from the original dictionary of values 
            //values must stay in order
            List<int> CombineCols = TopCols.Values.ToList();

            //height and value of the valid ws comp at each row ordered by datetime
            double[,] result = CombineSensorVals(CombineCols);

            Console.WriteLine("Rows in full data " + _data.Count + " rows in combined dataset " + result.GetLength(1));
            //array to store sheared ws 
            List<double> ShearResult = new List<double>(result.GetLength(1));

            //add colummn to dataset and capture colindex 
            string localcolname = shearht.ToString();
            localcolname += Enum.GetName(typeof(SessionColumnType), SessionColumnType.WSAvgSingleAxisShear );
            localcolname += "1" + "by" + x;
            Console.WriteLine("X by Y shear column will be named " + localcolname);

            int shearIndex = AddShearColumnToTable(localcolname, shearht);

            double thisAlpha;
            int yVal;
            DateTime thisDate;
            
            //loop combined values 
            for (int i = 0; i < result.GetLength(1); i++)
            {
                thisDate = (DateTime)_data[i][_collection.DateIndex];

                    yVal = Convert.ToInt16 (hourAxis.GetAxisValue(thisDate));
                    thisAlpha = gridcollection.Alpha[0, yVal];

                    if (result[0, i] > 0 )
                    {
                        _data[i][shearIndex] = (result[1, i] * Math.Pow((shearht / result[0, i]), thisAlpha));
                        //Console.WriteLine(_data[i][shearIndex]);
                    }
                    else
                    {
                        _data[i][shearIndex] = missing;
                    }
                
            }
            _data.Table.AcceptChanges();

            DateTime t_end = DateTime.Now;
            TimeSpan dur = t_end - t;
            Console.WriteLine("Calculating and adding 1 by " + x + " shear ws takes  " + dur);

            return true;
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
                    newSessCol.ColumnType = SessionColumnType.WSAvgSingleAxisShear  ;
                    Console.WriteLine("adding shear column " + colname);
                    _collection.Columns.Add(newSessCol);

                    //****metadata collection 
                    ISensorConfig config = new SensorConfig();
                    config.Height = shearht;
                    DateTime start = DateTime.Parse(_data[0][_collection.DateIndex].ToString());
                    DateTime end 
                        = DateTime.Parse(_data[_data.Count - 1][_collection.DateIndex].ToString());
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
