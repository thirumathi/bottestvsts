﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotCustomConnectorSvc.Models
{
    public class ResumptionContext
    {
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        [JsonProperty(PropertyName = "isTrustedServiceUrl")]
        public string IsTrustedServiceUrl { get; set; }

        public ResumptionContext()
        { }

        public ResumptionContext(string locale = default(string), string isTrustedServiceUrl = default(string))
        {
            Locale = locale;
            IsTrustedServiceUrl = isTrustedServiceUrl;
        }
    }

    public class StateData
    {
        [JsonProperty(PropertyName = "data")]
        public StateDataDictionary Data { get; set; }

        [JsonProperty(PropertyName = "eTag")]
        public string Etag { get; set; }

        public StateData()
        { }

        public StateData(StateDataDictionary data = default(StateDataDictionary), string etag = default(string))
        {
            Data = data;
            Etag = etag;
        }
    }

    public class StateDataDictionary : IDictionary<string, object>
    {
        private DictionaryEntry[] items;
        private Int32 ItemsInUse = 0;

        public StateDataDictionary()
        {
            items = new DictionaryEntry[100];
        }

        public StateDataDictionary(Int32 numItems)
        {
            items = new DictionaryEntry[numItems];
        }

        public object this[string key]
        {
            get
            {
                // If this key is in the dictionary, return its value.
                Int32 index;
                if (TryGetIndexOfKey(key, out index))
                {
                    // The key was found; return its value.
                    return items[index].Value as StateData;
                }
                else
                {
                    // The key was not found; return null.
                    return null;
                }
            }

            set
            {
                // If this key is in the dictionary, change its value. 
                Int32 index;
                if (TryGetIndexOfKey(key, out index))
                {
                    // The key was found; change its value.
                    items[index].Value = value;
                }
                else
                {
                    // This key is not in the dictionary; add this key/value pair.
                    Add(key, value);
                }
            }

        }

        public ICollection<string> Keys
        {
            get
            {
                // Return an array where each item is a key.
                string[] keys = new string[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                    keys[n] = items[n].Key as string;
                return keys;
            }
        }
        public ICollection<object> Values
        {
            get
            {
                // Return an array where each item is a value.
                object[] values = new object[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                    values[n] = items[n].Value as object;
                return values;
            }
        }

        public int Count
        {
            get
            {
                return ItemsInUse;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(string key, object value)
        {
            if (ItemsInUse == items.Length)
                throw new InvalidOperationException("The dictionary cannot hold any more items.");
            items[ItemsInUse++] = new DictionaryEntry(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            if (ItemsInUse == items.Length)
                throw new InvalidOperationException("The dictionary cannot hold any more items.");
            items[ItemsInUse++] = new DictionaryEntry(item.Key, item.Value);
        }

        public void Clear()
        {
            ItemsInUse = 0;
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            object value;
            if (!this.TryGetValue(item.Key, out value))
                return false;

            return EqualityComparer<object>.Default.Equals(value, item.Value);
        }

        public bool ContainsKey(string key)
        {
            int index = -1;
            return this.TryGetIndexOfKey(key, out index);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if ((array.Length - arrayIndex) < items.Count())
            {
                throw new ArgumentException("Destination array is not large enough.Check array.Length and arrayIndex.");
            }

            foreach (DictionaryEntry item in items)
            {
                array[arrayIndex++] = new KeyValuePair<string, object>(item.Key.ToString(), item.Value as object);
            }

        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (DictionaryEntry entry in this.items)
            {
                if (entry.Key != null)
                {
                    yield return new KeyValuePair<string, object>(entry.Key.ToString(), entry.Value as object);
                }
            }
        }

        public bool Remove(string key)
        {
            int index = -1;
            if (this.TryGetIndexOfKey(key, out index))
            {
                // If the key is found, slide all the items up.
                Array.Copy(items, index + 1, items, index, ItemsInUse - index - 1);
                ItemsInUse--;
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out object value)
        {
            int index = -1;
            value = null;
            if (this.TryGetIndexOfKey(key, out index))
            {
                value = items[index].Value as object;
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Boolean TryGetIndexOfKey(Object key, out Int32 index)
        {
            for (index = 0; index < ItemsInUse; index++)
            {
                // If the key is found, return true (the index is also returned).
                if (items[index].Key.Equals(key)) return true;
            }

            // Key not found, return false (index should be ignored by the caller).
            return false;
        }

        protected string GetItem(KeyValuePair<string, object> pair)
        {
            return pair.Key;
        }
    }
}
