using NUnit.Framework;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlusTests
{
    [Ignore("Ignore HTTP tests")]
    class DownloadTests
    {
        [Test]
        public void Test_Download_ImageData()
        {
            var grabbers = SampleData.GetGrabbers();
            DateTime dt = DateTime.UtcNow;

            Parallel.ForEach(grabbers, grabber =>
            {
                grabber.DownloadLatestGrab(dt);
                Console.WriteLine(grabber);
            });
        }

        [Test]
        public void Test_Download_GrabberListCSV()
        {
            string url = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            Grabber[] grabbers = GrabberIO.GrabbersFromCsvUrl(url).Result;
            Assert.Greater(grabbers.Length, 20);
        }
    }
}
