using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class SummaryGrid:AbstractSummaryGrid 
    {
        
            public SummaryGrid (ISessionColumnCollection collection, DataTable data, SessionColumnType sheartype
                , IAxis axis,SummaryType summarytype)
            {
                _collection = collection;
                _rowdata = data.AsDataView();
                _axis =axis;
                _sheartype = sheartype;
                _summarytype = summarytype;
                setCols();
                FilterData();

            }
            

            public override List<List<SummaryGridColumn >> CreateGrid()
            {
                
                    ICreateGridAlgorithm  gridalg = null;
                    switch (_summarytype )
                    {
                        case SummaryType .Month:
                            gridalg = new CreateMonthSummaryGrid(_filteredrows, (MonthAxis)_axis,
                                _upperwsht, _lowerwsht, _shearht, _sheartype);
                            break;
                        case SummaryType.Hour:
                            gridalg = new CreateHourSummaryGrid(_filteredrows, (HourAxis)_axis,
                                _upperwsht, _lowerwsht, _shearht, _sheartype);
                            break;
                        case SummaryType.WD:
                            gridalg = new CreateWindDirectionSummaryGrid(_filteredrows, (WindDirectionAxis)_axis,
                                _upperwsht, _lowerwsht, _shearht, _sheartype);
                            break;
                        case SummaryType.WS:
                            gridalg = new CreateWindSpeedSummaryGrid(_filteredrows, (WindSpeedAxis)_axis,
                                _upperwsht, _lowerwsht, _shearht, _sheartype);
                            break;
                        case SummaryType .WDRose:
                            gridalg = new CreateWindRoseSummaryGrid(_filteredrows, (WindDirectionAxis)_axis,
                                _upperwsht, _lowerwsht, _shearht, _sheartype);
                            break;
                        default:
                            gridalg= null;
                            break;
                            
                      
                    }
                    if (gridalg != null)
                    {
                        return gridalg.CreateGrid();
                    }
                    else
                    {
                        return null;
                    }
                   
                

            }

        }
    
}
