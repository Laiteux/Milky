using System;

namespace Milky.Utils
{
    public class DateTimeUtils
    {
        public long GetCurrentUnixTimeSeconds()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public int SecondsToMilliseconds(int seconds)
        {
            return seconds * 1000;
        }

        private static DateTimeUtils _classInstance;
        public static DateTimeUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new DateTimeUtils());
        }
    }
}