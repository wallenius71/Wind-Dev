using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART.DAL
{
    public interface IDataRepository
    {
        DataTable GetAllData();
        DataTable GetData(string param);
    }
}
