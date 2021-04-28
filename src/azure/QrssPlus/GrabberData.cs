using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QrssPlus
{
    public class GrabberData
    {
        public byte[] Bytes { get; private set; }
        public string Hash { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Response { get; private set; }
        public string Filename { get; private set; }
        public bool HasImageData => Bytes != null && Bytes.Length > 0;
        public bool ContainsNewUniqueImage = false;

        public void Download(GrabberInfo info, DateTime dt)
        {
            DateTime = dt;
            using WebClient client = new WebClient();
            try
            {
                Bytes = client.DownloadData(info.ImageUrl);
                if (Bytes[0] == '<')
                    throw new WebException("Image URL points to a HTML page, not an image file");
                Hash = GetHash(Bytes);
                Response = "success";
                string timestamp = $"{dt.Year:D2}.{dt.Month:D2}.{dt.Day:D2}.{dt.Hour:D2}.{dt.Minute:D2}.{dt.Second:D2}";
                string extension = System.IO.Path.GetExtension(info.ImageUrl);
                Filename = info.ID + " " + timestamp + extension;
            }
            catch (WebException ex)
            {
                Response = ex.Message;
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
