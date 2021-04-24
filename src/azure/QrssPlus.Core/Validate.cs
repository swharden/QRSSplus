using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QrssPlus.Core
{
    public static class Validate
    {
        /// <summary>
        /// Returns a valid grabber ID given any input string
        /// </summary>
        public static string SanitizeID(string id)
        {
            if (id is null)
                throw new ArgumentException("input cannot be null");

            var validChars = id.ToLower().ToCharArray().Where(c => char.IsLetterOrDigit(c) || c == '-');

            if (validChars.Count() == 0)
                throw new ArgumentException("input contains no valid characters");

            return string.Join("", validChars);
        }

        public static bool IsValidID(string id)
        {
            try
            {
                return id == SanitizeID(id);
            }
            catch
            {
                return false;
            }
        }
    }
}
