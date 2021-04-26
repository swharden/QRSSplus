using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class GrabberFactory
    {
        public static Grabber[] GrabbersFromCsvFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"CSV not exist: {filePath}");
            string text = File.ReadAllText(filePath);
            return GrabbersFromCsvText(text);
        }

        public static async Task<Grabber[]> GrabbersFromCsvUrl(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            string text = await response.Content.ReadAsStringAsync();
            Grabber[] grabbers = GrabbersFromCsvText(text);
            return grabbers;
        }

        public static Grabber[] GrabbersFromCsvText(string csv) =>
            csv
            .Split("\n")
            .Select(line => GrabberFromCsvLine(line))
            .Where(x => x != null)
            .ToArray();

        /// <summary>
        /// Try to parse a CSV line and return a grabber (or null if it cannot be parsed)
        /// </summary>
        public static Grabber GrabberFromCsvLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("#"))
                return null;

            string[] parts = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(line);
            parts = parts.Select(s => s.Trim(new char[] { '\'', '"', ' ' })).ToArray();
            if (parts.Length != 7)
                return null;

            return new Grabber(parts[0])
            {
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
