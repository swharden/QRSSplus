using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QrssPlusViewer
{
    public class GrabberTracker
    {
        public readonly List<GrabberStatus> Grabbers = new();
        public string BaseUrl = "https://qrssplus.z20.web.core.windows.net";
        public bool IsUpdating { get; private set; } = false;

        public DateTime LastUpdate { get; private set; }

        public GrabberTracker()
        {
        }

        public async Task UpdateAsync()
        {
            IsUpdating = true;

            string GrabbersUsonUrl = BaseUrl + "/grabbers.json";
            var client = new HttpClient();
            var response = await client.GetAsync(GrabbersUsonUrl);
            string json = await response.Content.ReadAsStringAsync();

            var document = System.Text.Json.JsonDocument.Parse(json);

            string dateTimeString = document.RootElement.GetProperty("created").GetString();
            DateTime dt = DateTime.Parse(dateTimeString);

            List<GrabberStatus> NewStatuses = new();
            foreach (var grabber in document.RootElement.GetProperty("grabbers").EnumerateObject())
            {
                GrabberStatus status = new()
                {
                    ID = grabber.Value.GetProperty("id").GetString(),
                    Name = grabber.Value.GetProperty("name").GetString(),
                    Callsign = grabber.Value.GetProperty("callsign").GetString(),
                    Location = grabber.Value.GetProperty("location").GetString(),
                    ImageUrl = grabber.Value.GetProperty("imageUrl").GetString(),
                    SiteUrl = grabber.Value.GetProperty("siteUrl").GetString(),
                    AgeMinutes = grabber.Value.GetProperty("lastUniqueAgeMinutes").GetDouble(),
                    GrabUrls = grabber.Value.GetProperty("urls").EnumerateArray().Select(x => x.GetString()).ToArray()
                };
                NewStatuses.Add(status);
            }

            Grabbers.Clear();
            Grabbers.AddRange(NewStatuses);
            IsUpdating = false;
        }
    }
}
