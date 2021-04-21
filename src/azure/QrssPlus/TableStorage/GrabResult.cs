using Microsoft.Azure.Cosmos.Table;
using System;

namespace QrssPlus.TableStorage
{
    public class GrabResult : TableEntity
    {
        public string GrabberID { get; init; }
        public string Hash { get; init; }
        public double DownloadDuration { get; init; }
        public string DownloadTimeCode { get; init; }

        public GrabResult() { }

        public GrabResult(Grabber grabber)
        {
            GrabberID = grabber.Info.ID;
            Hash = grabber.Grab.Hash;
            DownloadDuration = grabber.Grab.DownloadDuration;
            DownloadTimeCode = grabber.Grab.DownloadTimeCode;

            PartitionKey = "GrabResult";
            RowKey = $"{GrabberID} {DownloadTimeCode}";
        }
    }
}
