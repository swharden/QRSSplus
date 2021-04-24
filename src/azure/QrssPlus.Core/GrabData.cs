using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QrssPlus.Core
{
    public class GrabData
    {
        public DateTime DateTime;
        public byte[] Bytes;
        public string Hash;
        public string HttpResponse;

        public GrabData(DateTime dt, string url)
        {
            DateTime = dt;
            using WebClient client = new WebClient();
            try
            {
                Bytes = client.DownloadData(url);
                Hash = GetHash(Bytes);
            }
            catch (WebException ex)
            {
                HttpResponse = ex.Message;
            }
        }

        private static string GetHash(byte[] data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBytes = md5.ComputeHash(data);
            string hashString = string.Join("", hashBytes.Select(x => x.ToString("x2")));
            return hashString;
        }
    }
}
