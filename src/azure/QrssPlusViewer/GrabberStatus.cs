using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrssPlusViewer
{
    public class GrabberStatus
    {
        public string ID;
        public string Name;
        public string Callsign;
        public string Location;
        public string ImageUrl;
        public string SiteUrl;
        public double Age;
        public string[] GrabUrls;
        public bool IsActive => GrabUrls.Length > 0;
    }
}
