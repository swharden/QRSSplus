using NUnit.Framework;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QrssPlusTests
{
    class CsvTests
    {
        [Test]
        public void Test_GrabberList_FromCsvFile()
        {
            string csvFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../data/grabbers.csv");
            Grabber[] grabbers = GrabberFactory.GrabbersFromCsvFile(csvFilePath);
            Assert.AreEqual(121, grabbers.Length);
        }

        [Test]
        [Ignore("Ignore HTTP tests")]
        public void Test_GrabberList_FromCsvUrl()
        {
            string url = "https://raw.githubusercontent.com/swharden/QRSSplus/master/grabbers.csv";
            Grabber[] grabbers = GrabberFactory.GrabbersFromCsvUrl(url).Result;
            Assert.Greater(grabbers.Length, 20);
        }
    }
}
