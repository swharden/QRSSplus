using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace QrssPlus.Tests
{
    [Ignore("Ignore download tests")]
    class DownloadTests
    {
        [Test]
        public void Test_Hash_AllGrabs()
        {
            var grabbers = GrabberListFactory.CreateFromCsvFile(SampleData.GrabbersCsvPath);

            foreach (var grabber in grabbers)
            {
                GrabResult grab = grabber.Download();
                Console.WriteLine(grab);
                break;
            }
        }

        [Test]
        public void Test_Download_Hash()
        {
            using WebClient client = new();

            byte[] data = client.DownloadData(SampleData.SampleImageUrl);
            Assert.AreEqual(SampleData.SampleImageSize, data.Length);

            string hash = GrabData.Hash(data);
            Assert.AreEqual(SampleData.SampleImageHash, hash);
        }
    }
}
