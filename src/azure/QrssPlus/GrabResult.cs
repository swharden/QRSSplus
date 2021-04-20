using Microsoft.Azure.Cosmos.Table;
using System;

namespace QrssPlus
{
    public class GrabResult : TableEntity
    {
        public string GrabberID { get; init; }
        public string TimeCode { get; init; }
        public string Hash { get; init; }
        public int ResultCode { get; init; }
        public double ResultTime { get; init; }

        public override string ToString() => $"{RowKey} {Hash}";

        public GrabResult(string id, string hash, int code, double time)
        {
            TimeCode = GetTimeCode();
            GrabberID = id;
            Hash = hash;
            ResultCode = code;
            ResultTime = time;

            PartitionKey = "GrabResult";
            RowKey = $"{id} {TimeCode}";
        }

        private static string GetTimeCode()
        {
            DateTime dt = DateTime.UtcNow;
            return $"{dt.Year:D2}-{dt.Month:D2}-{dt.Day:D2}-{dt.Hour:D2}-{dt.Minute:D2}-{dt.Second:D2}";
        }
    }
}
