using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            PlotLeaders(loader);
            PlotEachGrabber(loader);
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
            (string[] ids, int[] counts) = GetLeaders(loader, only2020: true);
            var lines = Enumerable.Range(0, ids.Length).Select(x => $"{ids[x]},{counts[x]}");
            System.IO.File.WriteAllLines("output/leaders.csv", lines);
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

        static void PlotLeaders(LogLoader loader, int limit = 50)
        {
            (string[] ids, int[] counts) = GetLeaders(loader, only2020: true);
            double[] xs = ScottPlot.DataGen.Consecutive(limit);
            ids = ids.Take(limit).ToArray();
            double[] counts2 = counts.Take(limit).Select(x => (double)x).ToArray();

            var plt = new ScottPlot.Plot(800, 400);
            plt.PlotBar(xs, counts2);
            plt.Title("Most Active QRSS Grabbers in 2020");
            plt.YLabel("Unique Grabs in 2020");
            plt.AxisAuto();
            plt.TightenLayout();
            plt.Axis(x1: -.5, x2: limit - .25, y1: 0);
            plt.XTicks(ids);
            plt.Ticks(xTickRotation: 90);
            plt.Grid(enableVertical: false);
            plt.Layout(xLabelHeight: 110, yScaleWidth: 50);
            plt.SaveFig("output/grabbers-leader.png");
        }

        static void PlotEachGrabber(LogLoader loader)
        {
            foreach (string id in loader.IDs)
            {
                (double[] dates, double[] unique) = loader.GetUniquePerDay(id);
                int total = (int)unique.Sum();
                if (total < 1000)
                    continue;

                var plt = new ScottPlot.Plot(800, 400);
                plt.PlotScatter(dates, unique);
                plt.Title($"QRSS Grabber Activity for {id} ({total:N0} total grabs)");
                plt.YLabel("Unique Grabs / Day");
                plt.Axis(y1: 0, y2: 160);
                plt.Ticks(dateTimeX: true);
                plt.SaveFig($"output/individual-{id}.png");
                Console.WriteLine($"Saved graph for: {id}");
            }
        }

        static (string[] ids, int[] counts) GetLeaders(LogLoader loader, bool only2020)
        {
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (var log in loader.LogDays)
            {
                if (only2020 && log.DateTime.Year != 2020)
                    continue;

                foreach (string id in log.GrabsByID.Keys)
                {
                    if (!totals.ContainsKey(id))
                        totals.Add(id, 0);
                    totals[id] = totals[id] + log.GrabsByID[id];
                }
            }

            List<KeyValuePair<string, int>> leaders = totals.OrderBy(d => d.Value).Reverse().ToList();
            string[] ids = leaders.Select(x => x.Key).ToArray();
            int[] counts = leaders.Select(x => x.Value).ToArray();
            return (ids, counts);
        }
    }
}
