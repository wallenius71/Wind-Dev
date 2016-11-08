using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using WindART.DAL;

namespace WindART
{
    public interface ISession
    {
        
        //properties
        DataSet SessionDataSet
        {
            get;

        }
        Dictionary<string, ISessionColumnCollection> ColumnCollections { get; }   

         
        
        
    }
}
