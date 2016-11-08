using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;

namespace WindART
{
    public abstract class AbstractExcelReport
    {
        public abstract void CreateReport(string outputpath);
        public bool RunIndividualGrids { get; set; }
       
        protected void _ef_LimitNear(Object sender,  LimitEventArgs  e)
        {

            e.WriteWarningWorksheet = false;

        }
       
    }
}
