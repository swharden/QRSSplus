using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus.Tests
{
    [Ignore("Ignore download tests")]
    class DownloadTests
    {
        [Test]
        public void Test_Update_All()
        {
            Cloud.UpdateAll();
        }
    }
}
