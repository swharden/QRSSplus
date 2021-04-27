using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class GrabberIO
    {
        /// <summary>
        /// Return an array of grabbers from a local CSV file
        /// </summary>
        public static Grabber[] GrabbersFromCsvFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath);
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"CSV not exist: {filePath}");
            string text = File.ReadAllText(filePath);
            return GrabbersFromCsvText(text);
        }

        /// <summary>
        /// Return an array of grabbers from a CSV file on the internet
        /// </summary>
        public static async Task<Grabber[]> GrabbersFromCsvUrl(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            string text = await response.Content.ReadAsStringAsync();
            Grabber[] grabbers = GrabbersFromCsvText(text);
            return grabbers;
        }

        private static Grabber[] GrabbersFromCsvText(string csv) =>
            csv
            .Split("\n")
            .Select(line => GrabberFromCsvLine(line))
            .Where(x => x != null)
            .ToArray();

        /// <summary>
        /// Try to parse a CSV line and return a grabber (or null if it cannot be parsed)
        /// </summary>
        private static Grabber GrabberFromCsvLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("#"))
                return null;

            string[] parts = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(line);
            parts = parts.Select(s => s.Trim(new char[] { '\'', '"', ' ' })).ToArray();
            if (parts.Length != 7)
                return null;

            var info = new GrabberInfo()
            {
                ID = parts[0],
                Callsign = parts[1],
                Title = parts[2],
                Name = parts[3],
                Location = parts[4],
                SiteUrl = parts[5],
                ImageUrl = parts[6],
            };
            return new Grabber(info);
        }

        /// <summary>
        /// Create a JSON status file describing a list of grabbers
        /// </summary>
        public static string GrabbersToJson(Grabber[] grabbers)
        {
            const string API_VERSION = "1.0";

            DateTime dt = DateTime.UtcNow;

            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = true };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            writer.WriteString("version", API_VERSION);
            writer.WriteString("created", dt);
            writer.WriteStartObject("grabbers");
            foreach (var grabber in grabbers)
            {
                writer.WriteStartObject("Grabber");
                writer.WriteString("id", grabber.Info.ID);
                writer.WriteString("name", grabber.Info.Name);
                writer.WriteString("callsign", grabber.Info.Callsign);
                writer.WriteString("location", grabber.Info.Location);
                writer.WriteString("imageUrl", grabber.Info.ImageUrl);
                writer.WriteString("siteUrl", grabber.Info.SiteUrl);
                writer.WriteString("lastUniqueDateTime", grabber.History.LastUniqueDateTime);
                writer.WriteNumber("lastUniqueAgeMinutes", (dt - grabber.History.LastUniqueDateTime).TotalMinutes);

                writer.WriteStartArray("filenames");
                string[] grabberFilenames = { "a.jpg", "b.jpg", "c.jpg" };
                foreach (string filename in grabberFilenames)
                    writer.WriteStringValue(filename);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();

            writer.Flush();
            string json = Encoding.UTF8.GetString(stream.ToArray());

            return json;
        }
    }
}
