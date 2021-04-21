using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus.TableStorage
{
    public static class TableAction
    {
        public static void UpdateGrabberHashes(GrabberList grabbers, int maxAgeMinutes)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Cloud.GetStorageConnectionString());
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("GrabResults");
            table.CreateIfNotExists();

            AddNewGrabs(table, grabbers).Wait();
            DeleteOldGrabs(table, maxAgeMinutes).Wait();
        }

        public static async Task AddRunLog(RunResult runResult)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Cloud.GetStorageConnectionString());
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("RunLogs");
            table.CreateIfNotExists();

            TableOperation operation = TableOperation.InsertOrMerge(runResult);
            await table.ExecuteAsync(operation);
        }

        private static async Task AddNewGrabs(CloudTable table, GrabberList grabbers)
        {
            foreach (Grabber grabber in grabbers)
            {
                var result = new GrabResult(grabber);
                TableOperation operation = TableOperation.InsertOrMerge(result);
                await table.ExecuteAsync(operation);
            }
        }

        private static async Task DeleteOldGrabs(CloudTable table, int maxAgeMinutes = 5)
        {
            DateTime oldestDateTime = DateTime.UtcNow - TimeSpan.FromMinutes(maxAgeMinutes);
            var oldItems = table.CreateQuery<GrabResult>().Where(x => x.Timestamp < oldestDateTime);
            foreach (var item in oldItems)
            {
                TableOperation operation = TableOperation.Delete(item);
                await table.ExecuteAsync(operation);
            }
        }
    }
}
