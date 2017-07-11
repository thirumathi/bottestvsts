using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using BotCustomConnectorSvc.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BotCustomConnectorSvc.Helpers
{
    public static class RedisHelper
    {
        private static readonly RedisActivityStore<Activity> RedisActivityStore;
        private static readonly RedisStringStore<StateData> RedisStringStore;
        private static readonly RedisStringStore<Conversation> RedisConversationStore;

        static RedisHelper()
        {
            ConnectionMultiplexer redisConnection = RedisConnectionFactory.GetConnection();
            IDatabase db = redisConnection.GetDatabase();

            //hash demo
            RedisActivityStore = new RedisActivityStore<Activity>(db);
            RedisStringStore = new RedisStringStore<StateData>(db);
            RedisConversationStore = new RedisStringStore<Conversation>(db);
        }

        public static Activity GetLastActivity(string conversationId)
        {
            return RedisActivityStore.GetLastKeyValue(conversationId);
        }

        public static List<Activity> GetAllActivities(string conversationId)
        {
            return RedisActivityStore.GetAllKeyValues(conversationId);
        }

        public static Activity GetActivity(string conversationId, int messageId)
        {
            return RedisActivityStore.Get(conversationId, messageId, true);
        }

        public static void SaveActivity(string conversationId, Activity activity)
        {
            RedisActivityStore.Save(conversationId, activity, activity.InternalId);
        }

        public static void DeleteActivity(string conversationId, int messageId)
        {
            RedisActivityStore.Delete(conversationId, messageId);
        }

        public static void DeleteConversation(string conversationId)
        {
            Activity activity = RedisActivityStore.GetLastKeyValue(conversationId);
            if (activity != null)
            {
                RedisActivityStore.Delete(conversationId, activity.InternalId);
            }
        }

        public static void FlushConversations()
        {
            List<RedisKey> keys = RedisConversationStore.GetAllKeys("*");
            foreach (var key in keys)
            {
                Activity activity = RedisActivityStore.GetLastKeyValue(key);
                if (activity != null)
                {
                    RedisActivityStore.Delete(key, activity.InternalId);
                }
            }
        }

        public static void SaveConversation(Conversation conv)
        {
            RedisConversationStore.Save(conv.Id, conv);
        }

        public static void GetAllConversations(Conversation conv)
        {
            RedisConversationStore.Get("*", true);
        }

        public static StateData GetStateData(string id)
        {
            return RedisStringStore.Get(id);
        }

        public static void SaveStateData(string key, StateData stateData)
        {
            RedisStringStore.Save(key, stateData);
        }

        public static void DeleteStateData(string key)
        {
            RedisStringStore.Delete(key);
        }

        public static void FlushStateData()
        {
            List<RedisKey> keys = RedisStringStore.GetAllKeys("*");
            foreach (var key in keys)
            {
                RedisStringStore.Delete(key);
            }
        }
    }
    public class RedisConnectionFactory
    {

        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnectionFactory()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"].ToString();

                var options = ConfigurationOptions.Parse(connectionString);
                options.SyncTimeout = 30000;
                options.AbortOnConnectFail = false;

                Connection = new Lazy<ConnectionMultiplexer>(() =>
                    ConnectionMultiplexer.Connect(options)
                );
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }

    public class RedisActivityStore<T>
    {
        private readonly IDatabase _db;
        private IServer server;

        public RedisActivityStore(IDatabase db)
        {
            _db = db;
            server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        }

        public T GetLastKeyValue(string key)
        {
            T t = default(T);

            try
            {
                key = GenerateKey(key);
                //string pattern = string.Concat(prefix, prefix.Contains('*') ? string.Empty : "*");
                //var key = server.Keys(pattern: pattern).OrderByDescending(k => k).FirstOrDefault();
                RedisValue value = _db.SortedSetRangeByScore(key, order: Order.Descending).FirstOrDefault();
                
                if (value.HasValue)
                {
                    t = JsonConvert.DeserializeObject<T>(value);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
            return t;
        }

        public List<T> GetAllKeyValues(string key)
        {
            List<T> values = new List<T>();

            key = GenerateKey(key);

            RedisValue[] rValues = _db.SortedSetRangeByScore(key, order: Order.Ascending);
            foreach (var value in rValues)
            {
                values.Add(JsonConvert.DeserializeObject<T>(value));
            }
            return values;
        }

        public T Get(string key, int score, bool addPrefix = true)
        {
            if (addPrefix)
                key = GenerateKey(key);

            T t = default(T);
            RedisValue value = _db.SortedSetRangeByScore(key, score, score).FirstOrDefault();

            if (value.HasValue)
            {
                t = JsonConvert.DeserializeObject<T>(value);
            }

            return t;
        }

        public void Save(string key, T obj, double score)
        {
            if (obj != null)
            {
                key = GenerateKey(key);
                _db.SortedSetAddAsync(key, JsonConvert.SerializeObject(obj), score, When.NotExists);
            }
        }

        public void Delete(string key, int lastItemScore)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Contains(":"))
                throw new ArgumentException("invalid key");

            key = GenerateKey(key);

            _db.SortedSetRemoveRangeByScoreAsync(key, 1, lastItemScore);
        }

        #region Helpers

        string GenerateKey(string key) =>

            string.Concat(NameOfT.ToLower(), ":", key.ToLower());

        Type TypeOfT { get { return typeof(T); } }

        string NameOfT { get { return TypeOfT.FullName; } }

        PropertyInfo[] PropertiesOfT { get { return TypeOfT.GetProperties(); } }

        #endregion

    }

    public class RedisStringStore<T>
    {
        private readonly IDatabase _db;
        private IServer server;

        public RedisStringStore(IDatabase db)
        {
            _db = db;
            server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        }

        public T Get(string key, bool addPrefix = true)
        {
            if (addPrefix)
                key = GenerateKey(key);

            T t = default(T);
            RedisValue value = _db.StringGet(key);

            if (value.HasValue)
            {
                t = JsonConvert.DeserializeObject<T>(value);
            }

            return t;
        }

        public List<RedisKey> GetAllKeys(string key, bool addPrefix = true)
        {
            if (addPrefix)
                key = GenerateKey(key);

            if (!key.Contains('*'))
                key += "*";

            T t = default(T);
            var value = server.Keys(pattern: key);

            return value.ToList();
        }

        public void Save(string key, T obj)
        {
            if (obj != null)
            {
                key = GenerateKey(key);
                _db.StringSet(key, JsonConvert.SerializeObject(obj));
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Contains(":"))
                throw new ArgumentException("invalid key");

            key = GenerateKey(key);
            _db.KeyDeleteAsync(key);
        }

        #region Helpers

        string GenerateKey(string key) =>

            string.Concat(NameOfT.ToLower(), ":", key.ToLower());

        Type TypeOfT { get { return typeof(T); } }

        string NameOfT { get { return TypeOfT.FullName; } }

        #endregion

    }
}