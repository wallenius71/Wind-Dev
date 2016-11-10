using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common ;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.IO;



namespace WindART.DAL
{
    public class TextDataOrder:IDataOrder 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _connectionString;
        private string _providerName;
        private string _catalog;

        public TextDataOrder(string datasource, DataSourceType dst)
        {
            _dataSource = datasource;
            _dataType = dst;
            _catalog = Path.GetFileName(datasource);
            
           
        }
        
        //properties
        public string ConnectionString
        {            
            get
            {               
                if (string.IsNullOrEmpty ( _connectionString))
                {
                    _connectionString = getConnectionString();
                }
                return _connectionString ;

            }

        }
        public string ProviderType
        {
            get
            {
                if (string.IsNullOrEmpty(_providerName))
                {
                    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName (typeof(DataSourceType ), _dataType)];
                    _providerName = settings.ProviderName;
                }
                return _providerName;
            }
        }
        private string getConnectionString()
       {
           //file of csv 
           OleDbConnectionStringBuilder connstring = new OleDbConnectionStringBuilder();
           ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
           connstring.ConnectionString = settings.ConnectionString;
           connstring.DataSource = Path.GetDirectoryName ( _dataSource);

           return connstring.ConnectionString;
       }
        public string DefaultSQL
       {
           get
           {
               return string.Format(@"Select * from {0}", _catalog);
           }

       }

        

       
    }
}
