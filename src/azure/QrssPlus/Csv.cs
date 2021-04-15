using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace QrssPlus
{
    public static class Csv
    {
        public static GrabberList GetGrabbers(string csvFilePath)
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

                GrabberInfo grabber = new GrabberInfo
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
