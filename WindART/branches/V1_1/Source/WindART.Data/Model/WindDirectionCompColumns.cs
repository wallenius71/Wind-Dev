using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties ;

namespace WindART
{
    public class WindDirectionCompColumns
    {
        ISessionColumnCollection _collection;
        DataTable _data;

        public delegate void ProgressEventHandler(string msg);
        public event ProgressEventHandler AddingWDCompCol;
        public event ProgressEventHandler AddedWDCompCol;

        
        public WindDirectionCompColumns(ISessionColumnCollection collection, DataTable data)
        {
            _collection = collection;
            _data = data;
           
            
        }
        public  bool Add(List<ISessionColumn> columns)
        {
            
                string compositeSuffix=Settings .Default .CompColName ;
                string compositeName;
                //for each child column in each column add a composite column to the datatable 
                foreach (ISessionColumn column in columns)
                {
                    //add parent col 
                    compositeName = Enum.GetName(typeof(SessionColumnType), column.ColumnType) + compositeSuffix;
                   
                    if (!_data.Columns.Contains(compositeName))
                    {
                        Console.WriteLine("adding " + compositeName);
                        if(AddingWDCompCol !=null)
                        AddingWDCompCol("Adding " + compositeName);
                        //add to datatable
                        DataColumn newCol = new DataColumn();
                        newCol.DataType = _data.Columns[column.ColIndex].DataType;
                        newCol.ColumnName = compositeName;
                       _data.Columns.Add(newCol);
                        Console.WriteLine("Adding " + compositeName + " at index " + _data.Columns[newCol.ColumnName].Ordinal);
                        //add to sessioncolumn collection
                        ISessionColumn newSessCol = new SessionColumn(_data.Columns[newCol.ColumnName].Ordinal
                            , newCol.ColumnName);
                        
                        newSessCol.ColumnType = SessionColumnType.WDAvg;
                        newSessCol.IsComposite = true;
                        _collection.Columns.Add(newSessCol);
                        if (AddedWDCompCol !=null)
                        AddedWDCompCol("Successfully added " + compositeName + " at index " + _data.Columns[newCol.ColumnName].Ordinal);
                    }
                    //add child columns 
                    foreach (ISessionColumn child in column.ChildColumns)
                    {
                        //build comp column name from each column type and the composite suffix from the app properties 
                        compositeName = Enum.GetName(typeof(SessionColumnType), child.ColumnType) + compositeSuffix;
                        if (!_data.Columns.Contains(compositeName ))
                        {
                            
                            //add to datatable
                            DataColumn newCol = new DataColumn();
                            newCol.DataType = _data.Columns[child.ColIndex].DataType;
                            newCol.ColumnName = compositeName;
                            _data.Columns.Add(newCol);
                            Console.WriteLine("adding " + compositeName + " at index " + _data.Columns[newCol.ColumnName].Ordinal);
                            if(AddingWDCompCol !=null)
                            AddingWDCompCol("Adding " + compositeName + " at index " + _data.Columns[newCol.ColumnName].Ordinal);
                            //add to sessioncolumn collection
                            ISessionColumn newSessCol=new SessionColumn (_data.Columns [newCol.ColumnName ].Ordinal ,newCol.ColumnName  );
                            newSessCol.ColumnType =child.ColumnType;
                            newSessCol.IsComposite = true;
                            _collection.Columns.Add(newSessCol);
                            if (AddedWDCompCol !=null)
                            AddedWDCompCol("Successfully Added " + compositeName + " at index " + _data.Columns[newCol.ColumnName].Ordinal);
                        }
                    }

                }
                _data.AcceptChanges();
                return true;

        }
    }
}
