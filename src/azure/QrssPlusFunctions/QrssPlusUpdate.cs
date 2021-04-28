using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using QrssPlus;

namespace QrssPlusFunctions
{
    public static class QrssPlusUpdate
    {
        private const string PATH_JSON_STATUSES = "grabbers.json";
        private const string PATH_GRAB_PREFIX = "grabs/";

        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, "$web");

            DateTime dt = DateTime.UtcNow;
            log.LogInformation($"Starting at {dt}");

            log.LogInformation($"Downloading grabber list...");
            Grabber[] grabbers = GetGrabbers();
            log.LogInformation($"Loaded {grabbers.Length} grabbers");

            log.LogInformation($"Updating history from previous status file...");
            UpdateGrabberHistory(grabbers, container);

            log.LogInformation($"Downloading grabber images...");
            Parallel.ForEach(grabbers, grabber =>
            {
                grabber.DownloadLatestGrab(dt);
                if (grabber.Data.ContainsNewUniqueImage)
                    StoreImageData(grabber, container);

                string newMessage = grabber.Data.ContainsNewUniqueImage ? "new" : "old";
                log.LogInformation($" - {grabber.Info.ID} {grabber.Data.Response} ({newMessage})");
            });

            log.LogInformation($"Deleting old grabber images...");
            DeleteOldGrabs(60, container);

            log.LogInformation($"Saving status images...");
            SaveStatusFile(grabbers, container);

            log.LogInformation($"Finished");
        }

        /// <summary>
        /// Read the grabber list to get the latest grabber information
        /// </summary>
        private static Grabber[] GetGrabbers(int maximumCount = 999)
        {
            string grabberCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            Grabber[] grabbers = GrabberIO.GrabbersFromCsvUrl(grabberCsvUrl).Result;
            return grabbers.Take(maximumCount).ToArray();
        }

        /// <summary>
        /// Read the JSON status file to update the history of the given grabbers
        /// </summary>
        private static void UpdateGrabberHistory(Grabber[] grabbers, BlobContainerClient container)
        {
            BlobClient blob = container.GetBlobClient(PATH_JSON_STATUSES);
            if (!blob.Exists())
                return;

            using MemoryStream stream = new MemoryStream();
            blob.DownloadTo(stream);
            string json = Encoding.UTF8.GetString(stream.ToArray());

            Grabber[] oldGrabbers = GrabberIO.GrabbersFromJson(json);
            Dictionary<string, Grabber> oldGrabberDictionary = oldGrabbers.ToDictionary(x => x.Info.ID);

            foreach (Grabber grabber in grabbers.Where(x => oldGrabberDictionary.ContainsKey(x.Info.ID)))
                grabber.History.Update(oldGrabberDictionary[grabber.Info.ID].History);
        }

        private static void StoreImageData(Grabber grabber, BlobContainerClient container)
        {
            BlobClient blob = container.GetBlobClient(Path.Combine(PATH_GRAB_PREFIX, grabber.Data.Filename));
            using var stream = new MemoryStream(grabber.Data.Bytes);
            blob.Upload(stream);
        }

        private static void DeleteOldGrabs(int maxAgeMinutes, BlobContainerClient container)
        {
            string[] oldBlobNames = container.GetBlobs()
                .Where(x => (DateTime.UtcNow - x.Properties.LastModified) > TimeSpan.FromMinutes(maxAgeMinutes))
                .Select(x => x.Name)
                .ToArray();

            foreach (var bloboldBlobName in oldBlobNames)
                container.DeleteBlob(bloboldBlobName);
        }

        private static void SaveStatusFile(Grabber[] grabbers, BlobContainerClient container)
        {
            string json = GrabberIO.GrabbersToJson(grabbers);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            BlobClient blob = container.GetBlobClient(PATH_JSON_STATUSES);
            using var stream = new MemoryStream(jsonBytes, writable: false);
            blob.Upload(stream, overwrite: true);
        }
    }
}
