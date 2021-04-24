using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlus.Functions
{
    class GrabberStatusTableEntity : TableEntity
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Callsign { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string SiteUrl { get; set; }

        public DateTime LastRequestTime { get; set; }
        public string LastRequestResult { get; set; }

        public DateTime LastUniqueDateTime { get; set; }
        public string LastUniqueHash { get; set; }
        public bool IsNewHash { get; set; } = false;

        public GrabberStatusTableEntity()
        {
        }

        public GrabberStatusTableEntity(Core.Grabber grabber)
        {
            PartitionKey = "GrabberStatus";
            RowKey = grabber.Info.ID;
            Title = grabber.Info.Title;
            Name = grabber.Info.Name;
            Callsign = grabber.Info.Callsign;
            Location = grabber.Info.Location;
            ImageUrl = grabber.Info.ImageUrl;
            SiteUrl = grabber.Info.SiteUrl;
        }

        public void Update(Core.GrabData grab)
        {
            LastRequestTime = grab.DateTime;
            LastRequestResult = grab.HttpResponse;

            bool hasValidHash = grab.Hash != null;
            bool hasNewHash = grab.Hash != LastUniqueHash;
            if (hasValidHash && hasNewHash)
            {
                LastUniqueDateTime = grab.DateTime;
                LastUniqueHash = grab.Hash;
                IsNewHash = true;
            }
        }
    }
}
