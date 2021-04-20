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
    class DownloadTests
    {
        [Test]
        [Ignore("skip large download tests")]
        public void Test_Download_ImageFiles()
        {
            var grabbers = GrabberListFactory.CreateFromCsvFile(SampleData.GrabbersCsvPath);
            string localFolder = Path.GetFullPath("./downloads");

            Web.DownloadAllLocally(grabbers, localFolder, maxCount: 10);

            Assert.NotZero(Directory.GetFiles(localFolder).Length);
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
