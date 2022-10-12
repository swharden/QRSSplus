using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public int ImageWidth { get; private set; } = 0;
        public int ImageHeight { get; private set; } = 0;
        public bool HasImageData => Bytes != null && Bytes.Length > 0;
        public bool ContainsNewUniqueImage = false;

        public void Download(GrabberInfo info, DateTime dt)
        {
            DateTime = dt;
            using WebClient client = new WebClient();
            try
            {
                Bytes = client.DownloadData(info.ImageUrl);

                if (Bytes is null || Bytes.Length == 0)
                    throw new WebException("Image URL contains no data");

                if (Bytes[0] == '<')
                    throw new WebException("Image URL contains HTML (not an image)");

                using MemoryStream msIn = new MemoryStream(Bytes);
                Image originalImage;
                try {
                    originalImage = Bitmap.FromStream(msIn);
                } catch (Exception e) {
                    throw new InvalidOperationException($"image creation from downloaded bytes fail {info.ImageUrl}");
                }
                ImageWidth = originalImage.Width;
                ImageHeight = originalImage.Height;

                string timestamp = $"{dt.Year:D2}.{dt.Month:D2}.{dt.Day:D2}.{dt.Hour:D2}.{dt.Minute:D2}.{dt.Second:D2}";
                Filename = $"{info.ID} {timestamp} {ImageWidth}x{ImageHeight} " + Path.GetExtension(info.ImageUrl);

                Hash = GetHash(Bytes);
                Response = "success";
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
