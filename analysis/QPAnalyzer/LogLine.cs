using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPAnalyzer
{
    public class LogLine
    {
        public readonly DateTime DateTime;
        private readonly Dictionary<string, string> HashesByID = new Dictionary<string, string>();

        public LogLine(string logLine)
        {
            string[] parts = logLine.Split(',');
            int callCount = parts.Length - 1;

            if (callCount < 5)
                throw new InvalidOperationException("not enough callsigns seen");

            DateTime = GrabRecord.GetDateTime(parts[0]);

            for (int i = 0; i < callCount; i++)
            {
                string[] grabLineParts = parts[1 + i].Split(' ');
                string id = grabLineParts[0];
                string hash = grabLineParts[1];

                // ignore failed retrievals
                if (hash == "fail")
                    continue;

                // ignore grabber list errors where the same ID is used twice
                if (HashesByID.ContainsKey(id))
                    continue;

                HashesByID.Add(id, hash);
            }
        }

        public string[] GetIDs() => HashesByID.Keys.ToArray();

        public string GetHashForID(string id) => HashesByID.ContainsKey(id) ? HashesByID[id] : null;
    }
}
