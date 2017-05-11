using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Dialogs;
using Autofac;

namespace HelloWorldBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => new RootDialog().DefaultIfException());
                }
                else
                {
                    await HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                if (message.MembersAdded != null && message.MembersAdded.Any())
                {
                    //string membersAdded = string.Join(
                    //    ", ",
                    //    message.MembersAdded.Select(
                    //        newMember => (newMember.Id != message.Recipient.Id && !newMember.Name.Equals(botName, StringComparison.OrdinalIgnoreCase) && !newMember.Name.Equals("bot", StringComparison.OrdinalIgnoreCase)) ? $"{newMember.Name} (Id: {newMember.Id})" : string.Empty));

                    string membersAdded = string.Join(
                        ", ",
                        message.MembersAdded.Select(
                            newMember =>  $"{newMember.Name} (Id: {newMember.Id})"));


                    if (!string.IsNullOrEmpty(membersAdded))
                    {
                        Activity reply = message.CreateReply($"Welcome {membersAdded}");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }

                if (message.MembersRemoved != null && message.MembersRemoved.Any())
                {
                    string membersRemoved = string.Join(
                        ", ",
                        message.MembersRemoved.Select(
                            removedMember => (removedMember.Id != message.Recipient.Id) ? $"{removedMember.Name} (Id: {removedMember.Id})" : string.Empty));

                    Activity reply = message.CreateReply($"The following members {membersRemoved} were removed or left the conversation :(");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}