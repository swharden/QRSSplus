using Microsoft.Azure.Cosmos.Table;
using System;

namespace QrssPlus
{
    public class GrabResult : TableEntity
    {
        public string GrabberID { get; init; }
        public string Hash { get; init; }
        public int ResultCode { get; init; }
        public double ResultTime { get; init; }

        public GrabResult(string id, string hash, int code, double time)
        {
            PartitionKey = "GrabResult";
            string timestamp = DateTime.UtcNow.ToString("s", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            RowKey = $"{id} {timestamp}";
            GrabberID = id;
            Hash = hash;
            ResultCode = code;
            ResultTime = time;
        }
    }
}
