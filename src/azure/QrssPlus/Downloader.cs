using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QrssPlus
{
    public class Downloader
    {
        private readonly List<Grabber> Grabbers = new List<Grabber>();

        public Downloader()
        {

        }

        public Grabber[] GetGrabbers() => Grabbers.ToArray();

        /// <summary>
        /// Populate the list of grabbers from a CSV file
        /// </summary>
        public void DownloadGrabberList(int maxGrabberCount = 999)
        {
            const string url = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            using WebClient client = new WebClient();
            string[] lines = client.DownloadString(url).Split("\n");

            foreach (string line in lines)
            {
                var grabber = MakeGrabberFromCsvLine(line);

                if (grabber != null)
                    Grabbers.Add(grabber);

                if (Grabbers.Count() >= maxGrabberCount)
                    break;
            }
        }

        public void DownloadGrabberImages()
        {
            DateTime dt = DateTime.UtcNow;
            Parallel.ForEach(Grabbers, grabber =>
            {
                grabber.DownloadLatestGrab();
                grabber.DateTime = dt;
            });
        }

        /// <summary>
        /// Try to parse a CSV line and return a grabber (or null if it cannot be parsed)
        /// </summary>
        private static Grabber MakeGrabberFromCsvLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("#"))
                return null;

            string[] parts = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(line);
            parts = parts.Select(s => s.Trim(new char[] { '\'', '"', ' ' })).ToArray();
            if (parts.Length != 7)
                return null;

            return new Grabber()
            {
                ID = parts[0],
                Callsign = parts[1],
                Title = parts[2],
                Name = parts[3],
                Location = parts[4],
                SiteUrl = parts[5],
                ImageUrl = parts[6],
            };
        }
    }
}
