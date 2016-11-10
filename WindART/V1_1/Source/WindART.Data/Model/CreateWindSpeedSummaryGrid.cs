using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class CreateWindSpeedSummaryGrid : AbstractCreateGridAlgorithm
    {
        
        //constructor
        public CreateWindSpeedSummaryGrid(List<XbyYCoincidentRow> filtereddata, WindSpeedAxis axis,
            double upperwsht, double lowerwsht, double shearht, SessionColumnType sheartype)
        {
            _axis = axis;
            _filtereddata = filtereddata;
            _upperwsht = upperwsht;
            _lowerwsht = lowerwsht;
            _shearht = shearht;
            _sheartype = sheartype;
        }
        //methods
        
        protected override List<XbyYCoincidentRow> FilterRows(int axiselement)
        {

            WindSpeedAxis wsaxis = (WindSpeedAxis)_axis;

                    var result = from p in _filtereddata.AsEnumerable()
                                 where p.UpperWS  >= wsaxis.GetRangeStart(axiselement) 
                                 && p.UpperWS  < wsaxis.GetRangeEnd(axiselement)
                                 select p;
                    int resultCount = result.ToList().Count;
            
            Console.WriteLine("element " + axiselement + " count " + resultCount + " range start "
            + wsaxis.GetRangeStart(axiselement) + " end " + wsaxis.GetRangeEnd(axiselement));

                    if (resultCount==0)
                    {
                        XbyYCoincidentRow thisDummy = new XbyYCoincidentRow();
                        thisDummy.Date = default(DateTime);
                        thisDummy.UpperWS = 0;
                        return new List<XbyYCoincidentRow >(){thisDummy };
                    }
                    
                    return result.ToList();
                    
                
                
            
        }
    }
}
