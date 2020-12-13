using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QPAnalyzer.Tests
{
    public class TestAnalysis
    {
        readonly GrabRecords grabs = new GrabRecords();

        //[OneTimeSetUp]
        public void LoadAllRecords()
        {
            var sw = Stopwatch.StartNew();
            string folder = "../../../../../analysis/stats";
            string[] filePaths = System.IO.Directory.GetFiles(folder, "*.txt");
            foreach (string filePath in filePaths.Take(50))
                grabs.AddLogFile(filePath);

            Console.WriteLine($"Loaded {grabs.RecordCount} records in {sw.Elapsed} sec");
        }

        //[Test]
        public void Test_Analyze_ByCall()
        {
            double[] days = grabs.GetAllDays().Select(x => x.ToOADate()).ToArray();
            foreach (string id in grabs.GetAllIDs())
            {
                double[] counts = grabs.GetGrabCountByDay(id);
                var plt = new ScottPlot.Plot(600, 400);
                plt.YLabel("Unique Grabs / Day");
                plt.Title(id);
                plt.PlotScatter(days, counts);
                plt.Ticks(dateTimeX: true);
                plt.Axis(y1: 0);
                plt.SaveFig($"grabsByID-{id}.png");
            }
        }
    }
}
