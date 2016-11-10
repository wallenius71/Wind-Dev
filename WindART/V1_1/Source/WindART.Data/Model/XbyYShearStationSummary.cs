using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GemBox.Spreadsheet;
using System.Windows.Forms;

namespace WindART
{
    public class XbyYShearStationSummary : AbstractExcelReport
    {
        MonthbyHourShearWorkSheet _shearWksht;
        WindSpeedFrequencyMonthHourWorkSheet _monthHourWorksht;
        WindSpeedFrequencyWDWSWorkSheet _wSWDworkSht;
        WindRoseWorkSheet _wroseWksht;
        DataRecoveryWorksheet _dataRecoveryWksht;
        

        ISessionColumnCollection _collection;
            DataView _data;
            double _wdbin;
            double _wdrosebin;
            double _wsbin;
            ShearCalculationGridCollection _gridcollection;

        ExcelFile _ef;



        public XbyYShearStationSummary(
            ISessionColumnCollection collection,
            DataView data,
            double wdbin,
            double wdrosebin,
            double wsbin,
            ShearCalculationGridCollection gridcollection
           )
        {
            _collection = collection;
             _data=data;
            _wdbin=wdbin;
            _wdrosebin=wdrosebin;
            _wsbin=wsbin;
            _gridcollection=gridcollection ;

            _ef = new ExcelFile();
            _ef.LimitNear +=new LimitEventHandler(_ef_LimitNear);
            
            //original data
            _shearWksht = new MonthbyHourShearWorkSheet(gridcollection, _ef);
            _monthHourWorksht = new WindSpeedFrequencyMonthHourWorkSheet(collection, data.Table, _ef, SessionColumnType.WSAvgShear);
            _wSWDworkSht = new WindSpeedFrequencyWDWSWorkSheet(collection, data, wdbin, wsbin, _ef, SessionColumnType.WSAvgShear);
            _wroseWksht = new WindRoseWorkSheet(collection, data, wdrosebin, _ef, SessionColumnType.WSAvgShear);
            _dataRecoveryWksht = new DataRecoveryWorksheet(_ef, collection, data.Table);

            //built in order they will appear in work book
            _shearWksht.BuildWorkSheet();
            _monthHourWorksht.BuildWorkSheet();
            _wSWDworkSht.BuildWorkSheet();
            _wroseWksht.BuildWorkSheet();
            _dataRecoveryWksht.BuildWorkSheet();

        }

        public XbyYShearStationSummary(
            ISessionColumnCollection collection,
            DataView data,
            double wdbin,
            double wdrosebin,
            double wsbin,
            ShearCalculationGridCollection gridcollection,
            List<StationSummarySheetType > whatToRun
           )
        {
            _collection = collection;
            _data = data;
            _wdbin = wdbin;
            _wdrosebin = wdrosebin;
            _wsbin = wsbin;
            _gridcollection = gridcollection;

            _ef = new ExcelFile();
            _ef.LimitNear += new LimitEventHandler(_ef_LimitNear);

                foreach (StationSummarySheetType  s in whatToRun )
                {
                    try
                    {
                        RunReports[s].Invoke();
                    }
                    catch
                    {

                    }
                }
           
        }

        Dictionary<StationSummarySheetType , Action> RunReports
        {
            get
            {
                // Return dictionary of keys and actions
                return new Dictionary<StationSummarySheetType , Action>()
                {
                    {  StationSummarySheetType .ShearGrid ,CreateShearGrids },
                    {  StationSummarySheetType.MonthHourSheet ,CreateMonthHourWorksheet  },
                    {  StationSummarySheetType.WD_WSSheet  ,CreateWDandWSWorksheet },
                    {  StationSummarySheetType .WindRoseSheet ,CreateWindRoseWorkSheet  },
                    {  StationSummarySheetType .DataRecovery  ,CreateDataRecoveryWorkSheet },
                    
                };
            }
            
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

        void CreateShearGrids()
        {
            _shearWksht = new MonthbyHourShearWorkSheet(_gridcollection, _ef);
            _shearWksht.BuildWorkSheet();
        }
        void CreateMonthHourWorksheet()
        {
            _monthHourWorksht = new WindSpeedFrequencyMonthHourWorkSheet(_collection, _data.Table, _ef, SessionColumnType.WSAvgShear);
            _monthHourWorksht.BuildWorkSheet();
        }
        void CreateWDandWSWorksheet()
        {
            _wSWDworkSht = new WindSpeedFrequencyWDWSWorkSheet(_collection, _data, _wdbin, _wsbin, _ef, SessionColumnType.WSAvgShear);
            _wSWDworkSht.BuildWorkSheet();
        }
        void CreateWindRoseWorkSheet()
        {
            _wroseWksht = new WindRoseWorkSheet(_collection, _data, _wdrosebin, _ef, SessionColumnType.WSAvgShear);
            _wroseWksht.BuildWorkSheet();
        }
        void CreateDataRecoveryWorkSheet()
        {
            _dataRecoveryWksht = new DataRecoveryWorksheet(_ef, _collection, _data.Table);
            _dataRecoveryWksht.BuildWorkSheet();
        }

        
    }
}
