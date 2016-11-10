using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;


namespace WindART.DAL
{
    

    public class DbProviderFactory:IDbProviderFactory 
    {
        private IDataOrder _dataOrder;
        //concrete instance of db provider factory
        private  System.Data.Common.DbProviderFactory _frameworkProviderFactory;
        
       
        
       public DbProviderFactory(IDataOrder dataorder)
        {
            _dataOrder = dataorder;
            _frameworkProviderFactory = DbProviderFactories.GetFactory(_dataOrder.ProviderType  );
        }

        public IDbConnection CreateConnection()
        {
            //return  connection object of the correct type
            IDbConnection conn=  _frameworkProviderFactory.CreateConnection();
            conn.ConnectionString = _dataOrder.ConnectionString;
            return conn;
            
         }

        public IDataAdapter CreateDataAdapter()
        {
            //blank dataadapter of the correct type
            return  _frameworkProviderFactory.CreateDataAdapter ();
            

        }

        public IDbCommand CreateCommand()
        {
            //return blank command
            return _frameworkProviderFactory.CreateCommand();
        }

        public IDataParameter CreateParameter()
        {
            //return blank parameter of the correct type 
            return _frameworkProviderFactory.CreateParameter();
        }

        
    }
}
