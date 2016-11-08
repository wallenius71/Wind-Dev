using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART;
using WindART.Properties;

namespace WindART
{
    public class Alpha:AbstractAlpha 
    {
        public delegate void AlphaUpdatedEventHandler(AbstractAlpha alpha);
        public event AlphaUpdatedEventHandler AlphaUpdated;

        public Alpha(DataTable data, ISessionColumn upperWS, ISessionColumn lowerWS, IAxis xAxis, IAxis yAxis)
        {
            _data = data;
            _upperws = upperWS;
            _lowerws = lowerWS;
            _xAxis = xAxis;
            _yAxis = yAxis;
            _dataSetStart = DateTime.Parse(_data.Rows[0][Settings.Default.TimeStampName].ToString());
            
        }

        public Alpha(List<DataRow> coincident, ISessionColumn upperWS, ISessionColumn lowerWS, IAxis xAxis, IAxis yAxis)
        {//overload allows coincident data to be directly passed in if it has already been calculated
            _coincident = new List<DataRow>();
            _coincident = coincident;
            _upperws = upperWS;
            _lowerws = lowerWS;
            _xAxis = xAxis;
            _yAxis = yAxis;
            _dataSetStart = DateTime.Parse(_coincident[0][Settings.Default.TimeStampName].ToString());

        }
        public override void CalculateAlpha()
        {

            
            int x = _xAxis.AxisValues.Length;
            int y = _yAxis.AxisValues.Length;
            UpperAvg = new double[x, y];
            UpperAvgCount = new int[x, y];
            LowerAvg = new double[x, y];
            LowerAvgCount = new int[x, y];
            Alpha = new double[x, y];
            AlphaCount = new int[x, y];
            
            
            //list to hold avg values for each sensor grid at each x and y bin
            List<double> upperWsVals = new List<double>();
            List<double> lowerWsVals = new List<double>();
            //get coincident rows


            //calculate coincident data if coincident set not passed 
            if (_coincident ==null)
                _coincident=new ShearCoincidentRows (new List<int>{_upperws.ColIndex ,_lowerws.ColIndex },_data).GetRows ();

            //Console.WriteLine("coincident data rows " + _coincident.Count);
           

            int upperWSIdx = _upperws.ColIndex;
            int lowerWSIdx = _lowerws.ColIndex;

            
            
            double upperWsHeight=_upperws.getConfigAtDate (_dataSetStart ).Height;
            double lowerWSHeight=_lowerws.getConfigAtDate (_dataSetStart).Height;

            int LowerXSessionColIndex=_xAxis.SessionColIndex ;
            int LowerYSessionColIndex=_yAxis.SessionColIndex ;

            //Console.WriteLine("Alpha:found all ws in config");
            
            //the defualt sissioncolindex for  ws axis is upper comp, 
            //but if we have a ws axis for either the x or y axis 
            //then we need to substite the lower ws comp for the colindex
            if (_xAxis.AxisType == AxisType.WS) LowerXSessionColIndex = lowerWSIdx;
            if (_yAxis.AxisType == AxisType.WS) LowerYSessionColIndex = lowerWSIdx;


            AbstractRowFilter DataFilter = new RowFilterFactory().CreateRowFilter(_xAxis, _yAxis, _coincident);
            
            //Console.WriteLine(upperWSIdx + " " + lowerWSIdx);
            for (int a = 0; a < x; a++)
            {
                //comment
                for (int j = 0; j < y; j++)
                {
                    //get collection of doubles for this axis set
                    //xelement, yelement, xrow--col for x axis filter, yrow-- col for y axis filter,
                    //col to return
                    
                    upperWsVals = DataFilter.Filter(a, j, _xAxis.SessionColIndex, _yAxis.SessionColIndex, upperWSIdx);
                    lowerWsVals = DataFilter.Filter(a, j, _xAxis.SessionColIndex, _yAxis.SessionColIndex, lowerWSIdx);

                    //Console.WriteLine("assigned values for " + a + ", " + j);
                    if (upperWsVals == null || lowerWsVals == null) return;

                    if (upperWsVals.Count > 0 )
                    {
                       // Console.WriteLine("upper ws count " + upperWsVals.Count);
                        UpperAvgCount[a, j] = upperWsVals.Count;
                        UpperAvg[a, j] = upperWsVals.Average();

                    }
                    else
                    {
                        UpperAvg[a, j] = 0;
                        
                    }

                    if (lowerWsVals.Count > 0)
                    {
                       // Console.WriteLine("lower ws count " + lowerWsVals.Count);
                        LowerAvgCount[a, j] = lowerWsVals.Count;
                        LowerAvg[a, j] = lowerWsVals.Average();
                    }
                    else
                    {
                        
                        LowerAvg[a, j] = 0;
                    }

                    upperWsVals.Clear();
                    lowerWsVals.Clear();

                    
                }

            }
            
            //calculate each alpha value and assign to  a single grid 
           //fill alpha table
           

            for (int a = 0; a < x; a++)
            {
                for (int j = 0; j < y; j++)
                {
                    Console.WriteLine(a + " " + j + " upper avg" + UpperAvg[a, j].ToString() + " loweravg " + LowerAvg[a, j].ToString()
                        + " upper ht " + upperWsHeight.ToString() + " lower ht " + lowerWSHeight.ToString());
                    if (LowerAvg[a,j] > 0 && UpperAvg[a,j] >0)
                    {
                        Alpha[a, j] = Math.Log(UpperAvg[a, j] / LowerAvg[a, j])
                            / Math.Log(upperWsHeight / lowerWSHeight);
                    }
                   
                    else
                    {
                        Alpha[a, j] = 0;
                    }

                }
            }

            if (AlphaUpdated !=null)
            AlphaUpdated(this);

        }
        public override string Name
        {
            get {
                if (string.IsNullOrEmpty (_name))
                {
                    string xBin = Xaxis.BinWidth.ToString();
                    string yBin = Yaxis.BinWidth.ToString();
                    if (xBin == "0" || (_xAxis.AxisType ==AxisType.Hour | _xAxis.AxisType ==AxisType.Month)) xBin = string.Empty; else xBin = "_" + xBin;
                    if (yBin == "0" || (_yAxis.AxisType ==AxisType.Hour | _yAxis.AxisType ==AxisType.Month)) yBin = string.Empty; else yBin = "_" + yBin;

                    _name=SourceDataSet + " " + Xaxis.AxisType.ToString() + xBin + " by " +  Yaxis.AxisType .ToString() + yBin ;
                }
                return _name;
            }
        }
    }
}
