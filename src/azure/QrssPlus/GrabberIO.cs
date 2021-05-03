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
                writer.WriteStartObject("grabber");

                // info
                writer.WriteString("id", grabber.Info.ID);
                writer.WriteString("name", grabber.Info.Name);
                writer.WriteString("callsign", grabber.Info.Callsign);
                writer.WriteString("location", grabber.Info.Location);
                writer.WriteString("imageUrl", grabber.Info.ImageUrl);
                writer.WriteString("siteUrl", grabber.Info.SiteUrl);

                // history
                writer.WriteString("lastResponse", grabber.Data.Response);
                writer.WriteString("lastUniqueHash", grabber.History.LastUniqueHash);
                writer.WriteString("lastUniqueDateTime", grabber.History.LastUniqueDateTime);
                writer.WriteNumber("lastUniqueAgeMinutes", grabber.History.LastUniqueAgeMinutes);
                writer.WriteNumber("lastUniqueAgeDays", grabber.History.LastUniqueAgeMinutes / (60 * 24));

                // images
                writer.WriteStartArray("urls");
                foreach (string url in grabber.History.URLs)
                    writer.WriteStringValue(url);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();

            writer.Flush();
            string json = Encoding.UTF8.GetString(stream.ToArray());

            return json;
        }

        public static Grabber[] GrabbersFromJson(string json)
        {
            const string EXPECTED_VERSION = "1.0";

            using JsonDocument document = JsonDocument.Parse(json);

            string version = document.RootElement.GetProperty("version").GetString();
            if (version != EXPECTED_VERSION)
                throw new InvalidOperationException("invalid JSON version");

            List<Grabber> grabbers = new List<Grabber>();
            foreach (var grabber in document.RootElement.GetProperty("grabbers").EnumerateObject())
            {
                GrabberInfo info = new GrabberInfo()
                {
                    ID = grabber.Value.GetProperty("id").GetString(),
                    Name = grabber.Value.GetProperty("name").GetString(),
                    Callsign = grabber.Value.GetProperty("callsign").GetString(),
                    Location = grabber.Value.GetProperty("location").GetString(),
                    ImageUrl = grabber.Value.GetProperty("imageUrl").GetString(),
                    SiteUrl = grabber.Value.GetProperty("siteUrl").GetString(),
                };

                GrabberHistory history = new GrabberHistory()
                {
                    LastUniqueHash = grabber.Value.GetProperty("lastUniqueHash").GetString(),
                    LastUniqueDateTime = grabber.Value.GetProperty("lastUniqueDateTime").GetDateTime(),
                    LastUniqueAgeMinutes = grabber.Value.GetProperty("lastUniqueAgeMinutes").GetInt32(),
                    URLs = grabber.Value.GetProperty("urls").EnumerateArray().Select(x => x.GetString()).ToArray()
                };

                grabbers.Add(new Grabber(info, history));
            }

            return grabbers.ToArray();
        }
    }
}
