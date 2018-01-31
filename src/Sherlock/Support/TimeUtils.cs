using System;
using System.Collections.Generic;
using System.Text;

namespace Sherlock.Support
{
    internal static class EpochUtils
    {
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToEpochMillis(this DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime FromMillis(long ms)
        {
            return UnixEpoch.AddMilliseconds(ms);
        }
    }
}
