using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    public static class GrabData
    {
        public static string Hash(byte[] data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBytes = md5.ComputeHash(data);
            string hashString = string.Join("", hashBytes.Select(x => x.ToString("x2")));
            return hashString;
        }
    }
}
