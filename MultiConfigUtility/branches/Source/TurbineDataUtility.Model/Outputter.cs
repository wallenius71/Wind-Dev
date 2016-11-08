using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TurbineDataUtility.Model
{
    public class Outputter: IOutputter 
    {
        DataTable _data;

        public Outputter(DataTable data)
        {
            _data = data;
        }

        #region IOutputter Members

        public bool Output(string filename)
        {
            return true;
        }

        #endregion
    }
}
