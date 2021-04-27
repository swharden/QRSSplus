using NUnit.Framework;
using QrssPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace QrssPlusTests
{
    class JsonTests
    {
        [Test]
        public void Test_Json_Status()
        {
            Grabber[] grabbers = SampleData.GetGrabbers();
            string json = GrabberIO.GrabbersToJson(grabbers);
            Console.WriteLine(json);
            Assert.Greater(json.Length, 1000);
        }
    }
}
