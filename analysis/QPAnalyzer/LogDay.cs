using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QPAnalyzer
{
    public class LogDay
    {
        public readonly DateTime DateTime;
        public readonly Dictionary<string, int> GrabsByID = new Dictionary<string, int>();
        public readonly int TotalIDs;
        public readonly int TotalGrabs;
        public string Day => $"{DateTime.Year}-{DateTime.Month}-{DateTime.Day}";
        public override string ToString() => $"{Day} has data from {TotalIDs} grabbers ({TotalGrabs} unique images)";
        public string[] GetIDs() => GrabsByID.Keys.ToArray();

        public LogDay(string filePath)
        {
            filePath = System.IO.Path.GetFullPath(filePath);
            var sw = Stopwatch.StartNew();

            LogLine[] logLines = System.IO.File.ReadAllLines(filePath)
                                               .Where(x => string.IsNullOrWhiteSpace(x) == false)
                                               .Select(x => new LogLine(x))
                                               .ToArray();

            DateTime = logLines[0].DateTime;

            string[] ids = logLines.SelectMany(x => x.GetIDs()).Distinct().ToArray();
            foreach (string id in ids)
            {
                int uniqueGrabs = logLines.Select(x => x.GetHashForID(id))
                                          .Where(x => string.IsNullOrWhiteSpace(x) == false)
                                          .Distinct()
                                          .Count();

                // require 1hr of updates (6 grabs) per day to qualify
                if (uniqueGrabs >= 6)
                {
                    TotalIDs += 1;
                    TotalGrabs += uniqueGrabs;
                    GrabsByID.Add(id, uniqueGrabs);
                }
            }
        }
    }
}
