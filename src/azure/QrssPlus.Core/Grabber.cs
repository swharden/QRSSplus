using System;
using System.Collections.Generic;
using System.Text;

namespace QrssPlus.Core
{
    public class Grabber
    {
        public string ID => Info.ID;
        public GrabberInformation Info { get; private set; }
        public GrabData Grab { get; private set; }
        public DateTime LastUniqueHashDateTime;

        public Grabber(GrabberInformation info)
        {
            Info = info;
        }

        /// <summary>
        /// Perform the download and calculate the hash
        /// </summary>
        public void Download(DateTime dt)
        {
            Grab = new GrabData(dt, Info.ImageUrl);
        }

        public (string filename, byte[] bytes) GetFile()
        {
            DateTime dt = Grab.DateTime;
            string timestamp = $"{dt.Year:D2}.{dt.Month:D2}.{dt.Day:D2}.{dt.Hour:D2}.{dt.Minute:D2}.{dt.Second:D2}";
            string ext = System.IO.Path.GetExtension(Info.SiteUrl);
            string filename = $"{ID} {timestamp} {Grab.Hash}{ext}";
            return (filename, Grab.Bytes);
        }
    }
}
