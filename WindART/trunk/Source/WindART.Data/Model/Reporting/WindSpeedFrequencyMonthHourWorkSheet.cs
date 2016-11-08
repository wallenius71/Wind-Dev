using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GemBox.Spreadsheet;

namespace WindART
{
    public class WindSpeedFrequencyMonthHourWorkSheet:AbstractExcelWorkSheet 
    {
        
        SummaryGrid _monthgrid;
        SummaryGrid _hourgrid;
        ExcelFile _ef;


        public WindSpeedFrequencyMonthHourWorkSheet(
            ISessionColumnCollection collection,
            DataTable data,
            ExcelFile ef,
            SessionColumnType sheartype)
        {
            
            _ef = ef;
            _monthgrid = new SummaryGrid(collection, data, sheartype,new MonthAxis(), SummaryType.Month);
            _hourgrid = new SummaryGrid(collection, data,sheartype, new HourAxis(), SummaryType.Hour);
            
        }

        public WindSpeedFrequencyMonthHourWorkSheet(
            ISessionColumnCollection collection,
            DataTable data,
            ExcelFile ef,
            SessionColumnType sheartype,
            int shearidx)
        {

            _ef = ef;
            _monthgrid = new SummaryGrid(collection, data, sheartype,shearidx, new MonthAxis(), SummaryType.Month);
            _hourgrid = new SummaryGrid(collection, data, sheartype,shearidx, new HourAxis(), SummaryType.Hour);

        }
            
        public override ExcelWorksheet  BuildWorkSheet()
        {
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " building wind speed freq wksht ");
            List<List<SummaryGridColumn>> monthgrid = _monthgrid.CreateGrid();
            List<List<SummaryGridColumn>> hourgrid = _hourgrid.CreateGrid();

            //OnWriteNotification(DateTime.Now + " " + this.ToString() + " monthgrid in ws freq " + monthgrid.Count);
            ExcelWorksheet thisSheet = _ef.Worksheets.Add("Summary by Month & Hour");
            int i;
            double workval;
            object useval;
            //month
            for (i = 0; i < monthgrid.Count; i++)
            {
                
                  // OnWriteNotification (DateTime.Now  + " " + this.ToString () + " monthgrid " + i + " is not null");
                    for (int j = 0; j < monthgrid[0].Count; j++)
                    {
                        if (double.TryParse(monthgrid[i][j].Value.ToString(), out workval))
                        {
                            if (Double.IsNaN (workval))
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
                            //OnWriteNotification(DateTime.Now + " " + this.ToString() + " does not parse as double " + monthgrid[i][j].Value);
                            useval = monthgrid[i][j].Value;
                        }
                        
                        
                        thisSheet.Cells[i + 1, j + 1].Value = useval;
                        thisSheet.Cells[i + 1, j + 1].Style.WrapText = true;
                    }
                
            }
            //hour grid
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " hour grid rows  " + hourgrid.Count);
            for (int a = i + 3; a < hourgrid.Count + (i + 3); a++)
            {

               // OnWriteNotification(DateTime.Now + " " + this.ToString() + "  a in hour grid " + (a - (i + 3)));
                    for (int b = 0; b < hourgrid[0].Count; b++)
                    {
                        if (double.TryParse(hourgrid[a - (i + 3)][b].Value.ToString(), out workval))
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
                            //OnWriteNotification(DateTime.Now + " " + this.ToString() + " does not parse as double " + hourgrid[a - (i + 3)][b].Value);
                            useval = hourgrid[a - (i + 3)][b].Value;
                        }
                        
                                                   

                        thisSheet.Cells[a + 1, b + 1].Value = useval;
                        thisSheet.Cells[a + 1, b + 1].Style.WrapText = true;
                    }
                
            }
            OnWriteNotification(DateTime.Now + " " + this.ToString() + " finished with ws freq wksht");
            return thisSheet;
        }
    }
}
