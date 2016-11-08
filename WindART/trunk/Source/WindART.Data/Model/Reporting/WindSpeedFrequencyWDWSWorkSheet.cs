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

        public WindSpeedFrequencyWDWSWorkSheet(
            ISessionColumnCollection collection,
            DataView data,
            double wdbinwidth,
            double wsbinwidth,
            ExcelFile ef,
            SessionColumnType sheartype,
            int shearidx)
        {
            DateTime start = (DateTime)data[0][collection.DateIndex];
            _WDgrid = new SummaryGrid(collection, data.Table, sheartype,shearidx,new WindDirectionAxis(wdbinwidth), SummaryType.WD);
            _WSgrid = new SummaryGrid(collection, data.Table, sheartype,shearidx, new WindSpeedAxis(wsbinwidth, data, collection.UpperWSComp(start)), SummaryType.WS);
            _excelfile = ef;
        }
        public override ExcelWorksheet BuildWorkSheet()
        {
            List<List<SummaryGridColumn>> wdgrid = _WDgrid.CreateGrid();
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " created wd grid for worksheet");

            List<List<SummaryGridColumn>> wsgrid = _WSgrid.CreateGrid();
            
            ExcelWorksheet thisSheet = _excelfile.Worksheets.Add("Summary by WD & WS");
            int i;
            double workval;
            object useval;
            for( i=0;i<wdgrid.Count;i++)
            {
                for (int j = 0; j < wdgrid[0].Count; j++)
                {
                    if (double.TryParse(wdgrid[i][j].Value.ToString(), out workval))
                    {
                        if (Double.IsNaN(workval))
                        {
                            useval = 0;
                        }
                        else
                        {
                           // OnWriteNotification(DateTime.Now + " " + this.ToString() + " parsed as double is not NaN " + workval);
                            useval = (object)workval;
                        }

                    }
                    else
                    {
                       // OnWriteNotification(DateTime.Now + " " + this.ToString() + " does not parse as double " + wdgrid[i][j].Value);
                        useval = wdgrid[i][j].Value;
                    }
                        
                    thisSheet.Cells[i + 1, j+1].Value = useval;
                    thisSheet.Cells[i + 1, j + 1].Style.WrapText = true;
                }
            }
           
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " wd worksheet completed");

            for (int a = i+3; a < wsgrid.Count + (i+3); a++)
            {
                for (int b = 0; b < wsgrid[0].Count; b++)
                {
                    if (double.TryParse(wsgrid[a - (i + 3)][b].Value.ToString(), out workval))
                    {
                        if (Double.IsNaN(workval))
                        {
                            useval = 0;
                        }
                        else
                        {
                            //OnWriteNotification(DateTime.Now + " " + this.ToString() + " parsed as double is not NaN " + workval);
                            useval = (object)workval;
                        }

                    }
                    else
                    {
                        //OnWriteNotification(DateTime.Now + " " + this.ToString() + " does not parse as double " + wsgrid[a - (i + 3)][b].Value);
                        useval = wsgrid[a - (i + 3)][b].Value;
                    }
                    thisSheet.Cells[a + 1, b+1].Value = useval;
                    thisSheet.Cells[a + 1, b + 1].Style.WrapText = true;
                }
            }
            return thisSheet;
        }
    }
}
