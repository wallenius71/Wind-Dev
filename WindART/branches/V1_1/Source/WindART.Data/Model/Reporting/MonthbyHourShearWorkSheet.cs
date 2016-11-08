using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;
using System.IO;

namespace WindART
{
    public class MonthbyHourShearWorkSheet:AbstractExcelWorkSheet 
    {
        IGrid _upperAvgGrid;
        IGrid _upperCountGrid;
        IGrid _lowerAvgGrid;
        IGrid _lowerCountGrid;
        IGrid _alphaGrid;

        ExcelFile _ef;

              

        public MonthbyHourShearWorkSheet(AbstractAlpha   gridcollection,ExcelFile ef)
        {
            GridCollection = gridcollection;
            //prepare the grids 
            _ef = ef;

            _upperCountGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.UpperAvgCount));
            _lowerCountGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.LowerAvgCount));

           _upperAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.UpperAvg));
            _lowerAvgGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.LowerAvg ));
            _alphaGrid = new Grid(new CreateXbyYGrid(new MonthAxis(), new HourAxis(), gridcollection.Alpha ));
        }
        public MonthbyHourShearWorkSheet(AbstractAlpha  gridcollection, ExcelFile ef,IAxis x, IAxis y)
        {
            //prepare the grids 
            _ef = ef;
            _upperCountGrid = new Grid(new CreateXbyYGrid(x, y, gridcollection.UpperAvgCount));
            _upperAvgGrid = new Grid(new CreateXbyYGrid(x, y, gridcollection.UpperAvg));
            _lowerCountGrid = new Grid(new CreateXbyYGrid(x, y, gridcollection.LowerAvgCount));
            _lowerAvgGrid = new Grid(new CreateXbyYGrid(x,y, gridcollection.LowerAvg));
            _alphaGrid = new Grid(new CreateXbyYGrid(x,y, gridcollection.Alpha));
            GridCollection = gridcollection;
        }
        public override ExcelWorksheet  BuildWorkSheet()
        {
           OnWriteNotification (DateTime.Now + " " + this.ToString() +  " Build worksheet called for Month by Hour worksheet");

            uppercountgrid = _upperCountGrid.CreateGrid();
            lowercountgrid = _lowerCountGrid.CreateGrid();
            uppergrid = _upperAvgGrid.CreateGrid();
            lowergrid = _lowerAvgGrid.CreateGrid();
            alpha = _alphaGrid.CreateGrid();

            grids.AddRange (new List<List<List<SummaryGridColumn >>>(){{uppercountgrid},{lowercountgrid},{uppergrid}, {lowergrid},{alpha}});

            ExcelWorksheet thisSheet = _ef.Worksheets.Add("Shear Grids");

            int h;
            for (h = 0; h < uppercountgrid.Count; h++)
            {
                for (int j = 0; j < uppercountgrid[0].Count; j++)
                {
                    thisSheet.Cells[h + 1, j + 1].Value = uppercountgrid[h][j].Value;
                }
            } 
            int i;
            //upper ws avg grid 
            for (i = h+3; i < uppergrid.Count +(h+3); i++)
            {
                for (int j = 0; j < uppergrid[0].Count; j++)
                {
                    thisSheet.Cells[i + 1, j + 1].Value = uppergrid[i-(h+3)][j].Value;
                }
            }

            OnWriteNotification(DateTime.Now + " " + this.ToString() + " shear worksheet upper grid done");

            int b;
            for (b = i + 3; b < lowercountgrid.Count +(i+3); b++)
            {
                for (int j = 0; j < lowercountgrid[0].Count; j++)
                {
                    thisSheet.Cells[b + 1, j + 1].Value = lowercountgrid[b-(i+3)][j].Value;
                }
            }



            int a;
            //lower ws avg grid 
            for ( a = b + 3; a < lowergrid.Count + (b + 3); a++)
            {
                for (int c = 0; c < lowergrid[0].Count; c++)
                {
                    thisSheet.Cells[a + 1, c + 1].Value = lowergrid[a - (b + 3)][c].Value;
                }
            }

            OnWriteNotification(DateTime.Now + " " + this.ToString() + " shear worksheet lower grid done");
            //alpha grid 
            for (int c = a + 3; c < alpha.Count + (a + 3); c++)
            {
                for (int d = 0; d < alpha[0].Count; d++)
                {
                    thisSheet.Cells[c + 1, d + 1].Value = alpha[c - (a + 3)][d].Value;
                }
            }
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " shear worksheet alpha grid done");
            return thisSheet;
        }
    }
}
