using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public class WindSpeedCompColumns 
    {
        ISessionColumnCollection _collection;
        DataTable _data;
       
        //pass in date, pass in height, pass htconfig
        public WindSpeedCompColumns(ISessionColumnCollection collection, DataTable data)
        {
            _collection = collection;
            _data = data;
            
        }
        public  bool Add(double height, IList<ISessionColumn > columns, DateTime startdate,DateTime enddate)
        {

            string compositeSuffix = Settings.Default.CompColName;
            string compositeName=string.Empty;
            string childCompositeName = string.Empty;
            int compositeIndex = 0;
            

            compositeName = height.ToString();
            compositeName += Enum.GetName(typeof(SessionColumnType), SessionColumnType.WSAvg);
            compositeName += Settings.Default.CompColName;
            compositeName = compositeName.Replace(".", "_");

            //Console.WriteLine("comp parent will be named " + compositeName);
            //for each child column in each column add a composite column to the datatable 
                            
                //add parent col 
                
                if (!_data.Columns.Contains(compositeName))
                {
                   // Console.WriteLine("adding " + compositeName);
                    //***datatable
                    DataColumn newcol = new DataColumn(compositeName);
                    newcol.DataType = typeof(double);
                    _data.Columns.Add(newcol);
                    compositeIndex = _data.Columns[compositeName].Ordinal;
                    ISessionColumn newSessCol = new SessionColumn(compositeIndex, compositeName);
                    newSessCol.ColumnType = SessionColumnType.WSAvg;
                    newSessCol.IsComposite = true;
                    _collection.Columns.Add(newSessCol);

                    //****metadata collection 
                    //set the height of the composite col using either of the sensor pair 
                    ISensorConfig config = new SensorConfig();
                    config.Height =height;
                    config.StartDate = startdate ;
                    config.EndDate = enddate;
                    _collection[compositeName].addConfig(config);
                }
                //add child columns 
                Console.WriteLine(" number of parent columns " + columns.Count);
            foreach(ISessionColumn column in columns )
            {
                
                foreach (ISessionColumn child in column.ChildColumns)
                {
                    
                //    //build comp column name from each column type and the composite suffix from the app properties
                    childCompositeName = height.ToString();
                    childCompositeName += child.ColumnType;
                    childCompositeName += Settings.Default.CompColName;
                    childCompositeName = childCompositeName.Replace(".", "_");

                    

                    if (!_data.Columns.Contains(childCompositeName))
                    {
                        Console.WriteLine("adding child comp " + childCompositeName);
                       
                        //add to datatable
                        DataColumn newChildCol = new DataColumn();
                        newChildCol.DataType = _data.Columns[child.ColIndex].DataType;
                        newChildCol.ColumnName = childCompositeName;
                        _data.Columns.Add(newChildCol);

                        //add to sessioncolumn collection
                        ISessionColumn newCollectionChildCol = new SessionColumn(_data.Columns[newChildCol.ColumnName].Ordinal, newChildCol.ColumnName);
                        newCollectionChildCol.ColumnType = child.ColumnType;
                        newCollectionChildCol.IsComposite = true;
                        _collection.Columns.Add(newCollectionChildCol);

                        //config
                        ISensorConfig childconfig = new SensorConfig();
                        childconfig.Height = height;
                        childconfig.StartDate = startdate;
                        childconfig.EndDate = enddate;
                        _collection[childCompositeName].addConfig(childconfig);
                    }
                }

            }
            _data.AcceptChanges();
            return true;

        }
    }
}
