using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;
using System.Data;


namespace WindART
{
    public class WindRoseWorkSheet : AbstractExcelWorkSheet
    {

        SummaryGrid _wRoseGrid;
        ExcelFile _ef;
        
        public WindRoseWorkSheet(
            ISessionColumnCollection collection,
            DataView data, 
            double wdbinwidth,
            ExcelFile ef,
            SessionColumnType sheartype)
        {
            _ef = ef;
            _wRoseGrid = new SummaryGrid(collection, data.Table, sheartype, 
                new WindDirectionAxis(wdbinwidth ), SummaryType.WDRose );
           
        }
        public override ExcelWorksheet BuildWorkSheet()
        {
            List<List<SummaryGridColumn>> wrosegrid = _wRoseGrid.CreateGrid();
            
            ExcelWorksheet thisSheet = _ef.Worksheets.Add("Wind Rose");
            int i;
            for (i = 0; i < wrosegrid.Count; i++)
            {
                for (int j = 0; j < wrosegrid[0].Count; j++)
                {
                    thisSheet.Cells[i + 1, j + 1].Value = wrosegrid[i][j].Value;
                    thisSheet.Cells[i + 1, j + 1].Style.WrapText = true;
                }
            }

            
            return thisSheet;
        }
    }
}
