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

        public readonly List<DateTime> DaysWithData = new List<DateTime>();
        public int DaysWithDataCount => DaysWithData.Count();

        public readonly List<string> IDs = new List<string>();
        public int IdCount => IDs.Count();

        public GrabRecords()
        {

        }

        /// <summary>
        /// Return an array of all days for which data is available
        /// </summary>
        /// <returns></returns>
        public DateTime[] GetAllDays()
        {
            DaysWithData.Sort();
            List<DateTime> days = new List<DateTime>();

            DateTime day = DaysWithData.First();
            while (day <= DaysWithData.Last())
            {
                days.Add(day);
                day = day.AddDays(1);
            }

            return days.ToArray();
        }

        public void Add(GrabRecord record)
        {
            Records.Add(record);

            DateTime day = record.DateTime.Date;
            if (!DaysWithData.Contains(day))
                DaysWithData.Add(day);

            if (!IDs.Contains(record.ID))
                IDs.Add(record.ID);
        }

        public void AddLogLine(string logLone)
        {
            string[] parts = logLone.Split(',');
            if (parts.Length < 5)
                throw new InvalidOperationException("line doesn't contain enough commas to make sense");

            string timestamp = parts[0];
            foreach (string grabLine in parts.Skip(1))
            {
                string[] grabLineParts = grabLine.Split(' ');
                string id = grabLineParts[0];
                string hash = grabLineParts[1];
                Add(new GrabRecord(timestamp, id, hash));
            }
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

            return hashes.Count();
        }
    }
}
