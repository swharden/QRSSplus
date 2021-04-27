using NUnit.Framework;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace QrssPlusTests
{
    class JsonTests
    {
        [Test]
        public void Test_Status_GrabbersToJson()
        {
            Grabber[] grabbers = GrabberIO.GrabbersFromCsvFile(SampleData.GRABBERS_CSV_PATH);
            string json = GrabberIO.GrabbersToJson(grabbers);
            Console.WriteLine(json);
            Assert.Greater(json.Length, 1000);
        }

        [Test]
        public void Test_Status_GrabbersFromJson()
        {
            string json = File.ReadAllText(SampleData.GRABBERSTATUS_JSON_PATH);
            Grabber[] grabbers = GrabberIO.GrabbersFromJson(json);
            Assert.AreEqual(SampleData.GRABBERSTATUS_JSON_COUNT, grabbers.Count());
        }
    }
}