using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class GrabberListFactory
    {
        public static GrabberList GetFromCSV(string csvFilePath)
        {
            using var reader = new StreamReader(csvFilePath);

            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            GrabberList grabbers = new();

            while (csv.Read())
            {
                if (csv.GetField("#ID").StartsWith("#"))
                    continue;

                GrabberInfo grabber = new()
                {
                    ID = csv.GetField("#ID"),
                    Callsign = csv.GetField("callsign"),
                    Title = csv.GetField("title"),
                    Name = csv.GetField("name"),
                    Location = csv.GetField("location"),
                    SiteUrl = csv.GetField("website"),
                    ImageUrl = csv.GetField("file")
                };
                grabbers.Add(grabber);
            }

            return grabbers;
        }
    }
}
