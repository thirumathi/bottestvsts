using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorldBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {        
        List<Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>> table = new List<Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>>()
        {
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(1, " ", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(1, "Claim", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(1, "Claim Amount", SeparationStyle.None, TextColor.Default, "15", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(1, "Perquisite Tax (Taxable Income)", SeparationStyle.None, TextColor.Default, "25", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(1, "Non Taxable", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(2, "Car CC <= 1600", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(2, "Car Running Expenses", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(2, "5000", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(2, "1800", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(2, "3200", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(3, " ", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(3, "Chauffer's Salary", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(3, "5000", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(3, "900", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(3, "4100", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(4, "Car CC > 1600", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(4, "Car Running Expenses", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(4, "5000", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(4, "2400", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(4, "2600", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(5, " ", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Bolder),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(5, "Chauffer's Salary", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(5, "5000", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(5, "900", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal),
            new Tuple<int, string, SeparationStyle, TextColor, string, TextSize, TextWeight>(5, "4100", SeparationStyle.None, TextColor.Default, "20", TextSize.Small, TextWeight.Normal)
        };

        public async Task StartAsync(IDialogContext context)
        {
            using (TraceHelper.Trace("RootDialog.StartAsync", LogOptions.Entry))
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                using (TraceHelper.Trace("RootDialog.MessageReceivedAsync", context, LogOptions.All))
                {
                    Activity message = await result as Activity;
                    string response = $"You sent **{message.Text.ToUpper()}** which was {message.Text.Length} characters";

                    //var botCred = new MicrosoftAppCredentials("d174fca8-5ade-4189-afa4-1322429d2ff4", "AoVK8a51j7ioa44z2Dyks1v");
                    ////var stateClient = new StateClient(new Uri("https://dipabotconnector.azurewebsites.net"));
                    //var stateClient = new StateClient(new Uri("https://state.botframework.com"), botCred); 
                    //BotState botState = new BotState(stateClient);
                    //BotData botData = new BotData("*");
                    //botData.SetProperty<string>("AccessToken", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ind0NFVKb0lheHFadUpSTnE5Sk12LW5LU0FQOCIsImtpZCI6Ind0NFVKb0lheHFadUpSTnE5Sk12LW5LU0FQOCJ9.eyJjbGllbnRfaWQiOiI3MzY0LmRpZ2l0YWxwZXJzb25hbGFzc2lzdGFudC5ib3Qud2ViIiwiTVNBcHBJRC1EZXYiOiJkYzFmMmFhNC00M2I0LTQ4OTUtOWQzZC00M2U1ZGI3MjU4ZWUiLCJNU0FwcElELVRlc3QiOiJhOTY4MzhjMy1lMWIyLTQ5MTUtODEwNS1iN2ZhYWVjN2FmYjUiLCJNU0FwcElELVN0YWdlIjoiZDE3NGZjYTgtNWFkZS00MTg5LWFmYTQtMTMyMjQyOWQyZmY0Iiwic2NvcGUiOlsiZXh0ZW5kZWRfdXNlcl9wcm9maWxlIiwidXNlcl9wcm9maWxlIl0sInN1YiI6IkRTXFxzYXRoaXNoLm5lZWxhbWVnYW0iLCJhbXIiOiJiZWFyZXIiLCJhdXRoX3RpbWUiOjE0OTk0MTAxMTksImlkcCI6Imlkc3J2IiwidXBuIjoic2F0aGlzaC5uZWVsYW1lZ2FtQGFjY2VudHVyZS5jb20iLCJlbWFpbCI6InNhdGhpc2gubmVlbGFtZWdhbUBhY2NlbnR1cmUuY29tIiwic2FtYWNjb3VudF9uYW1lIjoic2F0aGlzaC5uZWVsYW1lZ2FtIiwicGVvcGxla2V5IjoiOTIwNjY2IiwicGVyc29ubmVsbmJyIjoiMTA4NjU0NTUiLCJhZGRyZXNzIjoiQ2hlbm5haSAyIC0gU2hyaXJhbSBHYXRld2F5LVR3ciBBIiwiY291bnRyeV9jZCI6IklOIiwiY291bnRyeV9kZXNjciI6IkluZGlhIiwiY29tcGFueV9jZCI6IjgxMDUiLCJjb21wYW55X2Rlc2NyIjoiQVNQTC1DaGVubmFpIFNUUEkiLCJidXNpbmVzc29yZyI6IkNJTzA1IiwiYnVzaW5lc3NvcmdfY2QiOiI1MjQ1MDQ0MSIsImdpdmVuX25hbWUiOiJTYXRoaXNoIiwic24iOiJOZWVsYW1lZ2FtIiwiYWxpZ25tZW50X2NkIjoiODAwMDg3NzYiLCJhbGlnbm1lbnRfZGVzY3IiOiJBdXRvbW90aXZlIiwiY2FyZWVydHJhY2tfY2QiOiIwNTIwIiwiY2FyZWVydHJhY2tfZGVzY3IiOiJDbGllbnQgRGVsaXZlcnkgXHUwMDI2IE9wZXJhdGlvbnMiLCJoYXNfdG9rZW4iOiIzIiwiY29tcGV0ZW5jeV9ncnBfY2QiOiI4MDAwODAwNSIsImNvc3RjZW50ZXJfY2QiOiIwMDgxMDUwMzE0IiwiY29zdGNlbnRlcl9kZXNjciI6IlNUIEFWQU5BREUgU1NFIDM0U043IiwiZ2VvZ3JhcGhpY191bml0IjoiSW5kaWEiLCJnZW9ncmFwaGljX3VuaXRfY2QiOiJJTkQiLCJqb2JmYW1pbHkiOiIxMDAwMDEwMCIsImpvYmZhbWlseV9kZXNjciI6IjEwIiwiYWRkcmVzc19jZCI6IklORC1DRS0xNCIsIm1ldHJvY2l0eSI6IkNFIiwibWV0cm9jaXR5X2Rlc2NyIjoiQ2hlbm5haSIsInNhcHVzZXJfaWQiOiIxMDg2NTQ1NSIsInNwZWNpYWx0eV9jZCI6IjgwMDA4MjM4Iiwic3BlY2lhbHR5X2Rlc2NyIjoiTWljcm9zb2Z0IFdlYiBEZXZlbG9wbWVudCIsImNuIjoic2F0aGlzaC5uZWVsYW1lZ2FtIiwiY29tcGV0ZW5jeV9ncnAiOiJNaWNyb3NvZnQgUGxhdGZvcm0iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL2F1dGhlbnRpY2F0aW9ubWV0aG9kIjoidXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmNtOmJlYXJlciIsImlzcyI6InVybjpmZWRlcmF0aW9uOmFjY2VudHVyZTpzdGFnZSIsImF1ZCI6InVybjpmZWRlcmF0aW9uOmFjY2VudHVyZTpzdGFnZS9yZXNvdXJjZXMiLCJleHAiOjE0OTk0MTM3MTksIm5iZiI6MTQ5OTQxMDExOX0.QlSbeIs3-Iu42x328Zs1-SmtRDEHEQzXiIH-GzEvkz-Wfmx53SrSNea-BhFz0D2xyrAGtqUl5FxKawAONLWvHRCvvzdPmP3ovdA8HHdHSb1SQZaYOvxh5UfZvqalwFTa6iv24yyIBNWCba9pohIQBqlSt30i-1hwo6fN7cRe5-VDMVPlHXRlpLmfMybp0jHA_q4GyM6tRq-cKeeIP4iMO-yBjvxte_jIojnWe-X2DKEk7RlUhfXhM0kCy8biM52SnWCHywH6a_RuSXb7qMdFXOxJrFlK-ei9F6jYCnxaMvA84XpBSj4fqrqterWIE65xU8cbm9UtWKomyDQeCuHG-w");
                    //botData.SetProperty<bool>("isAuthenticated", true);
                    //BotData botData1 = botState.SetUserData("directline", "10865455", botData);
                    //BotData botData2 = botState.GetUserData("directline", "10865455");
                    //if (botData2.GetProperty<bool>("isAuthenticated"))
                    //{
                    //    string token = botData2.GetProperty<string>("AccessToken");

                    //}

                    //await context.PostAsync(response);
                    //context.Wait(this.MessageReceivedAsync);

                    Activity reply = message.CreateReply();
                    CreateAdaptiveCard(reply);

                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    await connector.Conversations.SendToConversationAsync(reply);
                    context.Wait(this.MessageReceivedAsync);

                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                TraceHelper.Trace("RootDialog.MessageReceivedAsync", context, ex);
            }
        }

        private void CreateAdaptiveCard(Activity reply)
        {
            var card = new AdaptiveCard();
            card.Version = "0.5";
            card.Body.Add(new Container()
            {
                Separation = SeparationStyle.Strong,
                Style = ContainerStyle.Normal,
                Items = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = "Company Leased Car",
                        Color = TextColor.Default,
                        Size = TextSize.Large,
                        Separation = SeparationStyle.None,
                        Weight =TextWeight.Bolder                        
                    }
                }
            });

            int i = 0;
            ColumnSet columnSet = default(ColumnSet);
            foreach (var item in table)
            {
                if (i != item.Item1)
                {
                    i = item.Item1;
                    columnSet = new ColumnSet() { Separation = SeparationStyle.None  };
                    card.Body.Add(columnSet);
                }
                AddColumnToColumnSet(columnSet, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6, item.Item7);
            }

            reply.Attachments.Add(new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            });
        }

        private void AddColumnToColumnSet(ColumnSet columnSet, string text, SeparationStyle separationStyle, TextColor color, string columnSize, TextSize textSize, TextWeight textWeight = TextWeight.Normal)
        {
            Column column = new Column()
            {
                Separation = separationStyle,
                Size = columnSize,
                Style = ContainerStyle.Normal
            };

            AddTextBlockToContainer(column, text, color, textSize, textWeight);

            columnSet.Columns.Add(column);
        }

        private void AddTextBlockToContainer(Container container, string text, TextColor color, TextSize textSize, TextWeight textWeight = TextWeight.Normal)
        {
            container.Items.Add(
                new TextBlock()
                {
                    Text = text,
                    Color = color,
                    Weight = textWeight,
                    Size = textSize,
                    Wrap = true,
                    Separation = SeparationStyle.None
                });
        }
    }
}
