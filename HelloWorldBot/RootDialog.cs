using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorldBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {        
        public async Task StartAsync(IDialogContext context)
        {
            using (TraceHelper.Trace("RootDialog.StartAsync", LogOptions.Entry))
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            using (TraceHelper.Trace("RootDialog.MessageReceivedAsync", context, LogOptions.All))
            {
                Activity message = await result as Activity;
                string response = $"You sent **{message.Text.ToUpper()}** which was {message.Text.Length} characters";

                await context.PostAsync(response);
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}
