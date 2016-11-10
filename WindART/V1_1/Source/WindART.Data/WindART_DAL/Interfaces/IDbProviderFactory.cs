using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART.DAL
{
    public interface IDbProviderFactory
    {
        
        IDbConnection CreateConnection();
        IDataAdapter  CreateDataAdapter();
        IDbCommand    CreateCommand();
        IDataParameter CreateParameter();

    }
}
