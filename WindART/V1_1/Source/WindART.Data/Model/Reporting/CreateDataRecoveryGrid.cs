using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class CreateDataRecoveryGrid:ICreateGridAlgorithm 
    {
        protected DataView _data;
        protected SessionColumnCollection _collection;
        protected MonthYearAxis _axis;

        public CreateDataRecoveryGrid(ISessionColumnCollection collection,DataView data)
        {
            _data = data;
            _collection = (SessionColumnCollection)collection;
            
            
        }

        public List<List<SummaryGridColumn>> CreateGrid()
        {//get datarange of dataset
            _data.Sort = _collection[_collection.DateIndex].ColName;
            DateTime startdate = (DateTime)_data[0][_collection.DateIndex];
            DateTime enddate = (DateTime)_data[_data.Count-1][_collection.DateIndex];

            //create monthyear axis
            _axis = new MonthYearAxis(startdate, enddate);

            Console.WriteLine("axis values " + _axis.AxisValues.Length) ;
            List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>();
            
            int upperwscompindex = _collection.UpperWSComp(startdate);
            int lowerwscompindex = _collection.LowerWSComp(startdate);
            int wdcompindex = _collection.WDComp(startdate);
            double uppercompht = _collection[upperwscompindex].getConfigAtDate(startdate).Height;
            double lowercompht = _collection[lowerwscompindex].getConfigAtDate(startdate).Height;
            List<ISessionColumn> upperwscols = _collection.UpperWSAvgCols ( startdate);
            List<ISessionColumn> secondwscols = _collection.SecondWSAvgCols (startdate);
            SortedDictionary<double, ISessionColumn> wdcols = _collection.GetColumnsByType(SessionColumnType.WDAvg, startdate);
                
            List<double> upperwscomp = new List<double>();
            List<double> lowerwscomp = new List<double>();
            List<double> wdcomp = new List<double>();
            Console.WriteLine("ws column count " + upperwscols.Count);
            Console.WriteLine("ws column count " + secondwscols.Count);
            Console.WriteLine(" wd column count " + wdcols.Count);
            List<List<double>> upperwsColumnValues = new List<List<double>>(upperwscols.Count);
            List<List<double>> secondwsColumnValues = new List<List<double>>(secondwscols.Count);
            List<List<double>> wdColumnValues = new List<List<double>>(wdcols.Count);

            for (int i = 0; i < upperwscols.Count; i++)
            {
                
                upperwsColumnValues.Add( new List<double>());
            }
            for (int i = 0; i < secondwscols.Count; i++)
            {
                
                secondwsColumnValues.Add(new List<double>());
            }
            for (int j = 0; j < wdcols.Count; j++)
            {
                
                wdColumnValues.Add(new List<double>());
            }


            for (int i = 0; i < _axis.AxisValues.Length + 1;i++ )
            {
                //build grid one row at a time 
                //for each month add a new row 
                List<SummaryGridColumn> row = new List<SummaryGridColumn>();
                //if on first row 

                if (i == 0)
                {
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(null)));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Possible Data Count")));
                    //fill ws value lists
                    foreach (ISessionColumn col in upperwscols)
                    {
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(col.ColName)));

                    }
                    foreach (ISessionColumn col in secondwscols)
                    {
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(col.ColName)));

                    }
                    //fill wd value lists
                    foreach (KeyValuePair<double, ISessionColumn> kv in wdcols)
                    {
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(kv.Value.ColName)));
                    }
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("WD Comp % of recovery")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(uppercompht + "m WS Comp % of recovery")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(uppercompht + "m WS Comp")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(lowercompht + "m WS Comp % of recovery")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(lowercompht + "m WS Comp")));

                }
                else
                {
                    //populate data 
                    int thismonth = startdate.AddMonths(i - 1).Month;
                    int thisyear = startdate.AddMonths(i - 1).Year ;
                    Console.WriteLine(" date 1 " + thismonth + @"/" + "01" + @"/" + thisyear + " enddate " + enddate); 
                    if (DateTime.Parse (thismonth + @"/" + "01" + @"/" + thisyear ) <= enddate    )
                    {
                        Console.WriteLine(" this month " + thismonth + " this year " + thisyear);
                        DataView thisRowsData= FilterRows(thismonth, thisyear);
                        Console.WriteLine("successfully returned from filter. this row data " + thisRowsData.Count);
                        double thisval;
                        foreach (DataRowView drv in thisRowsData)
                        {
                            //Console.WriteLine("upper ws " + drv[upperwscompindex]);
                            if(!double.TryParse (drv[upperwscompindex].ToString(),out thisval))
                            {
                                thisval=-9999;
                            }
                            

                            upperwscomp.Add(thisval);
                            
                            //Console.WriteLine("lower ws " + drv[lowerwscompindex]);
                            if(!double.TryParse (drv[lowerwscompindex].ToString(),out thisval))
                            {
                                thisval=-9999;
                            }
                            
                            lowerwscomp.Add(thisval);

                            
                            //Console.WriteLine("wd comp " + drv[wdcompindex]);
                            if(!double.TryParse (drv[wdcompindex].ToString(),out thisval))
                            {
                                thisval=-9999;
                            }
                            
                            wdcomp.Add(thisval);
                            

                            //fill ws value lists

                            foreach (ISessionColumn col in upperwscols)
                            {
                                int thisindex = upperwscols.IndexOf(col);
                                double parseResult;
                                if(!double.TryParse (drv[col.ColIndex].ToString(),out parseResult))
                                {parseResult = -9999.00;}
                                
                                upperwsColumnValues[thisindex].Add(parseResult );
                            }
                            foreach (ISessionColumn col in secondwscols)
                            {
                                int thisindex = secondwscols.IndexOf(col);
                                double parseResult;
                                if (!double.TryParse(drv[col.ColIndex].ToString(), out parseResult))
                                { parseResult = -9999.00; }
                                secondwsColumnValues[thisindex].Add(parseResult );

                            }
                            //fill wd value lists
                            foreach (KeyValuePair<double, ISessionColumn> kv in wdcols)
                            {
                                int thisIndex = wdcols.Keys.ToList().IndexOf(kv.Key);
                                double parseResult;
                                if (!double.TryParse(drv[kv.Value.ColIndex].ToString(), out parseResult))
                                { parseResult = -9999.00; }

                                wdColumnValues[thisIndex].Add(parseResult );
                            }
                        }
                        Console.WriteLine(" filled lists for " + i  );
                        //fill each column of the current row 
                        //must be in exact order as headings above 
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(_axis.ReturnAxisValueHeader(i - 1))));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(thisRowsData.Count)));
                        foreach (List<double> ws in upperwsColumnValues)
                        {
                            if (ws.DefaultIfEmpty ().Count () > 0)
                            {
                                row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(ws)));
                                ws.Clear();
                            }
                        }
                        foreach (List<double> ws in secondwsColumnValues)
                        {
                            if (ws.DefaultIfEmpty ().Count ()>  0)
                            {
                                row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(ws)));
                                ws.Clear();
                            }
                        }
                        foreach (List<double> wd in wdColumnValues)
                        {
                            if (wd.DefaultIfEmpty ().Count ()  > 0)
                            {
                                row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(wd)));
                                wd.Clear();
                            }
                        }
                        if (wdcomp.DefaultIfEmpty ().Count ()  > 0)
                        {
                            row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(wdcomp)));
                        }
                        if (upperwscomp.DefaultIfEmpty().Count() > 0)
                        {
                            row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(upperwscomp)));
                            row.Add(new SummaryGridColumn(new AverageGridColumn(upperwscomp)));
                        }

                        if (lowerwscomp.DefaultIfEmpty().Count() > 0)
                        {
                            row.Add(new SummaryGridColumn(new DataRecoveryRateGridColumn(lowerwscomp)));
                            row.Add(new SummaryGridColumn(new AverageGridColumn(lowerwscomp)));
                        }


                        wdcomp.Clear();
                        upperwscomp.Clear();
                        lowerwscomp.Clear();

                    }
                }

                Console.WriteLine ("adding row " + i);
               workgrid.Add(row);
               
            }
            
            return workgrid;
            
        }
        protected virtual DataView FilterRows(int month, int year)
        {
            //Console.WriteLine("month passed to datarecovery filter : " + month + " year " + year);
            var result = from r in _data.Table.AsEnumerable()
                         where r !=null & ((DateTime)r[_collection.DateIndex]).Year== year &&
                         ((DateTime)r[_collection.DateIndex]).Month == month
                         select r;

            Console.WriteLine(" data recovery row count :" + result.DefaultIfEmpty ().Count() +
            " month " + month + " year " + year);



            return result.DefaultIfEmpty().CopyToDataTable().AsDataView();
                        
            

        }
        


       
    }
}
