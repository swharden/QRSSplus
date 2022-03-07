using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QrssPlusTests
{
    internal class CsvTests
    {
        [Test]
        public void Test_VerifyCsv_NoDuplicateIDs()
        {
            string csvFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../../../grabbers.csv");
            csvFilePath = Path.GetFullPath(csvFilePath);
            if (!File.Exists(csvFilePath))
                throw new FileNotFoundException(csvFilePath);

            string csv = File.ReadAllText(csvFilePath);

            QrssPlus.Grabber[] grabbers = QrssPlus.GrabberIO.GrabbersFromCsvText(csv);

            var ids = new HashSet<string>();
            foreach (var grabber in grabbers)
            {
                if (ids.Contains(grabber.Info.ID))
                    throw new InvalidOperationException($"Duplicate ID: {grabber.Info.ID}");

                ids.Add(grabber.Info.ID);
            }
        }
    }
}
