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

        public void DownloadLatestGrab(DateTime dt) => Data.Download(Info, dt);
    }
}
