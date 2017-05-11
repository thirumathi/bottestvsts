using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotConnectorTestApp
{
    public class Token
    {
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Ext_expires_in { get; set; }
        public string Access_token { get; set; }
        public DateTime ExpiryUtc { get; set; }
    }

    public class App
    {
        public string AppKey { get; set; }
        public string AppId { get; set; }
    }

    public class Conversation1
    {
        List<Activity> activities = new List<Activity>();
        public List<Activity> Activities
        {
            get
            {
                return activities;
            }
        }

        public string watermark { get; set; }
    }

    public class Conversation
    {
        public string Id { get; set; }

        Dictionary<int, Activity> activities = new Dictionary<int, Activity>();
        public Dictionary<int, Activity> Activities
        {
            get
            {
                return activities;
            }
        }
    }

    public class Conv
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public string Expires_in { get; set; }
        public string StreamUrl { get; set; }
    }

    /*
    public class Conversations : IDictionary<string, Conversation>
    {
        private DictionaryEntry[] items;
        private Int32 ItemsInUse = 0;

        public Conversations(Int32 numItems)
        {
            items = new DictionaryEntry[numItems];
        }

        public Conversation this[string key]
        {
            get
            {
                // If this key is in the dictionary, return its value.
                Int32 index;
                if (TryGetIndexOfKey(key, out index))
                {
                    // The key was found; return its value.
                    return items[index].Value as Conversation;
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
        public ICollection<Conversation> Values
        {
            get
            {
                // Return an array where each item is a value.
                Conversation[] values = new Conversation[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                    values[n] = items[n].Value as Conversation;
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

        public void Add(string key, Conversation value)
        {
            if (ItemsInUse == items.Length)
                throw new InvalidOperationException("The dictionary cannot hold any more items.");
            items[ItemsInUse++] = new DictionaryEntry(key, value);
        }

        public void Add(KeyValuePair<string, Conversation> item)
        {
            if (ItemsInUse == items.Length)
                throw new InvalidOperationException("The dictionary cannot hold any more items.");
            items[ItemsInUse++] = new DictionaryEntry(item.Key, item.Value);
        }

        public void Clear()
        {
            ItemsInUse = 0;
        }

        public bool Contains(KeyValuePair<string, Conversation> item)
        {
            Conversation value;
            if (!this.TryGetValue(item.Key, out value))
                return false;

            return EqualityComparer<Conversation>.Default.Equals(value, item.Value);
        }

        public bool ContainsKey(string key)
        {
            int index = -1;
            return this.TryGetIndexOfKey(key, out index);
        }

        public void CopyTo(KeyValuePair<string, Conversation>[] array, int arrayIndex)
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
                array[arrayIndex++] = new KeyValuePair<string, Conversation>(item.Key.ToString(), item.Value as Conversation);
            }

        }

        public IEnumerator<KeyValuePair<string, Conversation>> GetEnumerator()
        {
            foreach (DictionaryEntry entry in this.items)
                yield return new KeyValuePair<string, Conversation>(entry.Key.ToString(), entry.Value as Conversation);
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

        public bool Remove(KeyValuePair<string, Conversation> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out Conversation value)
        {
            int index = -1;
            value = null;
            if (this.TryGetIndexOfKey(key, out index))
            {
                value = items[index].Value as Conversation;
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

        protected string GetItem(KeyValuePair<string, Conversation> pair)
        {
            return pair.Key;
        }
    }
    */
}