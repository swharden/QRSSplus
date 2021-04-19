using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QrssPlus.Tests
{
    [Ignore("Skip Local Download Tests")]
    class DownloadTests
    {
        [Test]
        public void Test_Download_Locally()
        {
            var grabbers = GrabberListFactory.CreateFromCsvFile(SampleData.GrabbersCsvPath);
            string localFolder = Path.GetFullPath("./downloads");

            Web.DownloadAllLocally(grabbers, localFolder, maxCount: 10);

            Assert.NotZero(Directory.GetFiles(localFolder).Length);
        }
    }
}
