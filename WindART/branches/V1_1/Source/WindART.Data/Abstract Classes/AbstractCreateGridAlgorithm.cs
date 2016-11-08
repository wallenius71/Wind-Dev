using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractCreateGridAlgorithm:ICreateGridAlgorithm 
    {
        protected List<XbyYCoincidentRow> _filtereddata;
        protected double _upperwsht;
        protected double _lowerwsht;
        protected double _shearht;
        protected IAxis _axis;
        protected SessionColumnType _sheartype;


        public virtual List<List<SummaryGridColumn>> CreateGrid()
        {
            
                List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>(_axis.AxisValues.Length);
                for (int i = 0; i < _axis.AxisValues.Length+1; i++)
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
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(_axis.AxisType  + axisheader )));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Derived Count")));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Upper Count")));
                        row.Add(new SummaryGridColumn(new ExplicitValueGridColumn("Lower Count")));
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
                        thisRowsValues = FilterRows(i-1);
                        //fill ws lists
                        List<double> upperWS = new List<double>(_filtereddata.Count);
                        List<double> lowerWS = new List<double>(_filtereddata.Count);
                        List<double> shear = new List<double>(_filtereddata.Count);

                        if (_axis.AxisType == AxisType.WS)
                        {
                            IAxis wsaxis = (WindSpeedAxis)_axis;
                            double start = wsaxis.GetRange(i - 1, Range.start);
                            double end = wsaxis.GetRange(i - 1, Range.end);

                            //use upper ws for counts and frequencies
                            thisRowsValues  = thisRowsValues.Where(c => c.UpperWS >= start & c.UpperWS < end).ToList();

                            if (thisRowsValues.Count>0)
                            {
                                upperWS = thisRowsValues.Select(c => c.UpperWS).DefaultIfEmpty().ToList();
                                lowerWS = thisRowsValues.Where(c => c.LowerWS >= start & c.LowerWS < end).Select(c => c.LowerWS).DefaultIfEmpty().ToList();
                                shear = thisRowsValues.Where(c => c.Shear >= start & c.Shear < end).Select(c => c.Shear).DefaultIfEmpty().ToList();
                            }
                            

                        }

                        else
                        {
                            upperWS = thisRowsValues.Where(c => c.UpperWS >= 0).Select(c => c.UpperWS).ToList();
                            lowerWS = thisRowsValues.Where(c => c.LowerWS >= 0).Select(c => c.LowerWS).ToList();
                            shear = thisRowsValues.Where(c => c.Shear >= 0).Select(c => c.Shear).ToList();
                        }
                        //left axis
                        row.Add(new SummaryGridColumn(new AxisValueGridColumn(_axis, i-1)));
                        // count
                        row.Add(new SummaryGridColumn(new CountGridColumn(shear.ConvertAll(c => (object)c))));
                        // count
                        row.Add(new SummaryGridColumn(new CountGridColumn(upperWS.ConvertAll(c => (object)c))));
                        // count
                        row.Add(new SummaryGridColumn(new CountGridColumn(lowerWS.ConvertAll(c => (object)c))));

                        //prevent div by 0

                        upperWS.Add(0);
                        lowerWS.Add(0);
                        shear.Add(0);

                        //derived frequency
                        row.Add(new SummaryGridColumn(new FrequencyGridColumn(shear.Count, _filtereddata.Count)));
                        //upper frequency
                        row.Add(new SummaryGridColumn(new FrequencyGridColumn(upperWS.Count, _filtereddata.Count)));
                        //lower frequency
                        row.Add(new SummaryGridColumn(new FrequencyGridColumn(lowerWS.Count, _filtereddata.Count)));
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
        protected abstract List<XbyYCoincidentRow> FilterRows(int axiselement);

        
    }
}
