using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class GrabberListFactory
    {
        public static GrabberList CreateFromCsvFile(string csvFilePath)
        {
            string csvText = File.ReadAllText(csvFilePath);
            return FromCsvText(csvText);
        }

        public static GrabberList CreateFromCsvUrl(string url)
        {
            using WebClient client = new();
            string csvText = client.DownloadString(url);
            return FromCsvText(csvText);
        }

        private static GrabberList FromCsvText(string csv)
        {
            GrabberList grabbers = new();

            const int valuesPerLine = 5;

            foreach (string rawLine in csv.Split("\n"))
            {
                string line = rawLine.Trim();
                if (line.StartsWith("#"))
                    continue;
                if (line.Split(',').Length < valuesPerLine - 1)
                    continue;

                var grabber = GrabberInfoFromCsvLine(line);
                grabbers.Add(grabber);
            }

            return grabbers;
        }

        private static string[] ParseCsvLine(string line)
        {
            Regex CSVParser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            return CSVParser.Split(line);
        }

        private static GrabberInfo GrabberInfoFromCsvLine(string line)
        {
            string[] parts = ParseCsvLine(line);

            if (parts.Length != 7)
                throw new InvalidOperationException("Error parsing CSV line: " + line);

            return new GrabberInfo()
            {
                ID = parts[0],
                Callsign = parts[1],
                Title = parts[2],
                Name = parts[3],
                Location = parts[4],
                SiteUrl = parts[5],
                ImageUrl = parts[6]
            };
        }
    }
}
