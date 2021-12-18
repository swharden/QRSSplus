using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QrssPlus
{
    public class GrabberInfo
    {
        private string _ID;
        public string ID { get => _ID; set { _ID = SanitizeID(value); } }
        public string Callsign;
        public string Title;
        public string Name;
        public string Location;
        public string ImageUrl;
        public string SiteUrl;

        private static string SanitizeID(string id)
        {
            if (id is null)
                throw new ArgumentException("ID cannot be null");

            var validChars = id.ToUpper().ToCharArray().Where(c => char.IsLetterOrDigit(c) || c == '-');

            if (validChars.Count() == 0)
                throw new ArgumentException("ID contains no valid characters");

            return string.Join("", validChars);
        }
    }
}
