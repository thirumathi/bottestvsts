using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotConnectorTestApp
{
    public class Acti
    {
        public string type;
        public string text;
        public Dictionary<string, string> from;
        public string locale;
        public DateTime timestamp;
        public Dictionary<string, string> channelData;
        public string id;
        public string channelId;
        public DateTime localTimestamp;
        public Dictionary<string, string> recipient;
        public Dictionary<string, string> conversation;
        public string serviceUrl;
        public Dictionary<string, string> membersAdded;

        public Acti()
        {

        }
    }

    public class Conv
    {
        public string Id { get; set; }

        Dictionary<int, Acti> activities = new Dictionary<int, Acti>();
        public Dictionary<int, Acti> Activities
        {
            get
            {
                return activities;
            }
        }
    }

    public class Conversation
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public string Expires_in { get; set; }
        public string StreamUrl { get; set; }
    }

    public class Token
    {
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Ext_expires_in { get; set; }
        public string Access_token { get; set; }
    }

    class App
    {
        public string AppKey { get; set; }
        public string AppId { get; set; }
    }

    public class Activity
    {
        public string type;
        public string text;
        public Dictionary<string, string> from;
        public string locale;
        public DateTime timestamp;
        public Dictionary<string, string> channelData;
        public string id;
        public string channelId;
        public DateTime localTimestamp;
        public Dictionary<string, string> recipient;
        public Dictionary<string, string> conversation;
        public string serviceUrl;
        public string textFormat = "plain";

        public Activity()
        {

        }
    }

    public class Activity1 : Activity
    {
        public Dictionary<string, string> membersAdded;

        public Activity1()
        {

        }
    }
}
