using NUnit.Framework;
using System;

namespace QrssPlus.Tests
{
    public class Tests
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

            foreach (var info in grabbers)
                Console.WriteLine(info);
        }
    }
}