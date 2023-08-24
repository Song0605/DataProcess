using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcess.Method
{
    public static class TimeConverter
    {
        public static void TimeStampToDateTime()
        {
            var shortTimeStamp = 1612192654;
            var time = DateTimeOffset.FromUnixTimeSeconds(shortTimeStamp);
            var date = time.DateTime;
        }
    }
}
