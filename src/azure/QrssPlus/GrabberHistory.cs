using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlus
{
    public class GrabberHistory
    {
        public readonly List<string> RecentFilenames = new List<string>();
        public DateTime LastUniqueDateTime;
    }
}
