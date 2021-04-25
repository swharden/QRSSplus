using Microsoft.Azure.Cosmos.Table;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlusFunctions.TableEntities
{
    class GrabberStatus : TableEntity
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

        public double LastUniqueAge { get; set; }

        public GrabberStatus()
        {
            PartitionKey = "GrabberStatus";
        }

        public GrabberStatus(Grabber grabber)
        {
            PartitionKey = "GrabberStatus";
            RowKey = grabber.ID;
            Title = grabber.Title;
            Name = grabber.Name;
            Callsign = grabber.Callsign;
            Location = grabber.Location;
            ImageUrl = grabber.ImageUrl;
            SiteUrl = grabber.SiteUrl;
        }
    }
}
