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
        public double AgeMinutes;
        public double AgeDays => AgeMinutes / (60 * 24);
        public string[] GrabUrls;
        public string[] ThumbUrls => GrabUrls.Select(x => x + "-thumb-auto.jpg").ToArray();
        public bool IsActive => GrabUrls.Length > 0;
    }
}
