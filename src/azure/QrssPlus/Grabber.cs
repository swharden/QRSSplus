using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QrssPlus
{
    public class Grabber
    {
        public readonly GrabberInfo Info;
        public readonly GrabberData Data = new GrabberData();
        public readonly GrabberHistory History = new GrabberHistory();

        public Grabber(GrabberInfo info)
        {
            Info = info;
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
            bool isNewImage = Data.Hash != null && Data.Hash != History.LastUniqueHash;
            if (isNewImage)
            {
                History.LastUniqueHash = Data.Hash;
                History.LastUniqueDateTime = dt;
            }
            History.LastUniqueAgeMinutes = (dt - History.LastUniqueDateTime).Minutes;
        }
    }
}
