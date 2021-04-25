using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QrssPlus
{
    public class Grabber
    {
        // grabber details
        public string ID;
        public string Callsign;
        public string Title;
        public string Name;
        public string Location;
        public string ImageUrl;
        public string SiteUrl;

        // latest grab
        public byte[] Bytes;
        public string Hash;
        public DateTime DateTime;
        public string Response;

        public void DownloadLatestGrab()
        {
            using WebClient client = new WebClient();
            try
            {
                Bytes = client.DownloadData(ImageUrl);
                Hash = GetHash(Bytes);
                Response = "success";
                Console.WriteLine($"{ID} {Hash}");
            }
            catch (WebException ex)
            {
                Response = ex.Message;
                Console.WriteLine($"{ID} {Response}");
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
