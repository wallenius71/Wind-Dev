using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace WindART.DAL
{
    public class SQL2005DataOrder:IDataOrder 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _connectionString;
        private string _providerName;
        private string _uid;
        private string _password;
        
        public SQL2005DataOrder(string datasource, DataSourceType dst, string UID, string password)
        {
            _dataSource = datasource;
            _dataType = dst;
            _uid=UID;
            _password=password;

            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
           ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            conn.ConnectionString  = settings.ConnectionString;
            conn["User ID"] = _uid;
            conn["Password"] = _password;
            conn["Data Source"] = _dataSource;
            
            _connectionString = conn.ConnectionString;
                        
        }

        public SQL2005DataOrder(DataSourceType dst)
        {
            
            _dataType = dst;
                      
          
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            _connectionString  = settings.ConnectionString;
            

        }

        public string ConnectionString
        {
            get { return _connectionString; }
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
