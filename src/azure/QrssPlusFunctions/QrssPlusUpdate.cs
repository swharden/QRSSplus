using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using QrssPlus;

namespace QrssPlusFunctions
{
    public static class QrssPlusUpdate
    {
        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Console.WriteLine("Starting");
            CloudTable statusTable = ConnectToStatusTable();
            BlobContainerClient storage = ConnectToBlobStorage();

            var dl = new Downloader();
            dl.DownloadGrabberList(maxGrabberCount: 10);
            dl.DownloadGrabberImages();

            Parallel.ForEach(dl.GetGrabbers(), grabber =>
            {
                UpdateStatus(grabber, statusTable);
                if (grabber.Age == 0)
                    SaveNewGrab(grabber, storage);
            });

            DeleteOldGrabs(storage, maxAgeMinutes: 60);

            SaveStatusJson(dl.GetGrabbers(), storage);
        }

        private static void UpdateStatus(Grabber grabber, CloudTable table)
        {
            TableOperation getLastGrabberStatus = TableOperation.Retrieve<GrabberStatus>(partitionKey: "GrabberStatus", rowkey: grabber.ID);
            TableResult result = table.Execute(getLastGrabberStatus);
            GrabberStatus lastStatus = (GrabberStatus)result.Result;

            GrabberStatus newStatus = new GrabberStatus(grabber)
            {
                LastRequestTime = grabber.DateTime,
                LastRequestResult = grabber.Response
            };

            if (lastStatus is null)
            {
                // this a new grabber with a new hash
                newStatus.LastUniqueDateTime = grabber.DateTime;
                newStatus.LastUniqueHash = grabber.Hash;
                newStatus.LastUniqueAge = 0;
            }
            else
            {
                if (grabber.Hash != lastStatus.LastUniqueHash)
                {
                    // this is a new hash
                    newStatus.LastUniqueDateTime = grabber.DateTime;
                    newStatus.LastUniqueHash = grabber.Hash;
                    newStatus.LastUniqueAge = 0;
                }
                else
                {
                    // this is an old hash
                    newStatus.LastUniqueDateTime = lastStatus.LastUniqueDateTime;
                    newStatus.LastUniqueHash = lastStatus.LastUniqueHash;
                    TimeSpan age = grabber.DateTime - lastStatus.LastUniqueDateTime;
                    newStatus.LastUniqueAge = age.TotalMinutes;
                }
            }
            TableOperation setGrabberStatus = TableOperation.InsertOrMerge(newStatus);
            table.Execute(setGrabberStatus);
            grabber.Age = newStatus.LastUniqueAge;
            Console.WriteLine($"Updated status for {grabber.ID} (age={grabber.Age})");
        }

        private static CloudTable ConnectToStatusTable()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GrabberStatus");
            table.CreateIfNotExists();
            return table;
        }

        private static BlobContainerClient ConnectToBlobStorage()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, "$web");
            container.CreateIfNotExists();
            return container;
        }

        private static void SaveStatusJson(Grabber[] grabbers, BlobContainerClient storage)
        {
            string[] allFilePaths = storage.GetBlobs().Select(x => x.Name).ToArray();
            string[] grabberFilenames = allFilePaths.Select(x => Path.GetFileName(x)).ToArray();

            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = true };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            writer.WriteString("DateTime", grabbers[0].DateTime);
            writer.WriteStartObject("Grabbers");
            foreach (var grabber in grabbers)
            {
                writer.WriteStartObject("Grabber");
                writer.WriteString("ID", grabber.ID);
                writer.WriteString("Name", grabber.Name);
                writer.WriteString("Callsign", grabber.Callsign);
                writer.WriteString("Location", grabber.Location);
                writer.WriteString("ImageUrl", grabber.ImageUrl);
                writer.WriteString("SiteUrl", grabber.SiteUrl);
                writer.WriteString("Response", grabber.Response);
                writer.WriteNumber("Age", grabber.Age);

                writer.WriteStartArray("Filenames");
                foreach (string filename in grabberFilenames.Where(x => x.StartsWith(grabber.ID)))
                    writer.WriteStringValue(filename);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.Flush();

            BlobClient blob = storage.GetBlobClient("grabbers.json");
            stream.Position = 0;
            blob.Upload(stream, overwrite: true);
            Console.WriteLine("Saved grabbers.json");
        }

        private static void SaveNewGrab(Grabber grabber, BlobContainerClient storage)
        {
            BlobClient blob = storage.GetBlobClient("grabs/" + grabber.GetFilename());
            using var stream = new MemoryStream(grabber.Bytes, writable: false);
            blob.Upload(stream);
            stream.Close();
            Console.WriteLine($"Stored image for {grabber.ID}");
        }

        private static void DeleteOldGrabs(BlobContainerClient storage, int maxAgeMinutes)
        {
            foreach (var blobItem in storage.GetBlobs())
            {
                var blobItemAge = DateTime.UtcNow - blobItem.Properties.LastModified;
                if (blobItemAge > TimeSpan.FromMinutes(maxAgeMinutes))
                {
                    Console.WriteLine($"Deleting old blob: {blobItem.Name}");
                    storage.DeleteBlob(blobItem.Name);
                }
            }
        }
    }
}
