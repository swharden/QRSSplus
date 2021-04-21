using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QrssPlus.Functions
{
    public static class QrssPlusUpdate
    {
        [FunctionName("QrssPlusUpdate")]
        public static void Run([TimerTrigger("0 2,12,22,32,42,52 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            Cloud.UpdateAll(storageConnectionString);
        }
    }
}
