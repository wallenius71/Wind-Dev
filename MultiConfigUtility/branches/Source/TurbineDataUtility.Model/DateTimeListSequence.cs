using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART;

namespace TurbineDataUtility.Model
{
    public class DateListTimeSequence : IDateTimeSequence
    {
        protected List<DateTime> _subjectDateList;
        protected DateTime _start;
        protected DateTime _end;


        public DateListTimeSequence(List<DateTime> subjectDates, DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            _subjectDateList = subjectDates;

        }

        public List<DateTime> GetMissingTimeStamps()
        {
            
                List<DateTime> existing = GetExistingSequence();
                List<DateTime> expected = GetExpectedSequence();


                var result = expected.Except(existing);
                return result.ToList();
            
        }
        public TimeSpan DetectInterval()
        {
            //determines the interval by selecting the interval with the greatest occurrence
            

                //count each interval occurence
                TimeSpan gap = default(TimeSpan);
                DateTime prevVal = default(DateTime);
                DateTime curVal = default(DateTime);

                Dictionary<TimeSpan, int> resultCount = new Dictionary<TimeSpan, int>();


                //create table of counts grouped by gap
                foreach (DateTime dt in _subjectDateList)
                {
                    curVal = dt;
                    if (prevVal != default(DateTime))
                    {
                        gap = curVal - prevVal;
                        if (resultCount.ContainsKey(gap))
                        {
                            resultCount[gap] += 1;
                        }
                        else
                        {
                            resultCount.Add(gap, 1);
                        }
                    }
                    prevVal = curVal;
                }

                //determine the gap with the greatest count
                int workval = 0;
                int maxval = 0;
                TimeSpan greatest = default(TimeSpan);
                foreach (KeyValuePair<TimeSpan, int> kv in resultCount)
                {

                    workval = kv.Value;
                    if (workval > maxval)
                    {
                        maxval = workval;
                        greatest = kv.Key;
                    }
                }
                return greatest;
           
            
        }
        public List<DateTime> GetExistingSequence()
        {
            return _subjectDateList;
        }
        public List<DateTime> GetExpectedSequence()
        {   TimeSpan interval=DetectInterval();
            TimeSpan intvl = interval == TimeSpan.Zero ? TimeSpan.FromMinutes(10.0) : interval;
        
            return Utils.GetExpectedSequence(_start, _end, intvl);
        }

    }
}
