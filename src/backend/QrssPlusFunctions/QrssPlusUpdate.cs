using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using QrssPlus;

namespace QrssPlusFunctions
{
    public static class QrssPlusUpdate
    {
        private const string GRABBERS_JSON_FILENAME = "grabbers.json";
        private const string GRAB_FOLDER_PATH = "grabs/";
        private const string GRAB_FOLDER_URL = "https://qrssplus.z20.web.core.windows.net/grabs/";

        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            DateTime dt = DateTime.UtcNow;
            log.LogInformation($"Starting update at {dt}");

            BlobContainerClient webBlobClient = new BlobContainerClient(
                connectionString: Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process),
                blobContainerName: "$web");

            Grabber[] grabbers = GetGrabbers(log);
            UpdateGrabberHistory(grabbers, webBlobClient, log);

            Parallel.ForEach(grabbers, grabber =>
            {
                grabber.DownloadLatestGrab(dt);
                if (grabber.Data.ContainsNewUniqueImage)
                    StoreImageData(grabber, webBlobClient, log);
            });
            DeleteOldGrabs(maxAge: TimeSpan.FromHours(8), webBlobClient, log);
            UpdateGrabberURLs(grabbers, webBlobClient, log);
            SaveStatusFile(grabbers, webBlobClient, log);
        }

        /// <summary>
        /// Read the grabber list to get the latest grabber information
        /// </summary>
        private static Grabber[] GetGrabbers(ILogger log, int maximumCount = 999)
        {
            string grabberCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            log.LogInformation($"getting list of grabbers from: {grabberCsvUrl}");
            Grabber[] grabbers = GrabberIO.GrabbersFromCsvUrl(grabberCsvUrl).Result;
            return grabbers.Take(maximumCount).ToArray();
        }

        /// <summary>
        /// Read the JSON status file to update the history of the given grabbers
        /// </summary>
        private static void UpdateGrabberHistory(Grabber[] grabbers, BlobContainerClient container, ILogger log)
        {
            log.LogInformation($"reading information from stored grabber file: {GRABBERS_JSON_FILENAME}");
            BlobClient blob = container.GetBlobClient(GRABBERS_JSON_FILENAME);
            if (!blob.Exists())
                return;

            using MemoryStream stream = new MemoryStream();
            blob.DownloadTo(stream);
            string json = Encoding.UTF8.GetString(stream.ToArray());

            Grabber[] oldGrabbers = GrabberIO.GrabbersFromJson(json);
            Dictionary<string, Grabber> oldGrabberDictionary = oldGrabbers.ToDictionary(x => x.Info.ID);

            foreach (Grabber grabber in grabbers.Where(x => oldGrabberDictionary.ContainsKey(x.Info.ID)))
                grabber.History.Update(oldGrabberDictionary[grabber.Info.ID].History);

            log.LogInformation($"read information about {grabbers.Length} grabbers");
        }

        /// <summary>
        /// Save a grabber's image data as a new file in blob storage
        /// </summary>
        private static void StoreImageData(Grabber grabber, BlobContainerClient container, ILogger log)
        {
            log.LogInformation($"storing image for {grabber}");

            BlobHttpHeaders headers = new BlobHttpHeaders() { ContentType = "image/jpeg", ContentLanguage = "en-us", };

            BlobClient blobOriginal = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename));
            using var streamOriginal = new MemoryStream(grabber.Data.Bytes);
            blobOriginal.Upload(streamOriginal);
            blobOriginal.SetHttpHeaders(headers);

            BlobClient blobThumbSkinny = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename + "-thumb-skinny.jpg"));
            using var streamThumbSkinny = new MemoryStream(ImageProcessing.GetThumbnailSkinny(grabber.Data.Bytes));
            blobThumbSkinny.Upload(streamThumbSkinny);
            blobOriginal.SetHttpHeaders(headers);

            BlobClient blobThumbAuto = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename + "-thumb-auto.jpg"));
            using var streamThumbAuto = new MemoryStream(ImageProcessing.GetThumbnailAuto(grabber.Data.Bytes));
            blobThumbAuto.Upload(streamThumbAuto);
            blobOriginal.SetHttpHeaders(headers);
        }

        /// <summary>
        /// Delete blob files older than a given age
        /// </summary>
        private static void DeleteOldGrabs(TimeSpan maxAge, BlobContainerClient container, ILogger log)
        {
            string[] oldBlobNames = container.GetBlobs()
                .Where(x => (DateTime.UtcNow - x.Properties.LastModified) > maxAge)
                .Select(x => x.Name)
                .ToArray();

            log.LogInformation($"Deleting {oldBlobNames.Length} old grab images...");

            foreach (var bloboldBlobName in oldBlobNames)
                container.DeleteBlob(bloboldBlobName);
        }

        /// <summary>
        /// Update the grab URLs for each grabber with those currently in blob storage
        /// </summary>
        private static void UpdateGrabberURLs(Grabber[] grabbers, BlobContainerClient container, ILogger log)
        {
            log.LogInformation($"updating URLs for watched grabbers");

            string[] allFilenames = container
                .GetBlobs()
                .Where(x => x.Name.StartsWith(GRAB_FOLDER_PATH))
                .Select(x => Path.GetFileName(x.Name))
                .ToArray();

            foreach (Grabber grabber in grabbers)
                grabber.History.URLs = allFilenames
                    .Where(x => x.StartsWith(grabber.Info.ID))
                    .Where(x => !x.Contains("-thumb-"))
                    .Select(x => GRAB_FOLDER_URL + x)
                    .ToArray();
        }

        /// <summary>
        /// Create a JSON summary of all grabbers and save it as a flat file in blob storage
        /// </summary>
        private static void SaveStatusFile(Grabber[] grabbers, BlobContainerClient container, ILogger log)
        {
            log.LogInformation($"saving {GRABBERS_JSON_FILENAME}");

            string json = GrabberIO.GrabbersToJson(grabbers);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            BlobClient blob = container.GetBlobClient(GRABBERS_JSON_FILENAME);
            using var stream = new MemoryStream(jsonBytes, writable: false);
            blob.Upload(stream, overwrite: true);

            BlobHttpHeaders headers = new BlobHttpHeaders { ContentType = "text/plain", ContentLanguage = "en-us" };
            blob.SetHttpHeaders(headers);
        }
    }
}
