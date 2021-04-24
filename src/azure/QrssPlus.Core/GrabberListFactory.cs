using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace QrssPlus.Core
{
    public static class GrabberListFactory
    {
        public static List<Grabber> CreateGrabberListFromCsvUrl(string url, int maxGrabberCount = int.MaxValue)
        {
            var grabbers = new List<Grabber>();

            using WebClient client = new WebClient();
            string[] lines = client.DownloadString(url).Split("\n");

            foreach (string line in lines)
            {
                GrabberInformation info = GrabberInfoFromCsvLine(line);

                if (info != null)
                    grabbers.Add(new Grabber(info));

                if (grabbers.Count() >= maxGrabberCount)
                    break;
            }

            return grabbers;
        }

        /// <summary>
        /// Given a single line in CSV format, return a grabber if it can be parsed (or null if it cannot)
        /// </summary>
        public static GrabberInformation GrabberInfoFromCsvLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("#"))
                return null;

            string[] parts = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(line);
            if (parts.Length != 7)
                return null;
            parts = parts.Select(s => s.Trim(new char[] { '\'', '"', ' ' })).ToArray();

            return new GrabberInformation()
            {
                ID = Validate.SanitizeID(parts[0]),
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
