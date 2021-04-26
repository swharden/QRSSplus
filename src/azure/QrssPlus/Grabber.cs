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
        public readonly string ID;
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

        // historical grabs
        public readonly List<string> RecentFilenames = new List<string>();
        public DateTime LastUniqueDateTime;

        public Grabber(string id)
        {
            ID = SanitizeID(id);
        }

        /// <summary>
        /// Returns a valid grabber ID given any input string
        /// </summary>
        private static string SanitizeID(string id)
        {
            if (id is null)
                throw new ArgumentException("ID cannot be null");

            var validChars = id.ToUpper().ToCharArray().Where(c => char.IsLetterOrDigit(c) || c == '-');

            if (validChars.Count() == 0)
                throw new ArgumentException("ID contains no valid characters");

            return string.Join("", validChars);
        }

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

        public string GetFilename()
        {
            DateTime dt = DateTime;
            string timestamp = $"{dt.Year:D2}.{dt.Month:D2}.{dt.Day:D2}.{dt.Hour:D2}.{dt.Minute:D2}.{dt.Second:D2}";
            string ext = System.IO.Path.GetExtension(ImageUrl);
            string filename = $"{ID} {timestamp} {Hash}{ext}";
            return filename;
        }
    }
}
