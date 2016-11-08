using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;
using System.Windows.Forms;

namespace WindART
{
    public class WindDirectionComposite:IComposite
    {
        private DataTable _data;
        private ISessionColumnCollection _columns;

        public delegate void ProgressEventHandler(string msg);
        public event ProgressEventHandler NewCompositeColumnAdded;
        public event ProgressEventHandler DeterminingWindDirectionCompositeValues;
        public event ProgressEventHandler CompletedWindDirectionCompositeValues;

        public WindDirectionComposite(ISessionColumnCollection columns, DataTable data)
        {
            _data = data;
            _columns = columns;
        }
        public bool CalculateComposites()
        {
            try
            {
                //**** get list 
                List<ISessionColumn> initialColList = _columns.GetColumnsByType(SessionColumnType.WDAvg);
                if (initialColList.Count == 0)
                {
                    Console.WriteLine("no cols found");
                    return false;
                }
                //add the columns to the datatable to be populated. pass the list of existing wd cols because 
                //if one wd has more child columns than the other they all must be created for the code to 
                //execue without error 

                new WindDirectionCompColumns(_columns, _data).Add(initialColList);
                if (NewCompositeColumnAdded != null)
                    NewCompositeColumnAdded("Wind Direction Composite Column Added");

                string CompColName = Settings.Default.CompColName;
                double missingVal = Settings.Default.MissingValue;

                string compColPrefix = string.Empty;
                string ChildColPrefix = string.Empty;
                SortedDictionary<double, ISessionColumn> LocalWdCols;
                double outPutCatch;
                double childOutputVal;
                DataView view = _data.AsDataView();
                view.Sort = _columns[_columns.DateIndex].ColName + " asc";

               // Console.WriteLine(_columns[_columns.DateIndex].ColName + " asc");
                if (DeterminingWindDirectionCompositeValues != null)
                    DeterminingWindDirectionCompositeValues("Assigning Wind Direction Composite Values");
                foreach (DataRowView row in view)
                {
                    //Console.WriteLine("date " + DateTime.FromOADate (double.Parse(row[_columns.DateIndex].ToString())));

                    //get list of columns sorted desc by height at this date
                    LocalWdCols = _columns.GetColumnsByType(SessionColumnType.WDAvg, (DateTime)row[_columns.DateIndex]);

                    //set the wd comp column to the first valid value from heighest ht to lowest
                    //Console.WriteLine("WD Columns found " + LocalWdCols.Count);
                    row.BeginEdit();
                    foreach (KeyValuePair<double, ISessionColumn> kv in LocalWdCols.Reverse())
                    {
                        compColPrefix = Enum.GetName(typeof(SessionColumnType), kv.Value.ColumnType);

                        //evaluate parent column 
                        //if it is not missing and evaluates to a double use the value
                        if (double.TryParse(row[kv.Value.ColIndex].ToString(), out outPutCatch))
                        {
                            if (outPutCatch >= 0)
                            {

                                //set parent comp 
                                StringBuilder s = new StringBuilder();
                                s.Append(compColPrefix);
                                s.Append(CompColName);
                                row[s.ToString()] = outPutCatch;

                                //assign child column values  of the selected parent column to the comp child column  
                                foreach (ISessionColumn child in kv.Value.ChildColumns)
                                {
                                    ChildColPrefix = Enum.GetName(typeof(SessionColumnType), child.ColumnType);
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(ChildColPrefix);
                                    sb.Append(CompColName);

                                    if (double.TryParse(row[child.ColIndex].ToString(), out childOutputVal))
                                    {
                                        row[sb.ToString()] = childOutputVal;
                                    }
                                    else
                                    {
                                        row[sb.ToString()] = missingVal;
                                    }
                                }
                                break;
                            }
                        }


                        //nothing valid found set parent and child comp column values  to missing 
                        row[compColPrefix + CompColName] = missingVal;
                        //System.Windows.Forms.MessageBox.Show(row[0].ToString() + " " + row[LocalWdCols.First ().Value .ColIndex].ToString() + row[LocalWdCols.Last().Value.ColIndex].ToString());
                        foreach (ISessionColumn child in kv.Value.ChildColumns)
                        {
                            //Console.WriteLine("System setting wd comp to missing; thinks lower wd val is " + outPutCatch);
                            string prefix = Enum.GetName(typeof(SessionColumnType), child.ColumnType);
                            StringBuilder sb = new StringBuilder();
                            sb.Append(prefix);
                            sb.Append(CompColName);
                            row[sb.ToString()] = missingVal;
                        }

                    }
                    row.EndEdit();
                    
                }
                _data.AcceptChanges();
                if (CompletedWindDirectionCompositeValues != null)
                    CompletedWindDirectionCompositeValues("Completed Generating Wind Direction Composites");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(" Error while processing WD Comps: " + e.Message);
                return false;
            }
            finally
            {
               
            }
            
                      
        }
       
        
    }
}
