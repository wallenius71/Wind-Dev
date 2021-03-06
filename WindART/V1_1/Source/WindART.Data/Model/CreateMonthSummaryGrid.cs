﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class CreateMonthSummaryGrid:AbstractCreateGridAlgorithm 
    {
        

        //constructor
        public CreateMonthSummaryGrid(List<XbyYCoincidentRow> filtereddata, MonthAxis axis,
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
            
                    var result = from p in _filtereddata.AsEnumerable()
                                 where p.Date.Month ==_axis.AxisValues [axiselement]
                                 select p;
                    //Console.WriteLine("element " + axiselement + " count " + result.ToList().Count +
                    //    " Month=" + _axis.AxisValues[axiselement]);


                    if (result.Count() == 0)
                    {
                        return new List<XbyYCoincidentRow>() { new XbyYCoincidentRow() };
                    }
                    else
                    {
                        return result.ToList();
                    }
                


            
        }
    }
}
