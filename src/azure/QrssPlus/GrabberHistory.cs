using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlus
{
    public class GrabberHistory
    {
        public string LastUniqueHash;
        public DateTime LastUniqueDateTime;
        public int LastUniqueAgeMinutes = -1;

        public string[] URLs = new string[] { };

        public void Update(GrabberHistory oldHistory)
        {
            LastUniqueHash = oldHistory.LastUniqueHash;
            LastUniqueDateTime = oldHistory.LastUniqueDateTime;
            LastUniqueAgeMinutes = oldHistory.LastUniqueAgeMinutes;
        }
    }
}