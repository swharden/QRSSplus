using Microsoft.Azure.Cosmos.Table;
using System;

namespace QrssPlus.TableStorage
{
    public class GrabResult : TableEntity
    {
        public string GrabberID { get; set; }
        public string Hash { get; set; }
        public double DownloadDuration { get; set; }
        public string DownloadTimeCode { get; set; }

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
