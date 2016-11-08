using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace TurbineDataUtility.Model
{
    public class Outputter
    {
        DataTable _data;

        public Outputter(DataTable data)
        {
            _data = data;
        }

        #region IOutputter Members

        public bool Output(string filename )
        {
           
            return true;

        }

        #endregion
    }
}
