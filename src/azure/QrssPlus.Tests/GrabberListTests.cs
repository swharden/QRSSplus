using NUnit.Framework;
using System;

namespace QrssPlus.Tests
{
    public class GrabberListTests
    {
        [Test]
        public void Test_GrabberList_FromLocalFile()
        {
            var grabbers = GrabberListFactory.CreateFromCsvFile(SampleData.GrabbersCsvPath);
            Assert.Greater(grabbers.Count, 0);

            Console.WriteLine(grabbers);
            foreach (var grabber in grabbers)
                Console.WriteLine(grabber);
        }

        [Test]
        public void Test_GrabberList_FromUrl()
        {
            var grabbers = GrabberListFactory.CreateFromCsvUrl(SampleData.GrabbersCsvUrl);
            Assert.Greater(grabbers.Count, 0);

            Console.WriteLine(grabbers);
            foreach (var grabber in grabbers)
                Console.WriteLine(grabber);
        }

        [Test]
        public void Test_GrabberList_Filenames()
        {
            var grabbers = GrabberListFactory.CreateFromCsvFile(SampleData.GrabbersCsvPath);

            Console.WriteLine(grabbers);
            foreach (var grabber in grabbers)
                Console.WriteLine(grabber.GetFilename());
        }
    }
}