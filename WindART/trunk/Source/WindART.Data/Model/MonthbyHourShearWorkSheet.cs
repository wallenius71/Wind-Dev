using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;

namespace WindART
{
    public class MonthbyHourShearWorkSheet:AbstractExcelWorkSheet 
    {
        IGrid _upperAvgGrid;
        IGrid _lowerAvgGrid;
        IGrid _alphaGrid;
        ExcelFile _ef;

        public MonthbyHourShearWorkSheet(ShearCalculationGridCollection  gridcollection,ExcelFile ef)
        {
            //prepare the grids 
            _ef = ef;
            
            _upperAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.UpperAvg));
            _lowerAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.LowerAvg ));
            _alphaGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.Alpha ));
        }
        public override ExcelWorksheet  BuildWorkSheet()
        {
            Console.WriteLine("Build worksheet called for Month by Hour worksheet");
            List<List<SummaryGridColumn>> uppergrid = _upperAvgGrid.CreateGrid();
            List<List<SummaryGridColumn>> lowergrid = _lowerAvgGrid.CreateGrid();
            List<List<SummaryGridColumn>> alpha = _alphaGrid.CreateGrid();

            ExcelWorksheet thisSheet = _ef.Worksheets.Add("Shear Grids");
            int i;
            //upper ws avg grid 
            for (i = 0; i < uppergrid.Count; i++)
            {
                for (int j = 0; j < uppergrid[0].Count; j++)
                {
                    thisSheet.Cells[i + 1, j + 1].Value = uppergrid[i][j].Value;
                }
            }
            Console.WriteLine("shear worksheet upper grid done");
            int a;
            //lower ws avg grid 
            for ( a = i + 3; a < lowergrid.Count + (i + 3); a++)
            {
                for (int b = 0; b < lowergrid[0].Count; b++)
                {
                    thisSheet.Cells[a + 1, b + 1].Value = lowergrid[a - (i + 3)][b].Value;
                }
            }
            Console.WriteLine("shear worksheet lower grid done");
            //alpha grid 
            for (int c = a + 3; c < alpha.Count + (a + 3); c++)
            {
                for (int d = 0; d < alpha[0].Count; d++)
                {
                    thisSheet.Cells[c + 1, d + 1].Value = alpha[c - (a + 3)][d].Value;
                }
            }
            Console.WriteLine("shear worksheet alpha grid done");
            return thisSheet;
        }
    }
}
