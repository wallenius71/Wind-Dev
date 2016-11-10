using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.Properties ;

namespace WindART
{
    public  class DataRecovery:IDataRecovery 
    {
        //depends on no duplicates in dataset 
        private DataTable _data;
        readonly double missingval = Settings.Default.MissingValue;

        public  DataRecovery(DataTable data)
        {
            _data =data;
        }

        public virtual int GetNonMissingRecords(int colindex)
        {   
            //all values in column 
            var result = from d in _data.AsEnumerable()
                         where (double)d[colindex] != missingval 
                         select d;
            
            return result.ToList().Count;
        }
        public virtual int GetNonMissingRecords(int colindex,int paramindex, DateTime start, DateTime end)
        {
            //values between 
            try
            {
                var result = from d in _data.AsEnumerable()
                             where ((DateTime)d[paramindex] >= start && (DateTime)d[paramindex] <= end)
                             && (double)d[colindex] != missingval 
                             select d;
                return result.ToList().Count;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual int GetNonMissingRecords(int colindex, int paramindex, double start, double end)
        {
            try
            {
                var result = from d in _data.AsEnumerable()
                             where ((double)d[paramindex] >= start && (double)d[paramindex] <= end)
                             && (double)d[colindex] != missingval 
                             select d;
                return result.ToList().Count;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public virtual double GetNonMissingRate(int colindex)
        {
            try
            {
                int fullcount = _data.Rows.Count;
                var firstres = from d in _data.AsEnumerable()
                               where d[colindex].GetType() != typeof(double)
                               select d;
                int count = firstres.ToList().Count;
                var result = from d in _data.AsEnumerable()
                             where (double)d[colindex] != missingval
                             select d;

                int validcount = result.ToList().Count;
                double rate = Math.Round(((double)validcount / (double)fullcount) * 100, 2);
                Console.WriteLine(rate);
                return rate;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public virtual double GetNonMissingRate(int colindex, int paramindex, DateTime start, DateTime end)
        {
            try
            {
                var fullresult = from d in _data.AsEnumerable ()
                                where ((DateTime)d[paramindex] >=start && (DateTime)d[paramindex]<=end)
                                select d;
                
                int fullcount=fullresult.ToList ().Count;

                var result = from d in _data.AsEnumerable()
                             where ((DateTime)d[paramindex] >=start && (DateTime)d[paramindex]<=end)
                             && (double)d[colindex] != missingval
                             select d;

                int validcount = result.ToList().Count;
                double rate = Math.Round(((double)validcount / (double)fullcount) * 100, 2);
                return rate;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual double GetNonMissingRate(int colindex, int paramindex, double start, double end)
        {
            try
            {
                var fullresult = from d in _data.AsEnumerable()
                                 where ((double)d[paramindex] >= start && (double)d[paramindex] <= end)
                                 select d;

                int fullcount = fullresult.ToList().Count;

                var result = from d in _data.AsEnumerable()
                             where ((double)d[paramindex] >= start && (double)d[paramindex] <= end)
                             && (double)d[colindex] != missingval
                             select d;

                int validcount = result.ToList().Count;
                double rate = Math.Round(((double)validcount / (double)fullcount) * 100, 2);
                return rate;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        
    }
}
