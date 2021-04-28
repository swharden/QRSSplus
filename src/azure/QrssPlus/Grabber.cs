using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QrssPlus
{
    public class Grabber
    {
        public readonly GrabberInfo Info;
        public readonly GrabberHistory History;
        public readonly GrabberData Data;

        public Grabber(GrabberInfo info = null, GrabberHistory history = null)
        {
            Info = info ?? new GrabberInfo();
            History = history ?? new GrabberHistory();
            Data = new GrabberData();
        }

        public override string ToString()
        {
            if (Data.Hash is null)
                return $"{Info.ID} error={Data.Response}";
            else
                return $"{Info.ID} hash={Data.Hash}";
        }

        public void DownloadLatestGrab(DateTime dt)
        {
            Data.Download(Info, dt);
            Data.ContainsNewUniqueImage = Data.Hash != null && Data.Hash != History.LastUniqueHash;
            if (Data.ContainsNewUniqueImage)
            {
                History.LastUniqueHash = Data.Hash;
                History.LastUniqueDateTime = dt;
            }
            History.LastUniqueAgeMinutes = (dt - History.LastUniqueDateTime).Minutes;
        }
    }
}
