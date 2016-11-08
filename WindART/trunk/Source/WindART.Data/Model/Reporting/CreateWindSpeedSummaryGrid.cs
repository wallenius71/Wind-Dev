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

            //WindSpeedAxis wsaxis = (WindSpeedAxis)_axis;
            //double startRange = wsaxis.GetRange(axiselement, Range.start);
            //double EndRange = wsaxis.GetRange(axiselement, Range.end);

            //List<XbyYCoincidentRow> work = new List<XbyYCoincidentRow>();

            //var result = from p in _filtereddata.AsEnumerable()
            //             where p.UpperWS >= startRange
            //             && p.UpperWS < EndRange
            //             select p.UpperWS;

            //        int resultCount = result.ToList().Count;
              
             

            //var Lowerresult = from p in _filtereddata.AsEnumerable()
            //            where p.LowerWS  >= startRange 
            //            && p.LowerWS < EndRange
            //            select p.LowerWS ;

            //var ShearResult = from p in _filtereddata.AsEnumerable()
            //                  where p.Shear >= startRange 
            //                  && p.Shear<EndRange
            //                  select p.Shear;
            
            ////Console.WriteLine("element " + axiselement + " count " + resultCount + " range start "
            ////+ wsaxis.GetRange(axiselement,Range.start) + " end " + wsaxis.GetRange(axiselement,Range.end));

            //        if (resultCount==0)
            //        {
            //            XbyYCoincidentRow thisDummy = new XbyYCoincidentRow();
            //            thisDummy.Date = default(DateTime);
            //            thisDummy.UpperWS = 0;
            //            return new List<XbyYCoincidentRow >(){thisDummy };
            //        }


            return _filtereddata;  
                
                
            
        }
        
    }
}
