using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public interface IDateTimeSequence
    {
        TimeSpan DetectInterval();
        List<DateTime> GetMissingTimeStamps();
        List<DateTime> GetExistingSequence();
        List<DateTime> GetExpectedSequence();
    }
}
