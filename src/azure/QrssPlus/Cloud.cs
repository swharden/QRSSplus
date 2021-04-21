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
        /// <summary>
        /// This is called once every 10 minutes.
        /// </summary>
        public static void UpdateAll(string storageConnectionString)
        {
            const string GrabbersCsvUrl = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            const int maxAgeMinutes = 60;

            if (string.IsNullOrEmpty(storageConnectionString))
                throw new InvalidOperationException("null connection string");

            Stopwatch sw = Stopwatch.StartNew();

            GrabberList grabbers = GrabberListFactory.CreateFromCsvUrl(GrabbersCsvUrl);

            // download data
            Parallel.ForEach(grabbers, grabber => { grabber.Grab.Download(); });

            // add results to table storage
            TableStorage.TableAction.UpdateGrabberHashes(grabbers, maxAgeMinutes, storageConnectionString);

            // upload images to blob storage
            FileStorage.FileAction.UpdateFiles(grabbers, maxAgeMinutes, storageConnectionString);

            // update the run log
            TableStorage.RunResult run = new()
            {
                RunTime = sw.Elapsed.TotalSeconds,
                Grabbers = grabbers.Count,
                Errors = grabbers.Where(x => x.Grab.Hash.Contains(" ")).Count()
            };
            TableStorage.TableAction.AddRunLog(run, storageConnectionString).Wait();
        }
    }
}
