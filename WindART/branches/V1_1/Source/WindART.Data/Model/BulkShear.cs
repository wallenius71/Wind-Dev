using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public class BulkShear
    {
        DataView _data;
        ISessionColumnCollection _collection;

        public BulkShear(ISessionColumnCollection collection, DataView data)
        {
            //receives dataview because could be operating on subset of the data
            _data = data;
            
            _collection = collection;

        }
        public bool CalculateWindSpeed(double shearht, out ShearCalculationGridCollection grid)
        {

           //there is nothing here that guarantees that the view does not span multple configs
           //but assumes that it does not 
            try
            {
                _data.Sort = Settings.Default.TimeStampName;
                DateTime t = DateTime.Now;
                double missing = Settings.Default.MissingValue;

                //get a date at the beginning of the dataset provided 
                DateTime start = DateTime.Parse(_data[0][_collection.DateIndex].ToString());

                //get a list of the columns for this config sorted ascending by height
                SortedDictionary<double, ISessionColumn> orderedWsCols
                    = _collection.GetColumnsByType(SessionColumnType.WSAvg, start, true);
                
                
                //sort columns descending by height
                orderedWsCols.Reverse();
                List<ISessionColumn> workCols = orderedWsCols.Values.ToList();

                //put the column indexes into a list for coincident values constructor
                List<int> TopCols = new List<int>();
                foreach (KeyValuePair<double, ISessionColumn> kv in orderedWsCols)
                {
                    Console.WriteLine("col name " +kv.Value.ColName);
                    TopCols.Add(kv.Value.ColIndex);
                }
                Console.WriteLine("first row after sort " + _data[0][0] + " " + _data[0][TopCols[0]] + " " +  _data[0][TopCols[1]] );
                
                //pass the column list and the dataview to get the coincident values of the columns 
                ICoincidentValues coincident = new CoincidentValues(TopCols, _data);
                
                //take the average of the each of the lists of values 
                //add the average to a dictionary indexed by the column index 
                Dictionary<int, List<double>> coincidentValues = coincident.GetValues();

                Dictionary<int,double> avgvals = new Dictionary<int,double>();

                foreach (KeyValuePair <int,List<double>> kv in coincidentValues)
                {
                    avgvals.Add(kv.Key ,kv.Value.Average ());

                    Console.WriteLine(_collection[kv.Key].ColName + " " + kv.Value.Count);

                    double ht=_collection[kv.Key].getConfigAtDate (start).Height ;
                                        
                }
                
                Console.WriteLine("upper avg " + avgvals[TopCols[1]] + " lower avg " + avgvals[TopCols[0]]);
                Console.WriteLine("upper ht " + orderedWsCols.Keys.ToList()[1] + " lower ht " + orderedWsCols.Keys.ToList()[0]);
                //calculate alpha
                Console.WriteLine(" num " + Math.Log(avgvals[TopCols[1]] / avgvals[TopCols[0]]));
                Console.WriteLine(" den " + Math.Log(orderedWsCols.Keys.ToList()[1] / orderedWsCols.Keys.ToList()[0]));
                
                double alpha = Math.Log(avgvals[TopCols[1]] / avgvals[TopCols[0]])
                    / Math.Log( orderedWsCols .Keys.ToList ()[1] / orderedWsCols .Keys.ToList()[0]);

                Console.WriteLine("alpha=" + alpha.ToString());

                //create a single multi-d array from the original dictionary of values 
                //values must stay in order
                double[,] result = CombineSensorVals(TopCols );
                                                
                List<double> ShearResult = new List<double>(result.GetLength(1));

                //add colummn to dataset
                string localcolname = shearht.ToString ();
                localcolname += Enum.GetName(typeof(SessionColumnType), SessionColumnType.WSAvgBulkShear);
                Console.WriteLine("composite col will be named " + localcolname);

                int shearIndex= AddBulkShearToTable(localcolname,shearht);

                //apply bulk shear alpha to create sheared array values
                for (int i = 0; i < result.GetLength (1); i++)
                {
                    if (result[1, i] >= 0)
                    {
                        _data[i][shearIndex ]=(result[1, i] * Math.Pow((shearht / result[0, i]),  alpha));
                     }
                    else
                    {
                        _data[i][shearIndex] = missing;
                    }
                }
                _data.Table.AcceptChanges();

                DateTime t_end = DateTime.Now;
                TimeSpan dur = t_end-t;
                Console.WriteLine("Calculating and adding bulk shear ws takes  " + dur);
                grid = null;
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
                DateTime start=DateTime.Parse(_data[0][Settings.Default .TimeStampName ].ToString());
                //sort the 
                SortedDictionary <double, int> workvals = new SortedDictionary<double, int>();
                foreach (int i in cols)
                {
                    double ht = _collection[i].getConfigAtDate(start).Height;
                    workvals.Add(ht, i);
                }
                
                //create dummy double for tryparse
                double dummy;
                string rowval;
                int j = 0;
                foreach(DataRowView row in _data)
                {
                    workHt = 0;
                    workVal = missing;
                    //if the top value is not missing take it, next take lower, next assign missing
                    foreach(KeyValuePair <double,int> kv in workvals .Reverse ())
                    {
                        rowval=row[kv.Value].ToString();

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
        private int AddBulkShearToTable(string colname, double shearht)
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
                    newSessCol.ColumnType = SessionColumnType.WSAvgBulkShear;
                    _collection.Columns.Add(newSessCol);

                    //****metadata collection 
                    ISensorConfig config = new SensorConfig();
                    config.Height = shearht ;
                    DateTime start=DateTime.Parse(_data[0][_collection.DateIndex].ToString());
                    DateTime end = DateTime.Parse(_data[_data.Count -1][_collection.DateIndex].ToString());
                    config.StartDate = start;
                    config.EndDate = end;
                    _collection[colname].addConfig(config);

                }
                else
                {
                    shearIndex = _collection[colname].ColIndex;
                }

               
                return shearIndex ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
