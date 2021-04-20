using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QrssPlus
{
    public static class Web
    {
        private static void EmptyLocalDownloadFolder(string folder)
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
        }

        public static void DownloadAllLocally(GrabberList grabbers, string folder, int maxCount = int.MaxValue, int maxSimultaneous = 5)
        {
            EmptyLocalDownloadFolder(folder);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var limitedGrabbers = grabbers.Take(maxCount);
            Parallel.ForEach(
                source: limitedGrabbers,
                parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = maxSimultaneous },
                body: grabber => { DownloadFile(grabber); });
            Console.WriteLine($"Downloaded {limitedGrabbers.Count()} grabs in {sw.ElapsedMilliseconds} ms");
        }

        public static void DownloadFile(GrabberInfo grabber)
        {
            using WebClient client = new();
            string saveAs = grabber.GetFilename();
            client.DownloadFile(grabber.ImageUrl, saveAs);
        }
    }
}
