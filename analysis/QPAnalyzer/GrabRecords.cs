using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPAnalyzer
{
    public class GrabRecords
    {
        public readonly List<GrabRecord> Records = new List<GrabRecord>();
        public int RecordCount => Records.Count();

        public GrabRecords()
        {

        }

        public string[] GetAllIDs() => Records.Select(x => x.ID).Distinct().ToArray();

        public DateTime[] GetAllDays()
        {
            List<DateTime> days = new List<DateTime>();
            DateTime firstDay = Records.Select(x => x.DateTime).Min();
            DateTime day = new DateTime(firstDay.Year, firstDay.Month, firstDay.Day);
            while (day <= DateTime.Now)
            {
                days.Add(day);
                day = day.AddDays(1);
            }
            return days.ToArray();
        }

        public void Add(GrabRecord record)
        {
            Records.Add(record);
        }

        public void AddLogLine(string logLine)
        {
            throw new NotImplementedException();
        }

        public void AddLogFile(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath)
                                           .Where(x => string.IsNullOrWhiteSpace(x) == false)
                                           .ToArray();

            foreach (string line in lines)
                AddLogLine(line);
        }

        public double[] GetGrabCountByDay(string id)
        {
            DateTime[] days = GetAllDays();
            double[] counts = new double[days.Count()];
            for (int i = 0; i < days.Count(); i++)
                counts[i] = GetUniqueGrabCount(id, days[i]);
            return counts;
        }

        private int GetUniqueGrabCount(string id, DateTime day)
        {
            List<string> hashes = new List<string>();

            foreach (var record in Records)
                if (record.ID != id &&
                    record.DateTime.Date.Year == day.Year &&
                    record.DateTime.Date.Month == day.Month &&
                    record.DateTime.Date.Day == day.Day)
                    hashes.Add(record.Hash);

            return hashes.Distinct().Count();
        }
    }
}
