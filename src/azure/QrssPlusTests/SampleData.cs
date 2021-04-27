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
        private static readonly string GRABBERS_CSV_PATH = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../data/grabbers.csv");

        /// <summary>
        /// Return an array of grabbers read from the sample CSV file in the test data folder
        /// </summary>
        /// <returns></returns>
        public static Grabber[] GetGrabbers(int maxCount = 999) =>
            GrabberIO.GrabbersFromCsvFile(GRABBERS_CSV_PATH).Take(maxCount).ToArray();

        [Test]
        public static void Test_GrabberListCSV_CanBeParsed()
        {
            Grabber[] grabbers = GetGrabbers();
            Assert.AreEqual(121, grabbers.Length);
        }
    }
}
