using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp.IRIS
{
    internal static class IRISConfig
    {
        public const string BASE_URL = @"http://iris.noncd.db.de/iris-tts/timetable";
        public const string PLAN_URL = @"/plan";
        public const string FULL_CHANGE_URL = @"/fchg";
        public const string PARTIAL_CHANGE_URL = @"/rchg";
        public const string STATION_URL = @"/station";
    }
}
