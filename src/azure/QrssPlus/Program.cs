using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(GetStorageConnectionString());
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("test2");
            table.CreateIfNotExists();

            var grab = new GrabResult("testID", "testHash", 123, 12.345);

            TableOperation insertGrab = TableOperation.InsertOrMerge(grab);
            table.Execute(insertGrab);

            Console.WriteLine("DONE");
        }

        public static string GetStorageConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GrabberInfo>()
                .AddEnvironmentVariables()
                .Build();

            string connectionString = configuration["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("cannot find connection string");

            return connectionString;
        }
    }
}
