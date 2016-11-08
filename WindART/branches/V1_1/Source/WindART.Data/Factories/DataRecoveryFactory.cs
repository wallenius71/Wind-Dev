using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public class DataRecoveryFactory
    {
        public IDataRecovery CreateDataRecoveryObject(DataTable table)
        {
            return new DataRecovery(table);
        }
    }
}
