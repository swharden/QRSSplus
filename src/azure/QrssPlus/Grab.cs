using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    /// <summary>
    /// The data obtained by downloading the latest grab from a grabber
    /// </summary>
    public class Grab
    {
        public string Hash { get; private set; }
        public byte[] Data { get; private set; }
        public double DownloadDuration { get; private set; }
        public DateTime DownloadDateTime { get; private set; }
        public string DownloadTimeCode { get; private set; }

        private readonly GrabberInfo Info;

        public Grab(GrabberInfo info)
        {
            Info = info;
        }

        public void Download()
        {
            DownloadDateTime = DateTime.UtcNow;
            DownloadTimeCode = GetTimeCode(DownloadDateTime);

            Stopwatch sw = Stopwatch.StartNew();
            using WebClient client = new();
            try
            {
                Data = client.DownloadData(Info.ImageUrl);
                Hash = GetHash(Data);
            }
            catch (WebException ex)
            {
                Hash = ex.Message;
            }
            DownloadDuration = sw.Elapsed.TotalMilliseconds;
        }

        private static string GetTimeCode(DateTime dt) =>
            $"{dt.Year:D2}-{dt.Month:D2}-{dt.Day:D2}-{dt.Hour:D2}-{dt.Minute:D2}-{dt.Second:D2}";

        private static string GetHash(byte[] data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBytes = md5.ComputeHash(data);
            string hashString = string.Join("", hashBytes.Select(x => x.ToString("x2")));
            return hashString;
        }
    }
}
