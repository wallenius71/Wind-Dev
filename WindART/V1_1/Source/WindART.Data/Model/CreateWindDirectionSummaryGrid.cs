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
                                 where p.WD >= wdaxis.GetRangeStart(axiselement) | p.WD < wdaxis.GetRangeEnd(axiselement)
                                 select p;
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count + " range start "
                    //    + wdaxis.GetRangeStart(axiselement) + " end " + wdaxis.GetRangeEnd(axiselement));
                    
                    return result.ToList();
                }
                else
                {

                    var  result = from p in _filtereddata.AsEnumerable()
                                 where p.WD >= wdaxis.GetRangeStart(axiselement) && p.WD < wdaxis.GetRangeEnd(axiselement)
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
    }
}
