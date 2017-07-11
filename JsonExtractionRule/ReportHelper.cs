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
        private static int rowKey = 0;

        public static bool Init(string azureStoragAccount, string azureStorageSecret, string testName)
        {
            if (!storageExists && !string.IsNullOrEmpty(azureStoragAccount) && !string.IsNullOrEmpty(azureStorageSecret))
            {
                account = new CloudStorageAccount(new StorageCredentials(azureStoragAccount, azureStorageSecret), true);

                partitionKey = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")+$"_{testName}";
                tableClient = account.CreateCloudTableClient();

                // Create the table if it doesn’t exist. 
                table = tableClient.GetTableReference("BotLoadTestLog");
                table.CreateIfNotExistsAsync();
                storageExists = true;
            }

            return storageExists;
        }
            public static void WriteLog(string convId, string message, string messageId, string userId, string expectedResponse, string actualResponse, string status, string match, string duration, string activityCount, string businessArea = default(string), string luisQnA = default(string))
        {
            if (storageExists)
            {
                LogEntity logEntity = new LogEntity(partitionKey, convId)
                {
                    BusinessArea  =  businessArea,
                    LuisQnA = luisQnA,
                    Message = message,
                    ExpectedResult = expectedResponse,
                    ActualResult = actualResponse,
                    Status = status,
                    Match = match,
                    Duration = string.IsNullOrEmpty(duration) ? 0 : double.Parse(duration) ,
                    Timestamp = DateTime.UtcNow,
                    ActivityCount = activityCount,
                    MessageId = messageId,
                    UserId = userId
                };

                // Create the TableOperation that inserts the customer entity. 
                TableOperation insertOperation = TableOperation.InsertOrReplace(logEntity);

                // Execute the insert operation. 
                table.ExecuteAsync(insertOperation);
            }
        }

        public class LogEntity : TableEntity
        {
            public LogEntity(string partitionKey, string convId)
            {
                this.PartitionKey = partitionKey;
                this.RowKey = (++rowKey).ToString();
                this.ConversationId = convId;
            }

            public string ConversationId { get; set; }

            public string Message { get; set; }

            public string ExpectedResult { get; set; }

            public string ActualResult { get; set; }

            public string Status { get; set; }

            public string Match { get; set; }

            public double Duration { get; set; }

            public string BusinessArea { get; set; }

            public string LuisQnA { get; set; }

            public string ActivityCount { get; set; }

            public string MessageId { get; set; }

            public string UserId { get; set; }
        }
    }
}
