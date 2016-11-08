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
    public class ExcelDataOrder:IDataOrder 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _connectionString;
        private string _providerName;
        private string _catalog;

        public ExcelDataOrder(string datasource, DataSourceType dst)
        {
            _dataSource = datasource;
            _dataType = dst;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;

            settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
            
        }
        public ExcelDataOrder(string datasource, DataSourceType dst,string SheetName)
        {//constructor to optionally pass in a  specific worksheet name
            _dataSource = datasource;
            _dataType = dst;
            _catalog = SheetName;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;

            settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType), _dataType)];
            _providerName = settings.ProviderName;
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
           //excel 
           OleDbConnectionStringBuilder connstring = new OleDbConnectionStringBuilder();
           ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[Enum.GetName(typeof(DataSourceType)
               , _dataType)];
           connstring.ConnectionString = settings.ConnectionString;
           connstring.DataSource = _dataSource;
           return connstring.ConnectionString;
       }
        public string DefaultSQL
       {
           get
           {
               if (string.IsNullOrEmpty(_catalog))
               {
                   _catalog = GetExcelTable();
               }
               Console.WriteLine(_catalog);
               return string.Format(@"Select * from [{0}]", _catalog);
           }

       }

        #region methods
        private string GetExcelTable()
       {
           string tblName = string.Empty;
           OleDbConnection conn = new OleDbConnection(this.ConnectionString);
           try
           {
               
               conn.Open();
               //return the correct table from a sheet 
               if (conn.State == ConnectionState.Open)
               {
                   
                   DataTable XLTables = new DataTable();
                   XLTables = conn.GetSchema("Tables");
                   tblName = XLTables.Rows[0]["Table_Name"].ToString();
                   
                   return tblName;
               }
               else
               {
                   conn.Close();
                   return string.Empty;
               }
           }
           catch (Exception e)
           {
               conn.Close();
               throw;
               
           }

       }
        #endregion 


    }
}
