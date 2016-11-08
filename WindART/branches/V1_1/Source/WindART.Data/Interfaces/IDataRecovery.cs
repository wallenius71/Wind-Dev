using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public interface IDataRecovery
    {
        int GetNonMissingRecords(int colindex);
        int GetNonMissingRecords(int colindex, int paramindex, DateTime start, DateTime end);
        int GetNonMissingRecords(int colindex, int paramindex, double start, double end);
        double GetNonMissingRate(int colindex);
        double GetNonMissingRate(int colindex, int paramindex, DateTime start, DateTime end);
        double GetNonMissingRate(int colindex, int paramindex, double start, double end);
        
    }
}
