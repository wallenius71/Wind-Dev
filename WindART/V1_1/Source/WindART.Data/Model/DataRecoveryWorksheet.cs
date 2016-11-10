using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;
using System.Data;

namespace WindART
{
    public class DataRecoveryWorksheet : AbstractExcelWorkSheet 
    {
        private IGrid _dataRecoveryGrid;
        private ExcelFile _ef;

        public DataRecoveryWorksheet(ExcelFile ef, ISessionColumnCollection _collection, DataTable _data)
        {
            _ef = ef;
            _dataRecoveryGrid = new Grid(new CreateDataRecoveryGrid(_collection, _data.AsDataView()));
        }
        public override ExcelWorksheet BuildWorkSheet()
        {
            List<List<SummaryGridColumn>> drGrid = _dataRecoveryGrid.CreateGrid();
            

            ExcelWorksheet thisSheet = _ef.Worksheets.Add("Data Recovery");
            int i;
            Console.WriteLine(" rows in datagrid to write to excel " + drGrid.Count);
            for (i = 0; i < drGrid.Count; i++)
            {
                Console.WriteLine(" writing row " + i + " to excel sheet");
                Console.WriteLine(" columns in dgrid to write to excel " + drGrid[0].Count);
                
                    for (int j = 0; j < drGrid[i].Count; j++)
                    {
                        Console.WriteLine("attempting to write col " + j + ",  row " + i);
                        thisSheet.Cells[i + 1, j + 1].Value = drGrid[i][j].Value;
                        thisSheet.Cells[i + 1, j + 1].Style.WrapText = true;

                    }
                
            }
            return thisSheet;
        }
    }
}
