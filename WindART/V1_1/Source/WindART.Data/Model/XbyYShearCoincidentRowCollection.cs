using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties ;
using System.Windows.Forms;

namespace WindART
{
    public class XbyYShearCoincidentRowCollection
    {
        SortedDictionary<double, int> _colindex;
        DataView _data;
        int _dateindex;

        public XbyYShearCoincidentRowCollection(int dateindex, SortedDictionary<double, int> wsindex, DataView data)
        {
            _data = data;
            _colindex = wsindex;
            _dateindex = dateindex;
        }


       
        public List<XbyYCoincidentRow> GetRows(int wdindex)
        {

            try
            {
                _data.Sort = Settings.Default.TimeStampName;
                DateTime t = DateTime.Now;
                string missing = Settings.Default.MissingValue.ToString();
                bool Skip;

                List<XbyYCoincidentRow> result = new List<XbyYCoincidentRow>();

                // fill if no elements are set to missing 
                double[] workarry = new double[_colindex.Count];
                int arridx = 0;
                double usewdVal;
                bool done = true;
                foreach (DataRowView row in _data)
                {
                    //Console.WriteLine("inside coincident data row iteration");
                    Skip = false;
                    arridx = 0;

                    //if none of these are less than zero then add to the coincident collection below 
                    //Console.WriteLine("ws columns stored in coincident val class:" + _colindex.Values.Count);
                    foreach (int i in _colindex.Values)
                    {
                        if (double.TryParse(row[i].ToString(), out usewdVal))
                        {
                            workarry[arridx] = usewdVal;
                            if (workarry[arridx] < 0)
                            {
                                //Console.WriteLine(row[0] + " idx " + i + " bad value " + workarry[arridx]);
                                Skip = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine(row[0] + " idx " + i + " bad value " + workarry[arridx]);
                            Skip = true;
                            break;
                        }
                        arridx++;
                    }

                    if (!Skip)
                    {

                        if (done)
                        {
                            Console.WriteLine("Date " + row[_dateindex] +
                                " lower " + row[_colindex[_colindex.Keys.Min()]]
                                + " upper " + row[_colindex[_colindex.Keys.Max()]]
                                + " wd " + row[wdindex].GetType());
                            done = false;
                        }

                        XbyYCoincidentRow thisRow = new XbyYCoincidentRow();
                        thisRow.Date = (DateTime)row[_dateindex];
                        thisRow.LowerWS = double.Parse(row[_colindex[_colindex.Keys.Min()]].ToString());
                        thisRow.UpperWS = double.Parse(row[_colindex[_colindex.Keys.Max()]].ToString());
                        thisRow.WD = double.Parse(row[wdindex].ToString());
                        result.Add(thisRow);


                    }


                }
                DateTime end = DateTime.Now;
                TimeSpan dur = end - t;
                Console.WriteLine("Coincident Values takes " + dur);
                if (result.Count == 0)
                {
                    Console.WriteLine("No Coincident rows were found");
                    return default(List<XbyYCoincidentRow>);
                }
                else
                    return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<XbyYCoincidentRow> GetRows(int wdindex, int shearindex)
        {

            try
            {
                _data.Sort = Settings.Default.TimeStampName;
                DateTime t = DateTime.Now;
                string missing = Settings.Default.MissingValue.ToString();
                bool Skip;

                List<XbyYCoincidentRow> result = new List<XbyYCoincidentRow>();



                // fill if no elements are set to missing 
                double[] workarry = new double[_colindex.Count];
                int arridx = 0;
                string badrows = string.Empty;
                foreach (DataRowView row in _data)
                {

                    Skip = false;
                    arridx = 0;
                    double success = 0;
                    //if none of these are less than zero then add to the coincident collection below 
                    foreach (int i in _colindex.Values)
                    {
                        if (double.TryParse(row[i].ToString(), out success))
                        {
                            workarry[arridx] = success;
                            if (workarry[arridx] < 0)
                            {
                                Skip = true;
                                break;
                            }
                            arridx++;
                        }
                        else
                        {
                            badrows += row[0].ToString() + i.ToString() + Environment.NewLine ;
                            Skip = true;
                        }
                    }
                    if (!Skip)
                    {


                        //Console.WriteLine("Coincident values: " + workarry [localcnt]);
                        XbyYCoincidentRow thisRow = new XbyYCoincidentRow();
                        thisRow.Date = DateTime.Parse(row[_dateindex].ToString());
                        thisRow.LowerWS = double.Parse (row[_colindex[_colindex.Keys.Min()]].ToString ());
                        thisRow.UpperWS = double.Parse(row[_colindex[_colindex.Keys.Max()]].ToString());
                        thisRow.WD = double.Parse(row[wdindex].ToString());
                        thisRow.Shear = double.Parse(row[shearindex].ToString());
                        result.Add(thisRow);


                    }

                   


                }
                DateTime end = DateTime.Now;
                TimeSpan dur = end - t;
                Console.WriteLine("Coincident Values takes " + dur);
                
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<XbyYCoincidentRow> GetNCDNTRows(int wdindex, int shearindex)
        {

            try
            {
                _data.Sort = Settings.Default.TimeStampName;
                DateTime t = DateTime.Now;
                string missing = Settings.Default.MissingValue.ToString();
                

                List<XbyYCoincidentRow> result = new List<XbyYCoincidentRow>();



                // fill if no elements are set to missing 
                double[] workarry = new double[_colindex.Count];
               
                string badrows = string.Empty;
                foreach (DataRowView row in _data)
                {


                    double thisLowerWS = -9999.99;
                    double thisUpperWS = -9999.99;
                    double thisShear = -9999.99;
                    double thisWD = -9999.99;

                    if (double.TryParse(row[_colindex[_colindex.Keys.Min()]].ToString(), out thisLowerWS)) { };
                    if (double.TryParse(row[_colindex[_colindex.Keys.Max()]].ToString(), out thisUpperWS)) { };
                    if (double.TryParse(row[shearindex].ToString(), out thisShear)) { };
                    if (double.TryParse(row[wdindex].ToString(), out thisWD)) { };

                        //Console.WriteLine("Coincident values: " + workarry [localcnt]);
                        XbyYCoincidentRow thisRow = new XbyYCoincidentRow();
                        thisRow.Date = DateTime.Parse(row[_dateindex].ToString());
                        thisRow.LowerWS = thisLowerWS;
                        thisRow.UpperWS = thisUpperWS;
                        thisRow.WD = thisWD;
                        thisRow.Shear =thisShear;
                        result.Add(thisRow);

                }
                DateTime end = DateTime.Now;
                TimeSpan dur = end - t;
                Console.WriteLine("Non Coincident Values takes " + dur);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        
    }
}