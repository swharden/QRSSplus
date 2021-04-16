using NUnit.Framework;
using System;

namespace QrssPlus.Tests
{
    public class GrabberListTests
    {
        [Test]
        public void Test_GrabberText_HasContent()
        {
            string grabberText = System.IO.File.ReadAllText(SampleData.GrabbersCsvPath);
            Assert.IsNotNull(grabberText);
        }

        [Test]
        public void Test_GrabberList_ParseGrabberInfos()
        {
            var grabbers = GrabberListFactory.GetFromCSV(SampleData.GrabbersCsvPath);
            Assert.Greater(grabbers.Count, 0);

            foreach (var grabber in grabbers)
                Console.WriteLine(grabber);
        }

        [Test]
        public void Test_GrabberList_Filenames()
        {
            var grabbers = GrabberListFactory.GetFromCSV(SampleData.GrabbersCsvPath);
            foreach (var grabber in grabbers)
                Console.WriteLine(grabber.GetFilename());
        }
    }
}