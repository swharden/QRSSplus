using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QrssPlus.Functions
{
    public static class QrssPlusUpdate
    {
        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GrabberStatus");
            table.CreateIfNotExists();

            DateTime dt = DateTime.UtcNow;

            Console.WriteLine($"Downloading CSV data...");
            GrabberList grabbers = GrabbersFromCsvUrl();

            Console.WriteLine($"Reading old table...");
            UpdateFromTable(table, grabbers);

            Console.WriteLine($"Downloading grabs...");
            Parallel.ForEach(grabbers, grabber => { grabber.Download(dt); });

            Console.WriteLine($"Updating table...");
            UpdateTable(table, grabbers);

            Console.WriteLine($"Uploading images...");
            StoreImages();

            Console.WriteLine($"DONE");
        }

        /// <summary>
        /// Instantiate a grabber list from CSV data online
        /// </summary>
        private static GrabberList GrabbersFromCsvUrl()
        {
            const string url = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            using WebClient client = new();
            string[] lines = client.DownloadString(url).Split("\n");

            GrabberList grabbers = new();
            foreach (string line in lines)
            {
                var grabber = Validate.GrabberInfoFromCsvLine(line);
                if (grabber != null)
                    grabbers.Add(grabber);
            }
            return grabbers;
        }

        /// <summary>
        /// Update an existing grabber list from details saved during the last run
        /// </summary>
        private static void UpdateFromTable(CloudTable table, GrabberList newGrabberList)
        {
            var oldGrabberList = table.CreateQuery<GrabberInfo>();

            foreach (GrabberInfo newGrabber in newGrabberList)
            {
                foreach (GrabberInfo oldGrabber in oldGrabberList)
                {
                    if (newGrabber.ID == oldGrabber.ID)
                    {
                        newGrabber.UpdateFromLast(oldGrabber);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Put the latest results into the storage table
        /// </summary>
        private static void UpdateTable(CloudTable table, GrabberList newGrabberList)
        {
            foreach (GrabberInfo grabber in newGrabberList)
            {
                TableOperation operation = TableOperation.InsertOrMerge(grabber);
                table.Execute(operation);
            }
        }

        /// <summary>
        /// Write image data blobs
        /// </summary>
        private static void StoreImages()
        {

        }
    }
}
