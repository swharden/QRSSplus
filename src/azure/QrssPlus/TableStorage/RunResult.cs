using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlus.TableStorage
{
    public class RunResult : TableEntity
    {
        public int Grabbers { get; set; }
        public int Errors { get; set; }
        public double RunTime { get; set; }

        public RunResult()
        {
            PartitionKey = "RunResult";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
