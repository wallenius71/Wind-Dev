using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common ;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace WindART.DAL
{
   public class PiDBDataOrder:IDataOrder 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _connectionString;
        private string _providerName;
        private string _uid;
        private string _password;
        
        public PiDBDataOrder(string datasource, DataSourceType dst, string UID, string password)
        {
            _dataSource = datasource;
            _dataType = dst;
            _uid=UID;
            _password=password;
            
            
            OleDbConnectionStringBuilder connstring = new OleDbConnectionStringBuilder();
           ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            connstring.ConnectionString = settings.ConnectionString;
            connstring.DataSource = _dataSource;
            connstring["User ID"] = _uid;
            connstring["Password"] = _password;
            _connectionString = connstring.ConnectionString;

                        
        }

        public PiDBDataOrder(DataSourceType dst)
        {
            
            _dataType = dst;
           ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            _connectionString = settings.ConnectionString;


        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderType
        {
            get
            {
                
                return _providerName;
            }
        }

        public string DefaultSQL
        {
            get { return string.Empty; }
        }

}
}
