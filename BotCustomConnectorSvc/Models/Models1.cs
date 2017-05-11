using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotCustomConnectorSvc.Models
{
    public partial class Entity
    {
        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        public Entity() { }

        /// <summary>
        /// Initializes a new instance of the Entity class.
        /// </summary>
        public Entity(string type = default(string))
        {
            Type = type;
        }

        /// <summary>
        /// Entity Type (typically from schema.org types)
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

    }

    public partial class Attachment
    {
        /// <summary>
        /// Initializes a new instance of the Attachment class.
        /// </summary>
        public Attachment() { }

        /// <summary>
        /// Initializes a new instance of the Attachment class.
        /// </summary>
        public Attachment(string contentType = default(string), string contentUrl = default(string), object content = default(object), string name = default(string), string thumbnailUrl = default(string))
        {
            ContentType = contentType;
            ContentUrl = contentUrl;
            Content = content;
            Name = name;
            ThumbnailUrl = thumbnailUrl;
        }

        /// <summary>
        /// mimetype/Contenttype for the file
        /// </summary>
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Content Url
        /// </summary>
        [JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Embedded content
        /// </summary>
        [JsonProperty(PropertyName = "content")]
        public object Content { get; set; }

        /// <summary>
        /// (OPTIONAL) The name of the attachment
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// (OPTIONAL) Thumbnail associated with attachment
        /// </summary>
        [JsonProperty(PropertyName = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

    }

    public partial class ChannelAccount
    {
        /// <summary>
        /// Initializes a new instance of the ChannelAccount class.
        /// </summary>
        public ChannelAccount() { }

        /// <summary>
        /// Initializes a new instance of the ChannelAccount class.
        /// </summary>
        public ChannelAccount(string id = default(string), string name = default(string))
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Channel id for the user or bot on this channel (Example:
        /// joe@smith.com, or @joesmith or 123456)
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Display friendly name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }

    public partial class ConversationAccount
    {
        /// <summary>
        /// Initializes a new instance of the ConversationAccount class.
        /// </summary>
        public ConversationAccount() { }

        /// <summary>
        /// Initializes a new instance of the ConversationAccount class.
        /// </summary>
        public ConversationAccount(bool? isGroup = default(bool?), string id = default(string), string name = default(string))
        {
            IsGroup = isGroup;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Is this a reference to a group
        /// </summary>
        [JsonProperty(PropertyName = "isGroup")]
        public bool? IsGroup { get; set; }

        /// <summary>
        /// Channel id for the user or bot on this channel (Example:
        /// joe@smith.com, or @joesmith or 123456)
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Display friendly name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }

    public class ActionTypes
    {
        /// <summary>
        /// Client will open given url in the built-in browser.
        /// </summary>
        public const string OpenUrl = "openUrl";

        /// <summary>
        /// Client will post message to bot, so all other participants will see that was posted to the bot and who posted this.
        /// </summary>
        public const string ImBack = "imBack";

        /// <summary>
        /// Client will post message to bot privately, so other participants inside conversation will not see that was posted. 
        /// </summary>
        public const string PostBack = "postBack";

        /// <summary>
        /// playback audio container referenced by url
        /// </summary>
        public const string PlayAudio = "playAudio";

        /// <summary>
        /// playback video container referenced by url
        /// </summary>
        public const string PlayVideo = "playVideo";

        /// <summary>
        /// show image referenced by url
        /// </summary>
        public const string ShowImage = "showImage";

        /// <summary>
        /// download file referenced by url
        /// </summary>
        public const string DownloadFile = "downloadFile";

        /// <summary>
        /// Signin button
        /// </summary>
        public const string Signin = "signin";
    }

    public partial class CardAction
    {
        /// <summary>
        /// Initializes a new instance of the CardAction class.
        /// </summary>
        public CardAction()
        {
            this.Type = ActionTypes.ImBack;
        }

        /// <summary>
        /// Initializes a new instance of the CardAction class.
        /// </summary>
        public CardAction(string type, string title = default(string), string image = default(string), object value = default(object)) : this()
        {
            Type = type;
            Title = title;
            Image = image;
            Value = value;
        }

        /// <summary>
        /// Defines the type of action implemented by this button. Defaults to <see cref="ActionTypes.ImBack"/>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Text description which appear on the button.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// URL Picture which will appear on the button, next to text label.
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        /// <summary>
        /// Supplementary parameter for action. Content of this property
        /// depends on the ActionType
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

    }
    public partial class SuggestedActions
    {
        /// <summary>
        /// Initializes a new instance of the SuggestedActions class.
        /// </summary>
        public SuggestedActions() { }

        /// <summary>
        /// Initializes a new instance of the SuggestedActions class.
        /// </summary>
        public SuggestedActions(IList<string> to = default(IList<string>), IList<CardAction> actions = default(IList<CardAction>))
        {
            To = to;
            Actions = actions;
        }

        /// <summary>
        /// Ids of the recipients that the actions should be shown to.  These
        /// Ids are relative to the channelId and a subset of all recipients
        /// of the activity
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public IList<string> To { get; set; }

        /// <summary>
        /// Actions that can be shown to the user
        /// </summary>
        [JsonProperty(PropertyName = "actions")]
        public IList<CardAction> Actions { get; set; }

    }

    public partial class ConversationReference
    {
        /// <summary>
        /// Initializes a new instance of the ConversationReference class.
        /// </summary>
        public ConversationReference() { }

        /// <summary>
        /// Initializes a new instance of the ConversationReference class.
        /// </summary>
        public ConversationReference(string activityId = default(string), ChannelAccount user = default(ChannelAccount), ChannelAccount bot = default(ChannelAccount), ConversationAccount conversation = default(ConversationAccount), string channelId = default(string), string serviceUrl = default(string))
        {
            ActivityId = activityId;
            User = user;
            Bot = bot;
            Conversation = conversation;
            ChannelId = channelId;
            ServiceUrl = serviceUrl;
        }

        /// <summary>
        /// (Optional) ID of the activity to refer to
        /// </summary>
        [JsonProperty(PropertyName = "activityId")]
        public string ActivityId { get; set; }

        /// <summary>
        /// (Optional) User participating in this conversation
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public ChannelAccount User { get; set; }

        /// <summary>
        /// Bot participating in this conversation
        /// </summary>
        [JsonProperty(PropertyName = "bot")]
        public ChannelAccount Bot { get; set; }

        /// <summary>
        /// Conversation reference
        /// </summary>
        [JsonProperty(PropertyName = "conversation")]
        public ConversationAccount Conversation { get; set; }

        /// <summary>
        /// Channel ID
        /// </summary>
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Service endpoint where operations concerning the referenced
        /// conversation may be performed
        /// </summary>
        [JsonProperty(PropertyName = "serviceUrl")]
        public string ServiceUrl { get; set; }

    }

    public partial class Activity
    {
        /// <summary>
        /// Initializes a new instance of the Activity class.
        /// </summary>
        public Activity()
        {
            Attachments = new List<Attachment>();
            Entities = new List<Entity>();
            MembersAdded = new List<ChannelAccount>();
            MembersRemoved = new List<ChannelAccount>();
        }

        /// <summary>
        /// Initializes a new instance of the Activity class.
        /// </summary>
        public Activity(string type = default(string), string id = default(string), DateTime? timestamp = default(DateTime?), DateTime? localTimestamp = default(DateTime?), string serviceUrl = default(string), string channelId = default(string), ChannelAccount from = default(ChannelAccount), ConversationAccount conversation = default(ConversationAccount), ChannelAccount recipient = default(ChannelAccount), string textFormat = default(string), string attachmentLayout = default(string), IList<ChannelAccount> membersAdded = default(IList<ChannelAccount>), IList<ChannelAccount> membersRemoved = default(IList<ChannelAccount>), string topicName = default(string), bool? historyDisclosed = default(bool?), string locale = default(string), string text = default(string), string speak = default(string), string inputHint = default(string), string summary = default(string), SuggestedActions suggestedActions = default(SuggestedActions), IList<Attachment> attachments = default(IList<Attachment>), IList<Entity> entities = default(IList<Entity>), object channelData = default(object), string action = default(string), string replyToId = default(string), object value = default(object), string name = default(string), ConversationReference relatesTo = default(ConversationReference), string code = default(string)) : this()
        {
            Type = type;
            Id = id;
            Timestamp = timestamp;
            LocalTimestamp = localTimestamp;
            ServiceUrl = serviceUrl;
            ChannelId = channelId;
            From = from;
            Conversation = conversation;
            Recipient = recipient;
            TextFormat = textFormat;
            AttachmentLayout = attachmentLayout;
            TopicName = topicName;
            HistoryDisclosed = historyDisclosed;
            Locale = locale;
            Text = text;
            Speak = speak;
            InputHint = inputHint;
            Summary = summary;
            SuggestedActions = suggestedActions;
            ChannelData = channelData;
            Action = action;
            ReplyToId = replyToId;
            Value = value;
            Name = name;
            RelatesTo = relatesTo;
            Code = code;
            MembersAdded = membersAdded ?? new List<ChannelAccount>();
            MembersRemoved = membersRemoved ?? new List<ChannelAccount>();
            Attachments = attachments ?? new List<Attachment>();
            Entities = entities ?? new List<Entity>();
        }

        /// <summary>
        /// The type of the activity
        /// [message|contactRelationUpdate|converationUpdate|typing|endOfConversation|event|invoke]
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// ID of this activity
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// UTC Time when message was sent (set by service)
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Local time when message was sent (set by client, Ex:
        /// 2016-09-23T13:07:49.4714686-07:00)
        /// </summary>
        [JsonProperty(PropertyName = "localTimestamp")]
        public DateTimeOffset? LocalTimestamp { get; set; }

        /// <summary>
        /// Service endpoint where operations concerning the activity may be
        /// performed
        /// </summary>
        [JsonProperty(PropertyName = "serviceUrl")]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// ID of the channel where the activity was sent
        /// </summary>
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// Sender address
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public ChannelAccount From { get; set; }

        /// <summary>
        /// Conversation
        /// </summary>
        [JsonProperty(PropertyName = "conversation")]
        public ConversationAccount Conversation { get; set; }

        /// <summary>
        /// (Outbound to bot only) Bot's address that received the message
        /// </summary>
        [JsonProperty(PropertyName = "recipient")]
        public ChannelAccount Recipient { get; set; }

        /// <summary>
        /// Format of text fields [plain|markdown] Default:markdown
        /// </summary>
        [JsonProperty(PropertyName = "textFormat")]
        public string TextFormat { get; set; }

        /// <summary>
        /// Hint for how to deal with multiple attachments: [list|carousel]
        /// Default:list
        /// </summary>
        [JsonProperty(PropertyName = "attachmentLayout")]
        public string AttachmentLayout { get; set; }

        /// <summary>
        /// Array of address added
        /// </summary>
        [JsonProperty(PropertyName = "membersAdded")]
        public IList<ChannelAccount> MembersAdded { get; set; }

        /// <summary>
        /// Array of addresses removed
        /// </summary>
        [JsonProperty(PropertyName = "membersRemoved")]
        public IList<ChannelAccount> MembersRemoved { get; set; }

        /// <summary>
        /// Conversations new topic name
        /// </summary>
        [JsonProperty(PropertyName = "topicName")]
        public string TopicName { get; set; }

        /// <summary>
        /// True if the previous history of the channel is disclosed
        /// </summary>
        [JsonProperty(PropertyName = "historyDisclosed")]
        public bool? HistoryDisclosed { get; set; }

        /// <summary>
        /// The language code of the Text field
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Content for the message
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// SSML Speak for TTS audio response
        /// </summary>
        [JsonProperty(PropertyName = "speak")]
        public string Speak { get; set; }

        /// <summary>
        /// Indicates whether the bot is accepting, expecting, or ignoring
        /// input
        /// </summary>
        [JsonProperty(PropertyName = "inputHint")]
        public string InputHint { get; set; }

        /// <summary>
        /// Text to display if the channel cannot render cards
        /// </summary>
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// SuggestedActions are used to provide keyboard/quickreply like
        /// behavior in many clients
        /// </summary>
        [JsonProperty(PropertyName = "suggestedActions")]
        public SuggestedActions SuggestedActions { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>
        [JsonProperty(PropertyName = "attachments")]
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Collection of Entity objects, each of which contains metadata
        /// about this activity. Each Entity object is typed.
        /// </summary>
        [JsonProperty(PropertyName = "entities")]
        public IList<Entity> Entities { get; set; }

        /// <summary>
        /// Channel-specific payload
        /// </summary>
        [JsonProperty(PropertyName = "channelData")]
        public object ChannelData { get; set; }

        /// <summary>
        /// ContactAdded/Removed action
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        /// <summary>
        /// The original ID this message is a response to
        /// </summary>
        [JsonProperty(PropertyName = "replyToId")]
        public string ReplyToId { get; set; }

        /// <summary>
        /// Open-ended value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        /// <summary>
        /// Name of the operation to invoke or the name of the event
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Reference to another conversation or activity
        /// </summary>
        [JsonProperty(PropertyName = "relatesTo")]
        public ConversationReference RelatesTo { get; set; }

        /// <summary>
        /// Code indicating why the conversation has ended
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

    }
}
