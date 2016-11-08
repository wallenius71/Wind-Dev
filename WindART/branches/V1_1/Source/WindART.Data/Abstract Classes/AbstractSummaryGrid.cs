using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace WindART
{
    public abstract class AbstractSummaryGrid:IGrid
    {
        
        protected ISessionColumnCollection _collection;
        protected DataView _rowdata;
        protected IAxis _axis;
        protected List<XbyYCoincidentRow> _filteredrows = new List<XbyYCoincidentRow>();
        protected ISessionColumn _upperwscompcol;
        protected double _upperwsht;
        protected ISessionColumn _lowerwscompcol;
        protected double _lowerwsht;
        protected ISessionColumn _shearwscol;
        protected SessionColumnType _sheartype;
        protected double _shearht;
        protected ISessionColumn _axiscol;
        protected ISessionColumn _wdcompcol;
        protected DateTime _configstart;
        protected SummaryType _summarytype;

        protected virtual void setCols()
        {
            try
            {//figure out config dates and get comp columns 
                HeightConfigCollection configcollection = new HeightConfigCollection(_collection);
                List<IConfig> configs = configcollection.GetConfigs();

                Console.WriteLine(" doing " + _summarytype);

                if (configs.Count > 1)
                {
                    Console.WriteLine("configs found " + configs.Count);
                    foreach (IConfig c in configs)
                    {
                        Console.WriteLine("start date " + c.StartDate);
                    }
                    throw new ApplicationException("Can not process grid. System not yet set up to handle multpile configurations");
                }
                _configstart = configs[0].StartDate;
                Console.WriteLine("start date in summary grid " + _configstart);
                //wscomps
                //SortedDictionary<double, ISessionColumn> wscomps
                //    = _collection.GetColumnsByType(SessionColumnType.WSAvg, _configstart, true);

                _lowerwscompcol = _collection[_collection.LowerWSComp(_configstart)];
                _upperwscompcol = _collection[_collection.UpperWSComp(_configstart)];
                _lowerwsht = _lowerwscompcol.getConfigAtDate(_configstart).Height;
                _upperwsht =_upperwscompcol.getConfigAtDate(_configstart ).Height;

                //Console.WriteLine("Summary grid found " + wscomps.Values.Count + " ws comps");
                //wdcomp
                List<ISessionColumn> wdcomp =
                    _collection.GetColumnsByType(SessionColumnType.WDAvg);

                var compcol = from c in wdcomp.AsEnumerable()
                              where c.IsComposite
                              select c;

                _wdcompcol = compcol.ToList()[0];

                //Console.WriteLine("Summary grid found " + compcol.ToList().Count  + " wd comps");

                //shear
                if (_shearwscol == null)
                {
                    List<ISessionColumn> shearcol = _collection.GetColumnsByType(_sheartype); 
                    _shearwscol = shearcol[0];
                }
                
                
                   

                    if (_shearwscol.Configs.Count != 0)
                    {
                        _shearht = _shearwscol.getConfigAtDate(_configstart).Height;
                    }
                    else
                    {
                        SessionColumn sc = (SessionColumn)_shearwscol;
                        _shearht = sc.Height;
                    }
                
                //Console.WriteLine("Summary grid found " + _collection.GetColumnsByType(_sheartype).Count  + " shear");

            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

        }
        protected virtual void FilterData()
        {
                           
                SortedDictionary<double, int> wscols = new SortedDictionary<double, int>();

                wscols.Add(_lowerwsht  , _lowerwscompcol.ColIndex );
                wscols.Add( _upperwsht , _upperwscompcol.ColIndex );

                XbyYShearCoincidentRowCollection filteredValues = new XbyYShearCoincidentRowCollection(_collection.DateIndex,wscols, _rowdata);

                _filteredrows = filteredValues.GetRows(_wdcompcol.ColIndex,_shearwscol .ColIndex );

        }
        protected virtual void FilterData(bool CDNT)
        {

            SortedDictionary<double, int> wscols = new SortedDictionary<double, int>();

            wscols.Add(_lowerwsht, _lowerwscompcol.ColIndex);
            wscols.Add(_upperwsht, _upperwscompcol.ColIndex);

            XbyYShearCoincidentRowCollection filteredValues = new XbyYShearCoincidentRowCollection(_collection.DateIndex, wscols, _rowdata);
            if(CDNT)
                _filteredrows = filteredValues.GetRows(_wdcompcol.ColIndex, _shearwscol.ColIndex);
            if(!CDNT)
                _filteredrows = filteredValues.GetNCDNTRows(_wdcompcol.ColIndex, _shearwscol.ColIndex);

        }
        public abstract List<List<SummaryGridColumn >> CreateGrid();
       
    }
}
