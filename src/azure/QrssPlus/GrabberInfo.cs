using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public record GrabberInfo
    {
        public string ID { get; init; }
        public string Callsign { get; init; }
        public string Title { get; init; }
        public string Name { get; init; }
        public string Location { get; init; }
        public string SiteUrl { get; init; }
        public string ImageUrl { get; init; }

        public override string ToString() => $"Grabber {ID} {ImageUrl}";

        public string GetFilename(DateTime? timestamp = null)
        {
            DateTime dt = timestamp ?? DateTime.UtcNow;
            string date = $"{dt.Year:D4}.{dt.Month:D2}.{dt.Day:D2}";
            string time = $"{dt.Hour:D2}.{dt.Minute:D2}.{dt.Second:D2}";
            return $"{ID.ToLower()}-{date}-{time}.jpg";
        }

        public GrabResult Download()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string hash;
            int code;

            using WebClient client = new();
            try
            {
                byte[] data = client.DownloadData(ImageUrl);
                hash = GrabData.Hash(data);
                code = 200;
            }
            catch (WebException ex)
            {
                hash = null;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                code = (int)response.StatusCode;
            }

            double downloadTime = sw.Elapsed.TotalMilliseconds;
            return new GrabResult(ID, hash, code, downloadTime);
        }
    }
}
