using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;

namespace WindART
{
    public class MonthbyYearShearWorkSheet:AbstractExcelWorkSheet 
    {
        IGrid _upperAvgGrid;
        IGrid _lowerAvgGrid;
        IGrid _alphaGrid;

        public MonthbyYearShearWorkSheet(ShearCalculationGridCollection  gridcollection)
        {
            //prepare the grids 
            _upperAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.UpperAvg));
            _lowerAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.LowerAvg ));
            _alphaGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.Alpha ));
        }
        public override ExcelWorksheet  BuildWorkSheet()
        {
            throw new NotImplementedException();
        }
    }
}
