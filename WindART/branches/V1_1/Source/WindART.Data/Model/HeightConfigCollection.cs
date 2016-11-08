using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class HeightConfigCollection:IConfigCollection 
    {
        ISessionColumnCollection _collection;
        public HeightConfigCollection(ISessionColumnCollection collection)
        {
            _collection = collection;
            
        }

        public List<IConfig> GetConfigs()
        {
            //determines configurations in column collection by evaluating changes in sensor HT
            //adds columns with the same ht in the date  of the config to a collection within each config 
            //does not provide speicifc configs but guarentess that the columns provided have the hts returned at 
            //the time of each config 
            try
            {
                List<DateTime> Dates = new List<DateTime>();
                List<IConfig> result = new List<IConfig>();

                foreach (ISessionColumn col in _collection.Columns)
                {
                    foreach (ISensorConfig c in col.Configs)//create distinct ht and date listing 
                    {
                        //Console.WriteLine(c.StartDate + " " + c.Height);
                        Dates.Add(c.StartDate);

                    }

                }
                List<DateTime> distinctDates = Dates.AsEnumerable().Distinct().ToList();
                distinctDates.Sort();

                //find different configs based on ht
                foreach (DateTime d in distinctDates)
                {
                    //create new config for the distinct date which is based on a differing  ht
                    HeightConfig  thisConfig = new HeightConfig();

                    thisConfig.StartDate = d;

                    if (distinctDates.IndexOf(d) + 1 > distinctDates.Count - 1)
                    {
                        //end date is now if end of array
                        SessionColumnCollection sc=(SessionColumnCollection)_collection ;
                        if (sc.DataSetEnd != default(DateTime))
                            thisConfig.EndDate = sc.DataSetEnd;
                        else
                        thisConfig.EndDate = DateTime.Now;
                    }
                    else
                    {
                        //end date is next date -1 second if not at end of array
                        thisConfig.EndDate = distinctDates[distinctDates.IndexOf(d) + 1].AddSeconds(-1);

                    }

                    //find all columns with this ht at this date

                    foreach (ISessionColumn col in _collection.Columns)
                    {
                        
                        //don't process this column if it is a composite
                        if (col.IsComposite  )
                        { break; }
                        ISensorConfig workConfig = col.getConfigAtDate(d);
                        
                        if (workConfig != null)
                        {
                            //if ht is already in dictionary add the column to that hts list
                            if (!thisConfig.Columns.ContainsKey(workConfig.Height)  )
                            {
                                //Console.WriteLine(col.ColName + " is ws " + (col.ColumnType == SessionColumnType.WSAvg));
                                IList<ISessionColumn> newcols = new List<ISessionColumn>();
                                newcols.Add(col);
                                thisConfig.Columns.Add(workConfig.Height, newcols);

                            }
                            else
                            {
                                thisConfig.Columns[workConfig.Height].Add(col);
                            }

                        }
                    }

                    result.Add(thisConfig);
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        
    }
}
