using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.OleDb;

namespace WindART.DAL
{
    class DBDataOrder:IDataOrder 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _connectionString;
        private string _providerName;
        private string _catalog;

        public DBDataOrder(string datasource, DataSourceType dst)
        {
            _dataSource = datasource;
            _dataType = dst;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;

            settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            
        }

        public string ConnectionString
        {
            get { throw new NotImplementedException(); }
        }

        public string ProviderType
        {
            get { throw new NotImplementedException(); }
        }

        public string DefaultSQL
        {
            get { throw new NotImplementedException(); }
        }

        
    }
}
