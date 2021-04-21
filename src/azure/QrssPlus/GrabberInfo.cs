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
        public string ID { get; set; }
        public string Callsign { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string SiteUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}
