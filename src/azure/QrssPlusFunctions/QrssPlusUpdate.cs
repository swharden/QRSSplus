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
        private const string STATUS_FILENAME = "grabbers.json";
        private const string GRAB_FOLDER_PATH = "grabs/";
        private const string GRAB_FOLDER_URL = "https://qrssplus.z20.web.core.windows.net/grabs/";

        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, "$web");

            DateTime dt = DateTime.UtcNow;
            Grabber[] grabbers = GetGrabbers();
            UpdateGrabberHistory(grabbers, container);
            Parallel.ForEach(grabbers, grabber =>
            {
                grabber.DownloadLatestGrab(dt);
                if (grabber.Data.ContainsNewUniqueImage)
                    StoreImageData(grabber, container);
            });
            DeleteOldGrabs(maxAge: TimeSpan.FromHours(8), container);
            UpdateGrabberURLs(grabbers, container);
            SaveStatusFile(grabbers, container);
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
            BlobClient blob = container.GetBlobClient(STATUS_FILENAME);
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

        /// <summary>
        /// Save a grabber's image data as a new file in blob storage
        /// </summary>
        private static void StoreImageData(Grabber grabber, BlobContainerClient container)
        {
            BlobClient blobOriginal = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename));
            using var streamOriginal = new MemoryStream(grabber.Data.Bytes);
            blobOriginal.Upload(streamOriginal);

            BlobClient blobThumbSkinny = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename + "-thumb-skinny.jpg"));
            using var streamThumbSkinny = new MemoryStream(ImageProcessing.GetThumbnailSkinny(grabber.Data.Bytes));
            blobThumbSkinny.Upload(streamThumbSkinny);

            BlobClient blobThumbAuto = container.GetBlobClient(Path.Combine(GRAB_FOLDER_PATH, grabber.Data.Filename + "-thumb-auto.jpg"));
            using var streamThumbAuto = new MemoryStream(ImageProcessing.GetThumbnailAuto(grabber.Data.Bytes));
            blobThumbAuto.Upload(streamThumbAuto);
        }

        /// <summary>
        /// Delete blob files older than a given age
        /// </summary>
        private static void DeleteOldGrabs(TimeSpan maxAge, BlobContainerClient container)
        {
            string[] oldBlobNames = container.GetBlobs()
                .Where(x => (DateTime.UtcNow - x.Properties.LastModified) > maxAge)
                .Select(x => x.Name)
                .ToArray();

            foreach (var bloboldBlobName in oldBlobNames)
                container.DeleteBlob(bloboldBlobName);
        }

        /// <summary>
        /// Update the grab URLs for each grabber with those currently in blob storage
        /// </summary>
        private static void UpdateGrabberURLs(Grabber[] grabbers, BlobContainerClient container)
        {
            string[] allFilenames = container
                .GetBlobs()
                .Where(x => x.Name.StartsWith(GRAB_FOLDER_PATH))
                .Select(x => Path.GetFileName(x.Name))
                .ToArray();

            foreach (Grabber grabber in grabbers)
                grabber.History.URLs = allFilenames
                    .Where(x => x.StartsWith(grabber.Info.ID))
                    .Select(x => GRAB_FOLDER_URL + x)
                    .ToArray();
        }

        /// <summary>
        /// Create a JSON summary of all grabbers and save it as a flat file in blob storage
        /// </summary>
        private static void SaveStatusFile(Grabber[] grabbers, BlobContainerClient container)
        {
            string json = GrabberIO.GrabbersToJson(grabbers);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            BlobClient blob = container.GetBlobClient(STATUS_FILENAME);
            using var stream = new MemoryStream(jsonBytes, writable: false);
            blob.Upload(stream, overwrite: true);
        }
    }
}
