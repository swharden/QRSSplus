using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlus.Functions
{
    public class GrabberInfo : TableEntity
    {
        [IgnoreProperty]
        public string ID { get => RowKey; set { RowKey = value; } }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Callsign { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string SiteUrl { get; set; }

        public DateTime LastUniqueDateTime { get; set; }
        public string LastUniqueHash { get; set; }
        public DateTime LastRequestTime { get; set; }
        public string LastRequestResult { get; set; }

        [IgnoreProperty]
        public byte[] ImageData { get; set; }

        public GrabberInfo()
        {
        }

        public GrabberInfo(string id, string callsign, string title, string name, string location, string siteUrl, string imageUrl)
        {
            PartitionKey = "GrabberInfo";
            RowKey = id;
            Callsign = Callsign;
            Title = title;
            Name = name;
            Location = location;
            SiteUrl = siteUrl;
            ImageUrl = imageUrl;
        }

        public void Download(DateTime dt)
        {
            Console.WriteLine($" - downloading {ID}");
            UpdateSuccess(dt, Guid.NewGuid().ToString().Replace("-", ""));
        }

        private void UpdateSuccess(DateTime dt, string hash)
        {
            if (hash != LastUniqueHash)
            {
                LastUniqueDateTime = dt;
                LastUniqueHash = hash;
                LastRequestResult = "active";
            }
            LastRequestTime = dt;
            LastRequestResult = "inactive";
        }

        private void UpdateFail(DateTime dt, string error)
        {
            LastRequestTime = dt;
            LastRequestResult = error;
        }

        public void UpdateFromLast(GrabberInfo lastGrab)
        {
            LastUniqueDateTime = lastGrab.LastUniqueDateTime;
            LastUniqueHash = lastGrab.LastUniqueHash;
            LastRequestTime = lastGrab.LastRequestTime;
            LastRequestResult = lastGrab.LastRequestResult;
        }
    }
}
