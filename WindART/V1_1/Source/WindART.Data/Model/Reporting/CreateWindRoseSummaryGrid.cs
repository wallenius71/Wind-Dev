using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class CreateWindRoseSummaryGrid:AbstractCreateGridAlgorithm 
    {
        
        //constructor
        public CreateWindRoseSummaryGrid(List<XbyYCoincidentRow> filtereddata, WindDirectionAxis axis,
            double upperwsht, double lowerwsht, double shearht, SessionColumnType sheartype)
        {
            _axis = axis;
            _filtereddata = filtereddata;
            _upperwsht = upperwsht;
            _lowerwsht = lowerwsht;
            _shearht = shearht;
            _sheartype = sheartype;
        }
        public override List<List<SummaryGridColumn >> CreateGrid()
        {
            try
            {
                List<List<SummaryGridColumn >> workgrid = new List<List<SummaryGridColumn >>(_axis.AxisValues.Length );
                for (int i = 0; i < _axis.AxisValues.Length+1; i++)
                {
                    //create a row and add it to the grid

                    List<SummaryGridColumn  >row = new List<SummaryGridColumn>();

                    if (i == 0)
                    {
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("WD Bin " + _axis.Incrementor + " deg")));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Count")));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Freq")));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Average of " + _shearht + "m " + _sheartype)));
                    }
                    else
                    {
                           //filter data for this axis element
                        List<XbyYCoincidentRow> thisRowsValues = new List<XbyYCoincidentRow>();
                        thisRowsValues = FilterRows(i-1);
                        //fill ws lists
                        List<double> upperWS=new List<double>(_filtereddata.Count);
                        List<double> lowerWS=new List<double>(_filtereddata.Count);
                        List<double> shear=new List<double> (_filtereddata .Count);
                        foreach(XbyYCoincidentRow r in thisRowsValues )
                        {
                            upperWS.Add(r.UpperWS );
                            lowerWS.Add(r.LowerWS );
                            shear.Add (r.Shear );

                        } 
                        //wd grid has the following columns
                        row.Add(new SummaryGridColumn(new AxisValueGridColumn(_axis, i-1)));
                        // "WD " + _axis .BinWidth + " deg bin"));
                        //count
                        row.Add(new SummaryGridColumn(new CountGridColumn(thisRowsValues.ConvertAll(c=>(object)c))));
                        // "Count"));
                        //freq
                        row.Add(new SummaryGridColumn(new FrequencyGridColumn(thisRowsValues.Count, _filtereddata.Count)));
                        //,"Freq"));
                        //sheared ws 
                        row.Add(new SummaryGridColumn(new AverageGridColumn(shear)));
                        //"Average of " + _shearht + "m WS " + _sheartype ));

                        
                        upperWS.Clear();
                        lowerWS.Clear();
                        shear.Clear();
                    }
                    workgrid.Add(row);
                 }

                return workgrid;
            }
            catch
            {
                throw;
            }
        }


        protected override List<XbyYCoincidentRow> FilterRows(int axiselement)
        {

            WindDirectionAxis windroseaxis = (WindDirectionAxis)_axis;
                if (axiselement == 0)
                {
                     var result = from p in _filtereddata.AsEnumerable()
                                 where p.WD >= windroseaxis.GetRange(axiselement,Range.start) | p.WD < windroseaxis.GetRange(axiselement,Range.end)
                                 select p;
                    
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count + " range start "
                    //    + windroseaxis.GetRangeStart(axiselement) + " end " + windroseaxis.GetRangeEnd(axiselement));
                    
                    return result.ToList();
                }
                else
                {

                    var  result = from p in _filtereddata.AsEnumerable()
                                 where p.WD >= windroseaxis.GetRange(axiselement,Range.start) && p.WD < windroseaxis.GetRange(axiselement,Range.end)
                                 select p;
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count + " range start "
                    //    + windroseaxis.GetRangeStart(axiselement) + " end " + windroseaxis.GetRangeEnd(axiselement));

                    return result.ToList();
                }
                
               
            
        }
    }
}
