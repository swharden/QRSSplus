using NUnit.Framework;
using System;

namespace QrssPlus.Tests
{
    public class Tests
    {
        private const string SampleGrabbersCsvPath = "../../../data/grabbers.csv";

        [Test]
        public void Test_GrabberText_HasContent()
        {
            string grabberText = System.IO.File.ReadAllText(SampleGrabbersCsvPath);
            Assert.IsNotNull(grabberText);
        }

        [Test]
        public void Test_GrabberList_ParseGrabberInfos()
        {
            var grabbers = Csv.GetGrabbers(csvFilePath: SampleGrabbersCsvPath);
            Assert.Greater(grabbers.Count, 0);

            foreach (var info in grabbers.GetInfos())
                Console.WriteLine(info);
        }
    }
}