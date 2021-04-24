using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QrssPlus.Functions
{
    public static class QrssPlusUpdate
    {
        private const int MaxImageStorageMinutes = 60;
        private const string GrabberJsonFilename = "grabbers.json";
        private const string CsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";

        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            Console.WriteLine($"Downloading CSV data...");
            List<Core.Grabber> grabberList = Core.GrabberListFactory.CreateGrabberListFromCsvUrl(CsvUrl);

            (CloudTable table, BlobContainerClient container) = GetServices();
            Console.WriteLine($"Downloading grabs...");
            DateTime dt = DateTime.UtcNow;
            GrabberStatusTableEntity[] oldStatuses = table.CreateQuery<GrabberStatusTableEntity>().ToArray();
            Parallel.ForEach(grabberList, grabber =>
            {
                grabber.Download(dt);
                bool newHash = UpdateStatus(grabber, oldStatuses, table);
                if (newHash)
                    UploadImageData(grabber, container);
            });
            DeleteOldBlobs(container, maxAgeMinutes: MaxImageStorageMinutes);
            UpdateGrabberJson(grabberList, container, dt);

            Console.WriteLine($"DONE");
        }

        /// <summary>
        /// Connect to cloud services, create them if needed, and return them
        /// </summary>
        private static (CloudTable table, BlobContainerClient container) GetServices()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GrabberStatus");
            table.CreateIfNotExists();

            BlobContainerClient container = new(storageConnectionString, "$web");
            container.CreateIfNotExists();

            return (table, container);
        }

        /// <summary>
        /// Compare this grabber and its data to its previous status and update its status in the table.
        /// Returns True if the grab contains data with a new hash.
        /// </summary>
        private static bool UpdateStatus(Core.Grabber grabber, GrabberStatusTableEntity[] oldStatuses, CloudTable table)
        {
            var oldStatusesWithMatchingID = oldStatuses.Where(x => x.RowKey == grabber.ID);
            var status = oldStatusesWithMatchingID.Any() ? oldStatusesWithMatchingID.First() : new GrabberStatusTableEntity(grabber);
            status.Update(grabber.Grab);
            grabber.LastUniqueHashDateTime = status.LastUniqueDateTime;
            Console.WriteLine($" - updating {status.RowKey} status: (IsNewHash = {status.IsNewHash})");
            table.Execute(TableOperation.InsertOrMerge(status));
            return status.IsNewHash;
        }

        /// <summary>
        /// Upload the latest grab's data to blob storage.
        /// Only call this if the grabber contains a new hash.
        /// </summary>
        private static void UploadImageData(Core.Grabber grabber, BlobContainerClient container)
        {
            (string filename, byte[] bytes) = grabber.GetFile();
            Console.WriteLine($" - uploading blob: {filename}");
            BlobClient blob = container.GetBlobClient(filename);
            using var stream = new MemoryStream(bytes);
            blob.Upload(stream);
            stream.Close();
        }

        /// <summary>
        /// Delete blobs older than the given age
        /// </summary>
        private static void DeleteOldBlobs(BlobContainerClient container, int maxAgeMinutes)
        {
            foreach (var blobItem in container.GetBlobs())
            {
                var blobItemAge = DateTime.UtcNow - blobItem.Properties.LastModified;
                if (blobItemAge > TimeSpan.FromMinutes(maxAgeMinutes))
                {
                    Console.WriteLine($" - deleting old blob: {blobItem.Name}");
                    container.DeleteBlob(blobItem.Name);
                }
            }
        }

        /// <summary>
        /// Create a JSON summary of all grabber statuses and upload it
        /// </summary>
        private static void UpdateGrabberJson(List<Core.Grabber> grabberList, BlobContainerClient container, DateTime dt)
        {
            string[] allFilenames = container.GetBlobs().Select(x => x.Name).ToArray();

            using var stream = new MemoryStream();
            var options = new JsonWriterOptions() { Indented = true };
            using var writer = new Utf8JsonWriter(stream, options);

            writer.WriteStartObject();
            writer.WriteString("UpdatedDateTime", dt);
            writer.WriteStartObject("Grabbers");
            foreach (var grabber in grabberList)
            {
                writer.WriteStartObject("Grabber");
                writer.WriteString("ID", grabber.Info.ID);
                writer.WriteString("Name", grabber.Info.Name);
                writer.WriteString("Callsign", grabber.Info.Callsign);
                writer.WriteString("Location", grabber.Info.Location);
                writer.WriteString("ImageUrl", grabber.Info.ImageUrl);
                writer.WriteString("SiteUrl", grabber.Info.SiteUrl);
                writer.WriteString("LastUniqueHashDateTime", grabber.LastUniqueHashDateTime);
                writer.WriteNumber("LastUpdateAgeMinutes", (dt - grabber.LastUniqueHashDateTime).TotalMinutes);

                writer.WriteStartArray("Filenames");
                foreach (string filename in allFilenames.Where(x => x.StartsWith(grabber.ID, StringComparison.InvariantCultureIgnoreCase)))
                    writer.WriteStringValue(filename);
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();

            writer.Flush();

            BlobClient blob = container.GetBlobClient(GrabberJsonFilename);
            stream.Position = 0;
            blob.Upload(stream, overwrite: true);
            stream.Close();
        }
    }
}
