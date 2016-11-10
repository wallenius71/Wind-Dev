using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;


namespace WindART.DAL
{
    public class DataRepository:IDataRepository
    {
        #region events

        public delegate void FileProgressEventHandler(string msg);
        public event FileProgressEventHandler FileOpening;
        public event FileProgressEventHandler FileOpened;
        public event FileProgressEventHandler FileLoading;
        public event FileProgressEventHandler FileLoaded;

        #endregion
        private IDbProviderFactory _providerFactory;
        private IDataOrder _dataOrder;

        public DataRepository(DataSourceType dst)
        {
            //create correct type of data order
            IDataOrderFactory dataorderfactory = new DataOrderFactory(dst);
            _dataOrder = dataorderfactory.getDataOrder();
            IDbProviderFactory provider = new DbProviderFactory(_dataOrder);
            _providerFactory = provider;
        }

        public DataRepository(string datasource, DataSourceType dst)
        {
            //create correct type of data order
            IDataOrderFactory dataorderfactory = new DataOrderFactory(datasource,dst);
            _dataOrder = dataorderfactory.getDataOrder();
            IDbProviderFactory provider = new DbProviderFactory(_dataOrder );
            _providerFactory=provider;
        }
        public DataRepository(string datasource, DataSourceType dst, string uid, string password)
        {
            //create correct type of data order
            IDataOrderFactory dataorderfactory = new DataOrderFactory(datasource, dst,uid,password );
            _dataOrder = dataorderfactory.getDataOrder();
            IDbProviderFactory provider = new DbProviderFactory(_dataOrder);
            _providerFactory = provider;
        }

        //methods
        public DataTable GetAllData()
        {
            try
            {
                if (_dataOrder.ProviderType == @"PIOLEDB" ) return null;

                DataTable rawData = new DataTable();

                using (IDbConnection connection = _providerFactory.CreateConnection())
                {
                    connection.ConnectionString =  _dataOrder.ConnectionString;
                    if (FileOpening != null)
                        FileOpening("Opening File...");
                    
                    connection.Open();

                    if (FileOpened != null)
                        FileOpened("Opened file successfully");


                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = _dataOrder.DefaultSQL;
                    command.CommandType = CommandType.Text;
                    IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    if (FileLoading != null)
                        FileLoading("Loading file...");
                    //load dataset from reader
                    using (reader)
                    {
                        
                        rawData.Load(reader);
                        
                    }
                    if (FileLoaded != null)
                        FileLoaded("File loaded");
                }
                //return the data
                return rawData;
            }
            catch(Exception e)
            {   
                throw e;
            }
        }

        public DataTable GetData(string SQL)
        {

            try
            {

                DataTable rawData = new DataTable();

                using (IDbConnection connection = _providerFactory.CreateConnection())
                {
                    connection.ConnectionString = _dataOrder.ConnectionString;
                    
                    if (connection.State == ConnectionState.Closed) connection.Open();

                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = SQL;
                    command.CommandType = CommandType.Text;
                    IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    
                    //load dataset from reader
                    using (reader)
                    {

                        rawData.Load(reader);

                    }

                }
                //return the data
                return rawData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool WriteData(string sql)
        {
            try
            {
                using (IDbConnection connection = _providerFactory.CreateConnection())
                {
                    connection.ConnectionString = _dataOrder.ConnectionString;

                    if (connection.State == ConnectionState.Closed) connection.Open();

                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Repository Data Write error: " + e.Message);
                return false;

            }
            

            return true;

        }
        
        
    }
}
