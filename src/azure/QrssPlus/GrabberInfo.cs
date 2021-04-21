using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    /// <summary>
    /// This information describes a grabber station.
    /// It is defined in a CSV file on GitHub.
    /// </summary>
    public class GrabberInfo
    {
        public string ID { get; init; }
        public string Callsign { get; init; }
        public string Title { get; init; }
        public string Name { get; init; }
        public string Location { get; init; }
        public string SiteUrl { get; init; }
        public string ImageUrl { get; init; }
    }
}
