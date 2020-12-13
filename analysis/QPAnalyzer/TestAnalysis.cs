using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QPAnalyzer
{
    public class TestAnalysis
    {
        readonly GrabRecords grabs = new GrabRecords();

        [OneTimeSetUp]
        public void LoadAllRecords()
        {
            var sw = Stopwatch.StartNew();
            string folder = "../../../../../analysis/stats";
            foreach (string filePath in System.IO.Directory.GetFiles(folder, "*.txt"))
                grabs.AddLogFile(filePath);

            Console.WriteLine($"Loaded {grabs.RecordCount} records in {sw.Elapsed} sec");
        }

        [Test]
        public void Test_Analyze_AJ4VD()
        {
            double[] counts = grabs.GetGrabCountByDay("AJ4VD");
            double[] days = grabs.GetAllDays().Select(x => x.ToOADate()).ToArray();
            var plt = new ScottPlot.Plot();
            plt.PlotScatter(days, counts);
            plt.Ticks(dateTimeX: true);
        }
    }
}
