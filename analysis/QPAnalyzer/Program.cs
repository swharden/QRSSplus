using System;
using System.Collections.Generic;
using System.Linq;

namespace QPAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            ClearOutputFolder();

            LogLoader loader = new LogLoader("../../../../../analysis/stats");

            TotalGrabs(loader);
            Leaderboard(loader);

            PlotGrabsPerDay(loader);
            PlotStationsPerDay(loader);
        }

        static void ClearOutputFolder()
        {
            if (!System.IO.Directory.Exists("output"))
                System.IO.Directory.CreateDirectory("output");
            foreach (string oldImage in System.IO.Directory.GetFiles("output", "*.png"))
                System.IO.File.Delete(oldImage);
            foreach (string oldImage in System.IO.Directory.GetFiles("output", "*.csv"))
                System.IO.File.Delete(oldImage);
        }

        static void TotalGrabs(LogLoader loader)
        {
            int grabs2020 = loader.LogDays.Where(x => x.DateTime.Year == 2020).Select(x => x.TotalGrabs).Sum();
            Console.Write($"Total number of grabs in 2020: {grabs2020:N0}");
        }

        static void Leaderboard(LogLoader loader)
        {
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (var log in loader.LogDays)
            {
                if (log.DateTime.Year != 2020)
                    continue;

                foreach(string id in log.GrabsByID.Keys)
                {
                    if (!totals.ContainsKey(id))
                        totals.Add(id, 0);
                    totals[id] = totals[id] + log.GrabsByID[id];
                }
            }

            List<KeyValuePair<string, int>> leaders = totals.OrderBy(d => d.Value).Reverse().ToList();
            string[] leaderLines = leaders.Select(x => $"{x.Key},{x.Value}").ToArray();
            System.IO.File.WriteAllLines("output/leaders.csv", leaderLines);
        }

        static void PlotGrabsPerDay(LogLoader loader)
        {
            double[] grabsPerDay = loader.LogDays.Select(x => (double)x.TotalGrabs).ToArray();

            var plt = new ScottPlot.Plot(600, 400);
            plt.PlotScatter(loader.Days, grabsPerDay, markerSize: 3);
            plt.Ticks(dateTimeX: true);
            plt.Title("Unique Graber Images");
            plt.YLabel("Images / Day");
            plt.Axis(y1: 0);
            plt.SaveFig("output/grabs-per-day.png");
        }

        static void PlotStationsPerDay(LogLoader loader)
        {
            double[] idsPerDay = loader.LogDays.Select(x => (double)x.TotalIDs).ToArray();

            var plt = new ScottPlot.Plot(600, 400);
            plt.PlotScatter(loader.Days, idsPerDay, markerSize: 3);
            plt.Ticks(dateTimeX: true);
            plt.Title("Active Grabber Stations");
            plt.YLabel("Stations / Day");
            plt.Axis(y1: 0);
            plt.SaveFig("output/grabbers-per-day.png");
        }
    }
}
