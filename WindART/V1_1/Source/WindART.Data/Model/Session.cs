using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.IO;
using WindART.DAL;
using System.Windows.Forms;


namespace WindART
{
    
    public class Session:ISession
    {
            protected DataSet _sessionDataSet=new DataSet ("Session");
            protected Dictionary<string, ISessionColumnCollection> _columnCollection
               = new Dictionary<string, ISessionColumnCollection>();
            
      
      //properties
            public DataSet SessionDataSet
              {
                  get
                  {
                      return _sessionDataSet;
                  }
              }
            public Dictionary<string,ISessionColumnCollection> ColumnCollections
            {
                  get
                  {                      
                      return _columnCollection;
                  }
          
            }
            
       //public methods

            
      //private methods 
       protected List<ISessionColumn> ProcessColumns(DataTable data)
        {
            //create 
            List<ISessionColumn> result = new List<ISessionColumn>();
            foreach (DataColumn dc in data.Columns)
            {
                ISessionColumn  S = new SessionColumn(dc.Ordinal,dc.ColumnName);
                result.Add(S);
            }
            return result;
        }
        
    }
}
