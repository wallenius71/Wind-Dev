using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace WindART
{
    public interface IProcessDateTime
    {
        DataSetDateStatus FindDateTimeColumn(out int dateindex );
        bool IsDateTime(int dateindex);
        List<DateTime> BuildDateTimeList(IDateOrder dateorder);
        
    }
}
