using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public record GrabberInfo
    {
        public string ID { get; init; }
        public string Callsign { get; init; }
        public string Title { get; init; }
        public string Name { get; init; }
        public string Location { get; init; }
        public string SiteUrl { get; init; }
        public string ImageUrl { get; init; }
        public override string ToString() => $"Grabber {ID} {ImageUrl}";
    }
}
