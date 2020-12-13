using NUnit.Framework;
using System;
using System.Linq;

namespace QPAnalyzer
{
    public class TestRecords
    {
        [Test]
        public void Test_Timestamp_ConvertsToDate()
        {
            string timestamp = "1809180102";
            DateTime expected = new DateTime(2018, 09, 18, 01, 02, 00);
            DateTime converted = GrabRecord.GetDateTime(timestamp);
            Assert.AreEqual(expected, converted);
        }

        [Test]
        public void Test_Records_FromLogLine()
        {
            string line = "1809180102,7L1RLL e31b23802e,AA7US 423509cfdc,CT2IWW 72e7374cdf,DF2JP bfcd28fe6b,DK7FC 59ee7bcda8,DL4DTL 67c184698d,EA8BVP1 dc4d99d996,EA8BVP2 61b409fb0f,F6BAZ 97aaf6e039,G0MQW1 81cb396d0b,G0MQW2 6a1edc2691,G0MQW3 6a1edc2691,G3VYZ1 e46ea0c6e7,G3VYZ2 0009b8a6df,G3VYZ3 32e6747ddc,G3YXM-600m 22696c62af,G3ZJO db050902af,G4IOG-1 ed5b17a260,G4IOG-2 8fc481a9db,G4JVF e48a492e58,G6AVK 174c3d9af3,G6NHU 4ed74f7368,GJ7RWT 2832917c86,KL7L 1ef63de984,LA5GOA 1cf41a5b35,N2NXZ 63c9f00e9d,OK1FCX 4873af2684,ON4CDJ 342a8e0c3a,PA2OHH-30m 56ceb9c330,PA2OHH-40m fb1b8f0fab,S52AS 8a7d211007,SA6BSS1 c3948b1641,SA6BSS2 f214739972,SA6BSS3 fb28d89eff,VA3ROM d41d8cd98f,VE1VDM e4212f4cb0,VE3GTC 72a7d52fd1,VK3EDW 2a7d9e5516,VE7IGH bf44d27976,WD4AH 43298e7612,WD4ELG-20 308c839da8,WD4ELG-30 e47bf4b64a,WD4ELG-40 8adfeaffd2,WD4ELG-80 0bbf804e81,WD4ELG-160 ff475ba94f,W4HBK1 5b98ab41f7,W4HBK2 736d25967a,WA5DJJ-10m a3a95923b9,WA5DJJ-12m e2ae554abc,WA5DJJ-15m c12fa9ab1d,WA5DJJ-17m 31295fddfc,WA5DJJ-20m 6c4a4ca384,WA5DJJ-30m 7c26330d39,WA5DJJ-40m d7d1121583,WA5DJJ-60m 2514b4e07e,WA5DJJ-80m 5148409fc7,WA5DJJ-160m e6fc58109c,ZL2IK1 305473f141,ZL2IK2 125b439f30";
            var grabs = new GrabRecords();
            grabs.AddLogLine(line);
            Assert.IsNotEmpty(grabs.Records);

            DateTime expected = new DateTime(2018, 09, 18, 01, 02, 00);
            Assert.AreEqual(expected, grabs.Records.First().DateTime);

            Assert.AreEqual("7L1RLL", grabs.Records[0].ID);
            Assert.AreEqual("e31b23802e", grabs.Records[0].Hash);
        }

        [Test]
        public void Test_Records_FromLogFile()
        {
            string filePath = "../../../../../analysis/stats/2018-09-24.txt";

            filePath = System.IO.Path.GetFullPath(filePath);
            Assert.That(System.IO.File.Exists(filePath), $"file does not exist: {filePath}");

            var grabs = new GrabRecords();
            grabs.AddLogFile(filePath);

            Assert.IsNotEmpty(grabs.Records);
            Assert.AreEqual(2018, grabs.Records.First().DateTime.Year);
            Assert.AreEqual(09, grabs.Records.First().DateTime.Month);
            Assert.AreEqual(24, grabs.Records.First().DateTime.Day);
        }

        private GrabRecords AnalyzeLogFiles(int dayLimit)
        {
            var grabs = new GrabRecords();

            string folder = "../../../../../analysis/stats";
            foreach (string filePath in System.IO.Directory.GetFiles(folder, "*.txt"))
            {
                grabs.AddLogFile(filePath);
                if (grabs.DaysWithDataCount >= dayLimit)
                    return grabs;
            }

            return grabs;
        }

        [Test]
        public void Test_Records_ReadAllFiles()
        {
            var grabs = AnalyzeLogFiles(10);
            Console.WriteLine($"Loaded {grabs.RecordCount:N0} records from {grabs.IdCount:N0} grabbers over {grabs.DaysWithDataCount:N0} days");
        }

        [Test]
        public void Test_Records_AllDays()
        {
            var grabs = AnalyzeLogFiles(10);
            foreach (var day in grabs.GetAllDays())
                Console.WriteLine(day);
            Assert.AreEqual(10, grabs.GetAllDays().Length);
        }

        [Test]
        public void Test_Records_AllIDs()
        {
            var grabs = AnalyzeLogFiles(10);
            foreach (string id in grabs.IDs)
                Console.WriteLine(id);
        }
    }
}