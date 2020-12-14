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
        }
    }
}
