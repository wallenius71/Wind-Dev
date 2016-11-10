using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GemBox.Spreadsheet;

namespace WindART
{
    public class WindSpeedFrequencyWDWSWorkSheet : AbstractExcelWorkSheet
    {

        SummaryGrid _WDgrid;
        SummaryGrid _WSgrid;
        ExcelFile _excelfile;


        public WindSpeedFrequencyWDWSWorkSheet(
            ISessionColumnCollection collection,
            DataView data, 
            double wdbinwidth, 
            double wsbinwidth,
            ExcelFile ef,
            SessionColumnType sheartype)
        {
             DateTime start=(DateTime)data[0][collection.DateIndex ];
            _WDgrid = new SummaryGrid(collection, data.Table , sheartype , 
                new WindDirectionAxis (wdbinwidth ), SummaryType.WD );
            _WSgrid = new SummaryGrid(collection, data.Table , sheartype,
                new WindSpeedAxis(wsbinwidth,data,collection.UpperWSComp(start ) ), SummaryType.WS );
            _excelfile = ef;
        }
        public override ExcelWorksheet BuildWorkSheet()
        {
            List<List<SummaryGridColumn>> wdgrid = _WDgrid.CreateGrid();
            Console.WriteLine("created wd grid for worksheet");

            List<List<SummaryGridColumn>> wsgrid = _WSgrid.CreateGrid();
            
            ExcelWorksheet thisSheet = _excelfile.Worksheets.Add("Summary by WD & WS");
            int i;
            for( i=0;i<wdgrid.Count;i++)
            {
                for (int j = 0; j < wdgrid[0].Count; j++)
                {
                    thisSheet.Cells[i + 1, j+1].Value = wdgrid[i][j].Value;
                    thisSheet.Cells[i + 1, j + 1].Style.WrapText = true;
                }
            }
            Console.WriteLine(" wd worksheet completed");

            for (int a = i+3; a < wsgrid.Count + (i+3); a++)
            {
                for (int b = 0; b < wsgrid[0].Count; b++)
                {
                    thisSheet.Cells[a + 1, b+1].Value = wsgrid[a - (i + 3)][ b].Value ;
                    thisSheet.Cells[a + 1, b + 1].Style.WrapText = true;
                }
            }
            return thisSheet;
        }
    }
}
