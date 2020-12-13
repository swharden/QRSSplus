using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPAnalayzer
{
    public class GrabRecords
    {
        public readonly List<GrabRecord> Records = new List<GrabRecord>();

        public GrabRecords()
        {

        }

        public void Add(DateTime dt, string id, string hash) =>
            Records.Add(new GrabRecord(dt, id, hash));

        public void Add(GrabRecord record) =>
            Records.Add(record);

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
                Records.Add(new GrabRecord(timestamp, id, hash));
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
    }
}
