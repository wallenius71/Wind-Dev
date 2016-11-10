using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GemBox.Spreadsheet;

namespace WindART
{
    public class BulkShearStationSummary : AbstractExcelReport
    {
        
        WindSpeedFrequencyMonthHourWorkSheet _monthHourWorksht;
        WindSpeedFrequencyWDWSWorkSheet _wSWDworkSht;
        WindRoseWorkSheet _wroseWksht;

        ExcelFile _ef;



        public BulkShearStationSummary(
            ISessionColumnCollection collection,
            DataView data,
            double wdbin,
            double wdrosebin,
            double wsbin
           )
        {
            _ef = new ExcelFile();
            _ef.LimitNear += new LimitEventHandler(_ef_LimitNear);

            //original data
            SessionColumnType sheartype = SessionColumnType.WSAvgBulkShear;
            _monthHourWorksht = new WindSpeedFrequencyMonthHourWorkSheet(collection, data.Table, _ef,sheartype );
            _wSWDworkSht = new WindSpeedFrequencyWDWSWorkSheet(collection, data, wdbin, wsbin, _ef,sheartype );
            _wroseWksht = new WindRoseWorkSheet(collection, data, wdrosebin, _ef,sheartype );
            //datarecovery worksheet

            //built in order they will appear in work book
            
            _monthHourWorksht.BuildWorkSheet();
            _wSWDworkSht.BuildWorkSheet();
            _wroseWksht.BuildWorkSheet();
        }


        public override void CreateReport(string outputpath)
        {
            try
            {
                _ef.SaveXlsx(outputpath);
            }
            catch
            {
                throw;
            }

        }
    }
}