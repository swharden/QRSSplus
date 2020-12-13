using System;

namespace QPAnalyzer
{
    public class GrabRecord
    {
        public readonly DateTime DateTime;
        public readonly string ID;
        public readonly string Hash;

        public GrabRecord(DateTime dt, string id, string hash) => (DateTime, ID, Hash) = (dt, id, hash);
        public GrabRecord(string timestamp, string id, string hash) => (DateTime, ID, Hash) = (GetDateTime(timestamp), id, hash);

        public static DateTime GetDateTime(string timesamp)
        {
            if (timesamp.Length != 10)
                throw new ArgumentException("invalid timestamp format");

            int year = int.Parse(timesamp.Substring(0, 2)) + 2000;
            int month = int.Parse(timesamp.Substring(2, 2));
            int day = int.Parse(timesamp.Substring(4, 2));
            int hour = int.Parse(timesamp.Substring(6, 2));
            int minute = int.Parse(timesamp.Substring(8, 2));
            int second = 0;

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
