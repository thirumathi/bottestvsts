using BotCustomConnectorSvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace BotCustomConnectorSvc
{
    public static class OldCacheHelper
    {
        static CloudStorageAccount account = default(CloudStorageAccount);
        private static string partitionKey = default(string);
        static CloudTableClient tableClient;
        static CloudTable botStateTable, botConvTable, botErrorTable;
        static bool storageExists = false;

        static bool dataCleanupEnabled = bool.Parse(ConfigurationManager.AppSettings["DataCleanupEnabled"]);

        static OldCacheHelper()
        {
            string azureStoragAccount = ConfigurationManager.AppSettings["AzureStorageAccount"];
            string azureStorageSecret = ConfigurationManager.AppSettings["AzureStorageSecret"];

            if (!string.IsNullOrEmpty(azureStoragAccount) && !string.IsNullOrEmpty(azureStorageSecret))
            {
                account = new CloudStorageAccount(new StorageCredentials(azureStoragAccount, azureStorageSecret), true);

                partitionKey = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
                tableClient = account.CreateCloudTableClient();

                //// Create the botStateTable if it doesn’t exist. 
                //botStateTable = tableClient.GetTableReference("BotState");
                //botStateTable.CreateIfNotExistsAsync();

                //// Create the botStateTable if it doesn’t exist. 
                //botConvTable = tableClient.GetTableReference("BotConversationData");
                //botConvTable.CreateIfNotExistsAsync();

                // Create the botStateTable if it doesn’t exist. 
                botErrorTable = tableClient.GetTableReference("BotConnectorExceptionLog");
                botErrorTable.CreateIfNotExistsAsync();
                storageExists = true;
            }
        }

        public static int GetActivityId(string conversationId, Activity activity)
        {
            Conversation conv = ReadConvFromStorage(conversationId);

            if (conv == null)
            {
                conv = new Conversation() {Id = conversationId};
            }

            int sequence = conv.Activities.Count > 0 ? conv.Activities.Keys.Max() : 0;
            activity.Id = $"{conversationId}|{++sequence}";
            return sequence;
        }

        public static Activity GetConversationActivity(string conversationId, string activityId)
        {
            Conversation conv = ReadConvFromStorage(conversationId);

            Activity outPut = null;

            if (conv != null)
            {
                outPut = conv.Activities.Values.FirstOrDefault(act => act.Id == activityId && act.Delivered);
            }

            return outPut;
        }

        public static Conversation GetConversation(string conversationId, bool everything = false, int watermark = 0)
        {
            Conversation conv = ReadConvFromStorage(conversationId);

            Conversation outPut = new Conversation() {Id = conversationId};

            if (conv != null)
            {
                if (watermark > 0)
                {
                    foreach (KeyValuePair<int, Activity> act in conv.Activities.Where(a => a.Key >= watermark))
                    {
                        if (everything || act.Value.Delivered)
                        {
                            outPut.Activities.Add(act.Key, act.Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, Activity> act in conv.Activities)
                    {
                        if (everything || act.Value.Delivered)
                        {
                            outPut.Activities.Add(act.Key, act.Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return outPut;
        }

        private static void UpdateState(string key, StateData data)
        {
            WriteStateToStorage(key, data);
        }

        public static void UpdateConversationState(string conversationId, StateData data)
        {
            UpdateState(conversationId, data);
        }

        public static void UpdateConversationUserState(string conversationId, string userId, StateData data)
        {
            string key = $"{conversationId}_{userId}";
            UpdateState(key, data);
        }

        public static void UpdateUserState(string userId, StateData data)
        {
            UpdateState(userId, data);
        }

        private static StateData GetStateData(string key)
        {
            StateData stateData = ReadStateFromStorage(key);
            return stateData;
        }

        public static StateData GetConversationUserState(string conversationId, string userId)
        {
            string key = $"{conversationId}_{userId}";
            return GetStateData(key);
        }

        public static StateData GetConversationState(string conversationId)
        {
            return GetStateData(conversationId);
        }

        public static StateData GetUserState(string userId)
        {
            return GetStateData(userId);
        }

        public static string ClearAllConvData()
        {
            if (dataCleanupEnabled)
            {
                DeleteAllConvFromStorage();
                return $"conversation data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearConvData(string convId)
        {
            if (dataCleanupEnabled)
            {
                DeleteConversationFromStorage(convId);
                return $"conversation data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearAllConvStateData()
        {
            if (dataCleanupEnabled)
            {
                ClearAllConvStateData();
                return $"State Data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearConvStateData(string convId)
        {
            if (dataCleanupEnabled)
            {
                DeleteStateDataFromStorage(convId);
                return "State data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static void WriteConversationActivityToStorage(string convId, Activity activity, int activityId)
        {
            Conversation conv = ReadConvFromStorage(convId);

            if (conv == null)
            {
                conv = new Conversation() {Id = convId};
            }

            if (conv.Activities.ContainsKey(activityId))
            {
                conv.Activities[activityId] = activity;
            }
            else
            {
                conv.Activities.Add(activityId, activity);
            }

            Entity entity = new Entity(partitionKey, convId)
            {
                ETag = "*",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(conv)
            };

            TableOperation operation = TableOperation.InsertOrMerge(entity);

            // Execute the insert operation. 
            botConvTable.ExecuteAsync(operation);
        }

        private static Conversation ReadConvFromStorage(string convId)
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation retrieve = TableOperation.Retrieve(partitionKey, convId);
            TableResult result = botConvTable.Execute(retrieve);

            Conversation conversation = null;
            if (result != null && result.Result != null)
            {
                DynamicTableEntity entity = result.Result as DynamicTableEntity;
                conversation =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Conversation>(entity.Properties["Data"].StringValue);
            }

            return conversation;
        }

        private static void DeleteAllConvFromStorage()
        {
            var entityPattern = new DynamicTableEntity();
            entityPattern.PartitionKey = partitionKey;
            entityPattern.ETag = "*";

            botConvTable.ExecuteAsync(TableOperation.Delete(entityPattern));
        }

        private static void DeleteConversationFromStorage(string convId)
        {
            Entity entity = new Entity(partitionKey, convId) {ETag = "*"};
            TableOperation delete = TableOperation.Delete(entity);
            botConvTable.ExecuteAsync(delete);
        }

        private static void WriteStateToStorage(string key, StateData data)
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation retrieve = TableOperation.Retrieve(partitionKey, key);
            TableResult result = botStateTable.Execute(retrieve);

            bool update = result != null && result.Result != null;

            Entity entity = new Entity(partitionKey, key)
            {
                ETag = "*",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(data)
            };

            TableOperation operation = update ? TableOperation.Replace(entity) : TableOperation.InsertOrMerge(entity);

            // Execute the insert operation. 
            botStateTable.ExecuteAsync(operation);
        }

        private static StateData ReadStateFromStorage(string key)
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation retrieve = TableOperation.Retrieve(partitionKey, key);
            TableResult result = botStateTable.Execute(retrieve);

            StateData stateData = null;
            if (result != null && result.Result != null)
            {
                DynamicTableEntity entity = result.Result as DynamicTableEntity;
                stateData = Newtonsoft.Json.JsonConvert.DeserializeObject<StateData>(entity.Properties["Data"]
                    .StringValue);
            }

            return stateData;
        }

        private static void DeleteAllStateDataStorage()
        {
            var entityPattern = new DynamicTableEntity();
            entityPattern.PartitionKey = partitionKey;
            entityPattern.ETag = "*";

            botStateTable.ExecuteAsync(TableOperation.Delete(entityPattern));
        }

        private static void DeleteStateDataFromStorage(string key)
        {
            Entity entity = new Entity(partitionKey, key) {ETag = "*"};
            TableOperation delete = TableOperation.Delete(entity);
            botStateTable.ExecuteAsync(delete);
        }

        public static void WriteLogToStorage(string path, Exception ex)
        {
            ExcpetionEntity entity = new ExcpetionEntity(partitionKey)
            {
                Path = path,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };

            TableOperation operation = TableOperation.InsertOrMerge(entity);

            // Execute the insert operation. 
            botErrorTable.ExecuteAsync(operation);
        }
    }

    public class Entity : TableEntity
    {
        public Entity(string partitionKey, string convId)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = convId;
        }

        public string Data { get; set; }
    }

    public class ExcpetionEntity : TableEntity
    {
        public ExcpetionEntity(string partitionKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string Path { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
