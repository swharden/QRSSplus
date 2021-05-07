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
        public double AgeHours => AgeMinutes / 60;
        public double AgeDays => AgeHours / 24;
        public string[] GrabUrls;
        public string[] ThumbUrls => GrabUrls.Select(x => x + "-thumb-auto.jpg").ToArray();
        public bool IsActive => GrabUrls.Length > 0;
        public string LastGrabMessage
        {
            get
            {
                if (AgeMinutes < 20)
                    return "live";

                if (AgeDays < 1)
                    return $"{AgeHours:N0} hours";

                if (AgeDays > 10_000)
                    return "never";

                return $"{AgeDays:N2} days";
            }
        }
    }
}
