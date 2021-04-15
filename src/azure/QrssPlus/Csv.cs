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

                var grabber = new GrabberInfo(
                    id: csv.GetField("#ID"),
                    callsign: csv.GetField("callsign"),
                    title: csv.GetField("title"),
                    name: csv.GetField("name"),
                    location: csv.GetField("location"),
                    siteUrl: csv.GetField("website"),
                    imageUrl: csv.GetField("file")
                );
                grabbers.Add(grabber);
            }

            return grabbers;
        }
    }
}
