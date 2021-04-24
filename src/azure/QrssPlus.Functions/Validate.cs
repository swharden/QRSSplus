using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QrssPlus.Functions
{
    public static class Validate
    {
        /// <summary>
        /// Returns a trimmed lowercase ID. Throws an exception if contains special characters (except dash)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string SanitizeID(string id)
        {
            id = id.Trim().ToLower();
            char[] chars = id.ToCharArray();
            if (chars.All(x => char.IsLetterOrDigit(x) || x == '-'))
                return id;
            else
                throw new ArgumentException("id must ");

        }

        /// <summary>
        /// Return a grabber or null if the line cannot be parsed to a grabber
        /// </summary>
        public static GrabberInfo GrabberInfoFromCsvLine(string line)
        {
            line = line.Trim();

            if (line.StartsWith("#"))
                return null;

            string[] parts = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(line);
            if (parts.Length != 7)
                return null;

            return new GrabberInfo(
                id: parts[0],
                callsign: parts[1],
                title: parts[2],
                name: parts[3],
                location: parts[4],
                siteUrl: parts[5],
                imageUrl: parts[6]);
        }
    }
}
