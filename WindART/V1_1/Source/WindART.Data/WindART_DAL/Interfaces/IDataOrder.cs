using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART.DAL
{

    public interface IDataOrder
    {
        string ConnectionString
        {
            get;
        }
        string ProviderType
        {
            get;
        }
        string DefaultSQL
        {
            get;

        }
        
    }
}
