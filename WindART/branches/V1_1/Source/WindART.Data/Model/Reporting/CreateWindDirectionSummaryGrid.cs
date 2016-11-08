using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class CreateWindDirectionSummaryGrid:AbstractCreateGridAlgorithm 
    {
        

        
        public CreateWindDirectionSummaryGrid(List<XbyYCoincidentRow> filtereddata, WindDirectionAxis axis,
            double upperwsht, double lowerwsht, double shearht, SessionColumnType sheartype)
        {
            _axis = axis;
            _filtereddata = filtereddata;
            _upperwsht = upperwsht;
            _lowerwsht = lowerwsht;
            _shearht = shearht;
            _sheartype = sheartype;
        }
       


        protected override List<XbyYCoincidentRow > FilterRows(int axiselement)
        {
            try
            {
                WindDirectionAxis wdaxis = (WindDirectionAxis)_axis;
                if (axiselement == 0)
                {
                     var result = from p in _filtereddata.AsEnumerable()
                                 where (p.WD >= wdaxis.GetRange(axiselement,Range.start) && p.WD <=360)
                                 | (p.WD>=0 &&  p.WD < wdaxis.GetRange(axiselement,Range.end))
                                 select p;
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count + " range start "
                    //    + wdaxis.GetRangeStart(axiselement) + " end " + wdaxis.GetRangeEnd(axiselement));
                    
                    return result.ToList();
                }
                else
                {

                    var  result = from p in _filtereddata.AsEnumerable()
                                 where p.WD >= wdaxis.GetRange(axiselement, Range.start) && p.WD < wdaxis.GetRange(axiselement,Range.end)
                                 select p;
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count + " range start "
                    //    + _axis.GetRangeStart(axiselement) + " end " + _axis.GetRangeEnd(axiselement));

                    return result.ToList();
                }
                
               
            }
            catch
            {
                throw;
            }
        }
        public override List<List<SummaryGridColumn>> CreateGrid()
        {

            List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>(_axis.AxisValues.Length);
            for (int i = 0; i < _axis.AxisValues.Length + 1; i++)
            {
                //create a row and add it to the grid

                List<SummaryGridColumn> row = new List<SummaryGridColumn>();
                string axisheader = " bin ";
                if (_axis.AxisType == AxisType.WD || _axis.AxisType == AxisType.WS)
                {
                    axisheader = " bin " + _axis.Incrementor + " deg";
                }


                //column headings
                if (i == 0)
                {
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(_axis.AxisType + axisheader)));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Count")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("LowerCount")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("UpperCount")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Derived Freq")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Upper Freq")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Lower Freq")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Average of " + _upperwsht + "m WS Comp")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Average of  " + _lowerwsht + "m WS Comp")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Average of " + _shearht + "m " + _sheartype)));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Alpha (" + _lowerwsht + "m to " + _upperwsht + "m)")));
                    row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Alpha " + _upperwsht + "m to " + _shearht + "m)")));
                }
                else
                {
                    //filter data for this axis element
                    List<XbyYCoincidentRow> thisRowsValues = new List<XbyYCoincidentRow>();

                    //TODO Factor out this to a factory pattern. If its a wind speed axis then filter each avg column by itself
                    //to provide source numbers for averaging 
                    //*****

                    //******
                    thisRowsValues = FilterRows(i - 1);
                    //fill ws lists
                    List<double> upperWS = new List<double>(_filtereddata.Count);
                    List<double> lowerWS = new List<double>(_filtereddata.Count);
                    List<double> shear = new List<double>(_filtereddata.Count);

                   
                        upperWS = thisRowsValues.Where(c => c.UpperWS >=0).Select(c=>c.UpperWS ).ToList();
                        lowerWS = thisRowsValues.Where(c => c.LowerWS >=0).Select(c=>c.LowerWS ).ToList();
                        shear = thisRowsValues.Where(c => c.Shear>=0).Select(c=>c.Shear ).ToList();
                   
                    //left axis
                    row.Add(new SummaryGridColumn(new AxisValueGridColumn(_axis, i - 1)));
                    // shear count
                    row.Add(new SummaryGridColumn(new CountGridColumn(shear.ConvertAll(c => (object)c))));
                    //lower count 
                    row.Add(new SummaryGridColumn(new CountGridColumn(lowerWS.ConvertAll(c => (object)c))));
                    //upper count 
                    row.Add(new SummaryGridColumn(new CountGridColumn(upperWS.ConvertAll(c => (object)c))));
                    //derived frequency
                    row.Add(new SummaryGridColumn(new FrequencyGridColumn(shear.Count, _filtereddata.Where(c => c.Shear >= 0).Count())));
                    //upper frequency
                    row.Add(new SummaryGridColumn(new FrequencyGridColumn(upperWS.Count, _filtereddata.Where(c => c.UpperWS >= 0).Count())));
                    //lower freq
                    row.Add(new SummaryGridColumn(new FrequencyGridColumn(lowerWS.Count, _filtereddata.Where(c => c.LowerWS >= 0).Count())));
                    //upper ws 
                    row.Add(new SummaryGridColumn(new AverageGridColumn(upperWS)));
                    //lower ws
                    row.Add(new SummaryGridColumn(new AverageGridColumn(lowerWS)));
                    //sheared ws 
                    row.Add(new SummaryGridColumn(new AverageGridColumn(shear)));
                    //lower alpha
                    row.Add(new SummaryGridColumn(new AlphaGridColumn(upperWS.Average(), lowerWS.Average(),
                        _upperwsht, _lowerwsht)));
                    //"upper alpha
                    row.Add(new SummaryGridColumn(new AlphaGridColumn(shear.Average(), upperWS.Average(),
                        _shearht, _upperwsht)));



                    upperWS.Clear();
                    lowerWS.Clear();
                    shear.Clear();
                }
                workgrid.Add(row);
            }
            return workgrid;


        }
    }
}
