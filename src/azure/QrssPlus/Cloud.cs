using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class Cloud
    {
        public static string GetStorageConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GrabberInfo>()
                .AddEnvironmentVariables()
                .Build();

            string connectionString = configuration["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("cannot find connection string");

            return connectionString;
        }

        /// <summary>
        /// This is called once every 10 minutes.
        /// </summary>
        public static void UpdateAll()
        {
            const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            const int maxAgeMinutes = 60;

            if (string.IsNullOrEmpty(Cloud.GetStorageConnectionString()))
                throw new InvalidOperationException("null connection string");

            Stopwatch sw = Stopwatch.StartNew();

            GrabberList grabbers = GrabberListFactory.CreateFromCsvUrl(GrabbersCsvUrl);

            // download data
            Parallel.ForEach(grabbers, grabber => { grabber.Grab.Download(); });

            // add results to table storage
            TableStorage.TableAction.UpdateGrabberHashes(grabbers, maxAgeMinutes);

            // upload images to blob storage
            FileStorage.FileAction.UpdateFiles(grabbers, maxAgeMinutes);

            // update the run log
            TableStorage.RunResult run = new()
            {
                RunTime = sw.Elapsed.TotalSeconds,
                Grabbers = grabbers.Count,
                Errors = grabbers.Where(x => x.Grab.Hash.Contains(" ")).Count()
            };
            TableStorage.TableAction.AddRunLog(run).Wait();
        }
    }
}
