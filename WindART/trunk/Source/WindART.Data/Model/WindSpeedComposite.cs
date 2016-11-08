using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;
using System.Windows.Threading ;

namespace WindART
{
    public class WindSpeedComposite:IComposite
    {
        #region events

        public delegate void ProgressEventHandler(string msg);
        
        public event ProgressEventHandler NewCompositeColumnAdded;
        public event ProgressEventHandler DeterminingWindSpeedCompositeValues;
        public event ProgressEventHandler CompletedWindSpeedCompositeValues;
        
        
        #endregion
        ISessionColumnCollection _collection;
        DataTable _data;
         private  enum SelectValue { A,B,both,missing};
        
        //constructor
        public WindSpeedComposite(ISessionColumnCollection collection, DataTable data)
        {
            _data = data;
            _collection = collection;
            
        }

        //methods
        public bool CalculateComposites()
        {
            //get configs
            //add columns to datatable and column collection 
            //check deadranges
            //compare sensor a aand b acorsss all configs 
            //add columns and update data through the dataview that is each config 


                SelectValue UseColumn=default (SelectValue );
                double missing = Settings.Default.MissingValue;
                
                IWindSpeedConfig A = new WindSpeedConfig();
                int AIndex;
                double AVal;


                IWindSpeedConfig B = new WindSpeedConfig();
                int BIndex;
                double BVal;
                
                ISessionColumn ACol;
                ISessionColumn BCol;
                int compositeIndex=0;

                
                IConfigCollection configCollection = new HeightConfigCollection(_collection);
                string datecol=_data.Columns [_collection .DateIndex ].ColumnName  ;

                string wdcolname=Enum.GetName (typeof(SessionColumnType ),SessionColumnType .WDAvg ) 
                    + Settings.Default.CompColName ;
                int WDIndex = _collection[wdcolname].ColIndex;
                
                double WDVal;
                
                
                double workVal=default(double);
                                
                //find each config across height changes..
                List<HeightConfig > workConfigs
                =configCollection.GetConfigs().ConvertAll<HeightConfig>(t=>(HeightConfig)t).ToList ();
                
                //Console.WriteLine("workconfigs count " + workConfigs.Count);
                //loop through each config
                foreach (HeightConfig htConfig in workConfigs )
                {   //loop each ht in config 
                    DataView configData = _data.AsDataView();
                    configData.Sort = Settings.Default.TimeStampName + " asc ";

                    //filter the data based on start and end of the current ht config 
                    string  filter=datecol + ">= '" + htConfig.StartDate.ToString() + "' and " + datecol + " <='" 
                                           + htConfig.EndDate.ToString() + "'";
                    Console.WriteLine(filter);
                    configData.RowFilter = filter;
                    
                    if (configData.Count > _data.Rows.Count)
                    {
                        throw new ApplicationException("Error filtering dataset by configuration dates");
                    }

                    int RowNum = configData.Count;

                    Console.WriteLine("datatable and filtered dataviewcounts: " + _data.Rows.Count + " " + RowNum);

                    List<double> resultArray = new List<double>(RowNum);
                    Console.WriteLine(" Heights found " + htConfig.Columns.Count);

                    //loop each height's columns in config
                    //each htconfig stores a collection of collumns with those hts 
                    foreach (KeyValuePair<double, IList<ISessionColumn>> kv in htConfig.Columns)
                    {
                        var a = from f in kv.Value.AsEnumerable()
                                where f.ColumnType == SessionColumnType.WSAvg
                                select f;
                        
                        List<ISessionColumn> thisresult = a.ToList();                   
                        //get sensor pairs that are ws avg types 
                        if (a.Count() == 2)
                        {

                            
                            //Console.WriteLine(thisresult[0].ColumnType);
                            //Console.WriteLine(thisresult[1].ColumnType);

                             ACol=thisresult[0];
                             BCol=thisresult[1];
                            
                            AIndex=ACol.ColIndex;
                            BIndex=BCol.ColIndex;

                            //get the specific config for the date of the ht config 
                            A = (IWindSpeedConfig)ACol.getConfigAtDate(htConfig.StartDate);
                            B = (IWindSpeedConfig)BCol.getConfigAtDate(htConfig.StartDate);
                            
                            //change the config in the sensor config data if there are dead zones 
                            CheckDeadRange(A, B);

                            
                            //Console.WriteLine("A Good Range: " + A.GoodSector.SectorStart + " to " + A.GoodSector.SectorEnd);
                            //Console.WriteLine("A Bad Range: " + A.ShadowSector.SectorStart + " to " + A.ShadowSector.SectorEnd);
                            //Console.WriteLine("B Good Range: " + B.GoodSector.SectorStart + " to " + B.GoodSector.SectorEnd);
                            //Console.WriteLine("B Bad Range: " + B.ShadowSector.SectorStart + " to " + B.ShadowSector.SectorEnd);

                            
                            
                        }
                        else
                        {
                            continue;
                        }

                        double height = A.Height;

                        IList<ISessionColumn> thisColumnSet = kv.Value ;
                       
                        //add column to datatable and column collection if necessary 
                        WindSpeedCompColumns wscompcols = new WindSpeedCompColumns(_collection, _data);
                        wscompcols.Add(height, thisColumnSet, htConfig.StartDate,htConfig.EndDate);
                        if(NewCompositeColumnAdded !=null)
                        NewCompositeColumnAdded("Wind Speed Composite Columns Added");


                        #region decide on value 
                        //begin calculating comps for each row
                        if(DeterminingWindSpeedCompositeValues !=null)
                        DeterminingWindSpeedCompositeValues("Assigning Wind Speed Composite Values");

                        foreach (DataRowView rowview in configData)
                        {
                            if (!Double.TryParse(rowview[AIndex].ToString(), out AVal))
                            {
                                AVal = missing;

                            }
                            else
                            {
                                if (AVal < 0)
                                {
                                    AVal = missing;
                                }
                            }
                            if (!Double.TryParse(rowview[BIndex].ToString(), out BVal))
                            {
                                BVal = missing;
                            }
                            else
                            {
                                if (BVal < 0)
                                {
                                    BVal = missing;
                                }
                            }
                            if (!double.TryParse(rowview[WDIndex].ToString(), out WDVal) || WDVal<0)
                            {
                                WDVal = missing;
                                UseColumn = SelectValue.missing;
                                goto assignvalue;
                            }

                            InSector SensorA = A.BelongsToSector(WDVal);
                            InSector SensorB = B.BelongsToSector(WDVal);

                            //in A not in B
                            if (SensorA.Equals(InSector.Not_Shadowed) &&
                              (SensorB.Equals(InSector.Shadowed) |
                               SensorB.Equals(InSector.Neither)))
                            {
                                workVal = AVal;
                                if (workVal.Equals (missing))
                                { UseColumn =SelectValue .missing ; }
                                else
                                { UseColumn =SelectValue .A ; }

                                goto assignvalue;
                            }

                            //in B not in A take b
                            if (SensorB.Equals(InSector.Not_Shadowed) &&
                               (SensorA.Equals(InSector.Shadowed) |
                                SensorA.Equals(InSector.Neither)))
                            {
                                workVal = BVal;
                                if (workVal < 0)
                                { UseColumn = SelectValue.missing; }
                                else
                                { UseColumn = SelectValue.B; }

                                goto assignvalue;
                            }

                            //in both A and B avg the two
                            if (SensorB.Equals(InSector.Not_Shadowed) &&
                               SensorA.Equals(InSector.Not_Shadowed))
                            {
                                if (AVal < 0 && BVal >= 0)
                                {
                                    UseColumn =SelectValue.B;
                                    goto assignvalue;
                                }

                                if (BVal < 0 && AVal >= 0)
                                {
                                    UseColumn = SelectValue .A;
                                    goto assignvalue;
                                }
                                //if wdval is on a boundary use the other sensor
                                //if (WDVal == A.GoodSector.SectorStart | WDVal == A.GoodSector.SectorEnd)
                                //{
                                //    UseColumn = SelectValue.B;
                                //    goto assignvalue;
                                //}

                                //if (WDVal == B.GoodSector.SectorStart | WDVal == B.GoodSector.SectorEnd)
                                //{
                                //    UseColumn = SelectValue.A;
                                //    goto assignvalue;
                                //}

                                List<double> avg = new List<double>() { BVal, AVal };
                                UseColumn = SelectValue.both;
                                workVal = avg.Average();
                                goto assignvalue;
                                
                            }
                                
                                //otherwise set to missing 
                                workVal = missing;
                                goto assignvalue;
                                
#endregion
                                #region assign value
                            assignvalue:
                                {
                                    
                                    double useval = missing;
                                    double usevalB = missing; 
                                    string heightstring = height.ToString().Replace(".", "_");
                                    StringBuilder s = new StringBuilder();
                                    s.Append(heightstring);
                                    s.Append(Enum.GetName(typeof(SessionColumnType), SessionColumnType.WSAvg));
                                    s.Append(Settings.Default.CompColName);
                                    string parentColName = s.ToString();
                                if (_collection[parentColName ]!=null)
                                    {
                                        compositeIndex = _collection[parentColName].ColIndex;
                                    }
                                    else
                                    {
                                        throw new ApplicationException(" Parent composite column " + parentColName + " not found");
                                    }
                                    switch(UseColumn )
                                    {
                                        case SelectValue .A:
                                            rowview[compositeIndex] = AVal;
                                            foreach (ISessionColumn child in ACol.ChildColumns )
                                            {
                                                

                                                StringBuilder s1 = new StringBuilder();
                                                s1.Append(heightstring);
                                                s1.Append(Enum.GetName(typeof(SessionColumnType), child.ColumnType));
                                                s1.Append(Settings.Default.CompColName);
                                                string thischildcompname = s1.ToString();

                                                if (_collection[thischildcompname]!=null)
                                                {
                                                    if (!double.TryParse(rowview[child.ColIndex].ToString(), out useval))
                                                    {
                                                        rowview[thischildcompname] = missing;
                                                    }
                                                    else
                                                    {
                                                        rowview[thischildcompname] = useval;
                                                    }
                                                }
                                                else
                                                {
                                                    throw new ApplicationException(" Child composite column " + thischildcompname + " not found");
                                                }
                                            }
                                            break;
                                        case SelectValue.B:
                                            rowview[compositeIndex] = BVal;
                                            foreach (ISessionColumn child in BCol.ChildColumns)
                                            {
                                                StringBuilder s1 = new StringBuilder();
                                                s1.Append(heightstring);
                                                s1.Append(Enum.GetName(typeof(SessionColumnType), child.ColumnType));
                                                s1.Append(Settings.Default.CompColName);
                                                string thischildcompname = s1.ToString();

                                                if (_collection[thischildcompname] != null)
                                                {
                                                    if (!double.TryParse(rowview[child.ColIndex].ToString(), out useval))
                                                    {
                                                        rowview[thischildcompname] = missing;
                                                    }
                                                    else
                                                    {
                                                        rowview[thischildcompname] = useval;
                                                    }
                                                }
                                                else
                                                {
                                                    throw new ApplicationException(" Child composite column " + thischildcompname + " not found");
                                                }
                                            }
                                            break;
                                        case SelectValue.missing:
                                            rowview[compositeIndex] = missing ;
                                            //System.Windows.Forms.MessageBox.Show(rowview[0].ToString() + " " + WDVal + " " + AVal + " " + BVal);
                                            //loop through comp cols for both a and b in case one column has more children than the other
                                            foreach (ISessionColumn child in ACol.ChildColumns)
                                            {
                                                StringBuilder s1 = new StringBuilder();
                                                s1.Append(heightstring);
                                                s1.Append(Enum.GetName(typeof(SessionColumnType), child.ColumnType));
                                                s1.Append(Settings.Default.CompColName);
                                                string thischildcompname = s1.ToString();

                                                if (_collection[thischildcompname] != null)
                                                {
                                                    rowview[thischildcompname] = missing;
                                                }
                                                else
                                                {
                                                    throw new ApplicationException(" Child composite column " + thischildcompname + " not found");
                                                }
                                            }
                                            foreach (ISessionColumn child in BCol.ChildColumns)
                                            {
                                                StringBuilder s1 = new StringBuilder();
                                                s1.Append(heightstring);
                                                s1.Append(Enum.GetName(typeof(SessionColumnType), child.ColumnType));
                                                s1.Append(Settings.Default.CompColName);
                                                string thischildcompname = s1.ToString();

                                                if (_collection[thischildcompname] != null)
                                                {
                                                    rowview[thischildcompname] = missing;
                                                }
                                                else
                                                {
                                                    throw new ApplicationException(" Child composite column " + thischildcompname + " not found");
                                                }
                                            }
                                            break;
                                        case SelectValue.both:
                                            rowview[compositeIndex] = new double[2]{AVal,BVal}.Average ();
                                            //loop through comp cols for both a and b in case one column has more children than the other
                                            foreach (ISessionColumn child in ACol.ChildColumns)
                                            {
                                                StringBuilder s1 = new StringBuilder();
                                                s1.Append(heightstring);
                                                s1.Append(Enum.GetName(typeof(SessionColumnType), child.ColumnType));
                                                s1.Append(Settings.Default.CompColName);
                                                string thischildcompname = s1.ToString();

                                                if (_collection[thischildcompname] != null)
                                                {

                                                    var result = from c in BCol.ChildColumns.AsEnumerable()
                                                                 where c.ColumnType == child.ColumnType
                                                                 select c;

                                                    List<ISessionColumn> columnfound = result.ToList();
                                                    
                                                    if (columnfound.Count == 1)
                                                    {
                                                        //Console.WriteLine(" ****************** average taken");
                                                        if (!double.TryParse(rowview[columnfound[0].ColIndex].ToString(), out useval) &
                                                            !double.TryParse(rowview[child.ColIndex].ToString(), out usevalB))
                                                        {
                                                            rowview[thischildcompname] = missing;
                                                        }
                                                        else
                                                        {
                                                            rowview[thischildcompname] = new double[2] { useval, usevalB }.Average();

                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine(" missing taken A value =" + AVal + " B Val " + BVal);
                                                        rowview[thischildcompname] = missing;

                                                    }

                                                }
                                                else
                                                {
                                                    throw new ApplicationException(" Child composite column " + thischildcompname + " not found");
                                                }


                                            }
                                            break;
                                        default:
                                            break;

                                    }
                                
                                }
                           
                        }
                        
                    //calculate recovery
                    SessionColumnCollection scc = (SessionColumnCollection)_collection;
                    scc.CalculateRecoveryRate(compositeIndex);
#endregion
                    }
                   
                }
            if(CompletedWindSpeedCompositeValues!=null)
            {
                

                //fire completed event
                CompletedWindSpeedCompositeValues("Completed Generating Wind Speed Composites");
                
            }
                return true;
            }
            
        
        private void CheckDeadRange(IWindSpeedConfig A, IWindSpeedConfig B)
        {
            
            
                //dead range=360-(2*ShadowInterval+SensorDifference)
                if(B.Orientation==0)
                { B.Orientation = 360; }
                if (A.Orientation == 0)
                {
                    A.Orientation = 360;
                }

                //organize the sensors consistently
                IWindSpeedConfig extendend = null;
                IWindSpeedConfig extendstart = null;
                
                double distanceAB = Utils.SectorRange(A.Orientation, B.Orientation, CircleDirection.Clockwise);
                double distanceBA = Utils.SectorRange(B.Orientation, A.Orientation, CircleDirection.Clockwise);
                double sensorDistance =0;
                
            Console.WriteLine(" dist from a to b " + distanceAB + " B to A " + distanceBA);
                
                if (distanceAB < distanceBA )
                {   
                    sensorDistance =distanceAB ;
                    extendend = B;
                    extendstart = A;
                }
                else
                {
                    sensorDistance =distanceBA ;
                    extendend = A;
                    extendstart = B;
                }
                
                
                double sectorvar = (2 * Settings.Default.ShadowInterval) + sensorDistance;

               if (sectorvar <= 360)
                {
                    double Extendrange = (360 - ((2 * Settings.Default.ShadowInterval) + sensorDistance));
                    Console.WriteLine("extending range by " + Extendrange);

                    extendend.ExtendGoodRangeEnd(Extendrange);
                    extendstart.ExtendGoodRangeStart(Extendrange);
                   
                }

        }
       
        
        
        
    }
}
