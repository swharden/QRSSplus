using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public struct GrabberInfo
    {
        public readonly string ID;
        public readonly string Callsign;
        public readonly string Title;
        public readonly string Name;
        public readonly string Location;
        public readonly string SiteUrl;
        public readonly string ImageUrl;

        public override string ToString() => $"Grabber {ID} {ImageUrl}";

        public GrabberInfo(string id, string callsign, string title, string name, string location, string siteUrl, string imageUrl)
        {
            ID = id;
            Callsign = callsign;
            Title = title;
            Name = name;
            Location = location;
            SiteUrl = siteUrl;
            ImageUrl = imageUrl;
        }
    }
}
