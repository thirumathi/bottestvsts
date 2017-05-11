using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelperLib
{
    public static class ReportHelper
    {
        static bool storageExists = false;
        static CloudStorageAccount account = default(CloudStorageAccount);
        static string partitionKey;
        static CloudTableClient tableClient;
        static CloudTable table;

        public static bool Init(string azureStoragAccount, string azureStorageSecret)
        {
            if (!storageExists && !string.IsNullOrEmpty(azureStoragAccount) && !string.IsNullOrEmpty(azureStorageSecret))
            {
                account = new CloudStorageAccount(new StorageCredentials(azureStoragAccount, azureStorageSecret), true);

                partitionKey = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
                tableClient = account.CreateCloudTableClient();

                // Create the table if it doesn’t exist. 
                table = tableClient.GetTableReference("BotLoadTestLog");
                table.CreateIfNotExistsAsync();
                storageExists = true;
            }

            return storageExists;
        }
            public static void WriteLog(string condId, string message, string expectedResponse, string actualResponse, string status, double duration)
        {
            if (storageExists)
            {
                LogEntity logEntity = new LogEntity(partitionKey, condId) { Message = message, ExpectedResult = expectedResponse, ActualResult = actualResponse, Status = status, Duration = duration, Timestamp = DateTime.UtcNow };

                // Create the TableOperation that inserts the customer entity. 
                TableOperation insertOperation = TableOperation.Insert(logEntity);

                // Execute the insert operation. 
                table.Execute(insertOperation);
            }
        }

        public class LogEntity : TableEntity
        {
            public LogEntity(string partitionKey, string convId)
            {
                this.PartitionKey = partitionKey;
                this.RowKey = convId;
            }

            public string Message { get; set; }

            public string ExpectedResult { get; set; }

            public string ActualResult { get; set; }

            public string Status { get; set; }

            public double Duration { get; set; }
        }
    }
}
