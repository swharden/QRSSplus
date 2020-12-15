using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QPAnalyzer
{
    public class LogLoader
    {
        public readonly List<LogDay> LogDays = new List<LogDay>();
        public readonly string[] IDs;
        public readonly double[] Days;

        public LogLoader(string statsFolderPath)
        {
            var sw = Stopwatch.StartNew();
            string[] filePaths = System.IO.Directory.GetFiles(statsFolderPath, "*.txt");
            filePaths = filePaths.Select(x => System.IO.Path.GetFullPath(x)).ToArray();

            LogDay firstLogDay = new LogDay(filePaths[0]);
            for (int i = 0; i < filePaths.Length; i++)
            {
                double frac = 100.0 * (i + 1) / filePaths.Length;
                var day = new LogDay(filePaths[i]);
                LogDays.Add(day);
                Console.WriteLine($"[{frac:N2}%] {day}");

                // verify that no days were ever skipped
                if (day.DateTime.Day != firstLogDay.DateTime.AddDays(i).Day)
                    throw new InvalidOperationException("missing data");
            }

            IDs = LogDays.SelectMany(x => x.GetIDs()).Distinct().ToArray();
            Days = LogDays.Select(x => x.DateTime.ToOADate()).ToArray();
            Console.WriteLine($"Total IDs: {IDs.Length}");

            Save();
        }

        public void Save()
        {
            string[] ids = LogDays.SelectMany(x => x.GrabsByID.Keys).Distinct().ToArray();
            Array.Sort(ids);

            DateTime[] dates = LogDays.Select(x => x.DateTime).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("id," + string.Join(',', dates.Select(x => x.ToString())));

            foreach (string id in ids)
            {
                sb.Append(id + ",");
                foreach (var logDay in LogDays)
                {
                    int grabs = logDay.GrabsByID.ContainsKey(id) ? logDay.GrabsByID[id] : 0;
                    sb.Append(grabs + ",");
                }
                sb.AppendLine("");
            }

            System.IO.File.WriteAllText("output/grabs.csv", sb.ToString());
        }

        public (double[] days, double[] unique) GetUniquePerDay(string id)
        {
            double[] counts = new double[Days.Length];
            for (int i = 0; i < counts.Length; i++)
                counts[i] = LogDays[i].GrabsByID.ContainsKey(id) ? LogDays[i].GrabsByID[id] : 0;
            return (Days, counts);
        }
    }
}
