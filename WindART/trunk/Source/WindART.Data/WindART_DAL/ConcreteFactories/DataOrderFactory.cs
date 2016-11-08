using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART.DAL
{
    public class DataOrderFactory:IDataOrderFactory 
    {
        private DataSourceType _dataType;
        private string _dataSource;
        private string _uid;
        private string _password;

        public  DataOrderFactory(string datasource,DataSourceType dataType)
        {
            Console.WriteLine(datasource );
            _dataType = dataType;
            _dataSource = datasource;
        }
        public DataOrderFactory(DataSourceType dataType)
        {
            _dataType = dataType;
           
        }
        public DataOrderFactory(string datasource, DataSourceType dataType,string uid,string password)
        {
            Console.WriteLine(datasource);
            _dataType = dataType;
            _dataSource = datasource;
            _uid = uid;
            _password = password;
        }
        public IDataOrder getDataOrder()
        {           
            try
            {

                IDataOrder dataorder = null;
                switch (_dataType)
                {
                    case DataSourceType.XL2003:
                        dataorder = new ExcelDataOrder(_dataSource, _dataType );
                        break;
                    case DataSourceType.XL2007:
                        dataorder = new ExcelDataOrder(_dataSource, _dataType);
                        break;
                    case DataSourceType.CSV:
                        dataorder = new TextDataOrder(_dataSource, _dataType);
                        break;
                    case DataSourceType.TXT:
                        dataorder = new TextDataOrder(_dataSource, _dataType);
                        break;
                    case DataSourceType.PRN:
                        dataorder = new TextDataOrder(_dataSource, _dataType);
                        break;

                    case DataSourceType.OSIPI:
                        dataorder = new PiDBDataOrder(_dataSource, _dataType,_uid,_password);
                        break;
                    case DataSourceType.SQL2005 :
                        dataorder = new SQL2005DataOrder(_dataSource, _dataType, _uid, _password);
                        break;
                    
                    case DataSourceType.Operations_PI :
                        dataorder = new PiDBDataOrder(_dataType);
                        break;
                    
                    case DataSourceType.WindART_PI :
                        dataorder = new PiDBDataOrder(_dataType);
                        break;
                    
                    case DataSourceType.WindART_SQL :
                        dataorder = new SQL2005DataOrder(_dataType);
                    break;

                    case DataSourceType.WindART_ekho:
                    dataorder = new SQL2005DataOrder(_dataType);
                    break;

                    case DataSourceType.Cedar_Creek:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Edom_Hills :
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Flat_Ridge :
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Fowler_01 :
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Fowler_02:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Silver_Star :
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Titan:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Trinity:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Sherbino_Mesa_2:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;

                    case DataSourceType.Goshen:
                    dataorder = new PiDBDataOrder(_dataType);
                    break;


                }
                
                return dataorder;
            }
            catch (Exception e)
            {
                throw;
            }


        }

        
    }
}
