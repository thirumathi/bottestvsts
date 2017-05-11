using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorldBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var message = await result;
                string response = $"You sent **{message.Text.ToUpper()}** which was {message.Text.Length} characters";
                await context.PostAsync(response);
                context.Wait(this.MessageReceivedAsync);
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }
        }
    }
}
