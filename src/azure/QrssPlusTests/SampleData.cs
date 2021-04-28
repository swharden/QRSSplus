using NUnit.Framework;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QrssPlusTests
{
    public static class SampleData
    {
        public static string GRABER_IMAGES_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../data/grabs/");
        public static string GRABBERS_CSV_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../data/grabbers.csv");
        public static int GRABBERS_CSV_COUNT = 121;
        public static string GRABBERSTATUS_JSON_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../data/grabberStatus.json");
        public static int GRABBERSTATUS_JSON_COUNT = 121;

        [Test]
        public static void Test_GrabberListCSV_CanBeParsed()
        {
            Grabber[] grabbers = GrabberIO.GrabbersFromCsvFile(GRABBERS_CSV_PATH);
            Assert.AreEqual(GRABBERS_CSV_COUNT, grabbers.Length);
        }
    }
}
