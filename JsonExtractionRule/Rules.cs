using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using JsonExtractionRule;

namespace HelperLib
{
    [DisplayName("JSON Extraction Rule")]
    [Description("Extracts the specified JSON value from an object.")]
    public class JsonExtractionRule : ExtractionRule
    {
        public string Name { get; set; }

        public override void Extract(object sender, ExtractionEventArgs e)
        {
            try
            {
                if (e.Response.BodyString != null)
                {
                    e.WebTest.Context.Add(Constants.Context_BotResponseReceived, "false");

                    var json = e.Response.BodyString;
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JObject.Parse(json);

                        if (data != null)
                        {
                            e.WebTest.Context.Add(this.ContextParameterName, data.SelectToken(Name));
                            e.Success = true;
                            return;
                        }
                    }
                }

                e.Success = false;
                e.Message = String.Format(CultureInfo.CurrentCulture, "Not Found: {0}", Name);
            }
            catch (Exception ex)
            {
                e.Success = false;
                e.Message = ex.Message;
            }
        }
    }

    [DisplayName("Json Message Id Validation Rule")]
    [Description("Extracts the specified JSON value from an object.")]
    public class JsonMessageIdValidationRule : ValidationRule
    {
        public string Name { get; set; }

        public string ContextVariableToValidate { get; set; }

        public string ExpectedResult { get; set; }

        public override void Validate(object sender, ValidationEventArgs e)
        {
            try
            {
                if (e.Response.BodyString != null)
                {
                    var json = e.Response.BodyString;

                    var data = JObject.Parse(json);
                    Dictionary<string, object> dataDictionary = data.ToObject<Dictionary<string, object>>();

                    Dictionary<int, dynamic> activityDetails = new Dictionary<int, dynamic>();
                    if (dataDictionary != null && dataDictionary.ContainsKey("activities"))
                    {
                        List<object> activities = ((JArray)dataDictionary["activities"]).ToList<object>();

                        if (activities.Count > 0)
                        {
                            foreach (var item in activities)
                            {
                                JObject activityInfo = item as JObject;
                                if (activityInfo != null)
                                {
                                    string messageId = activityInfo[Name].ToString();
                                    var messageIdStr = messageId.Split(new char[] { '|' });
                                    int messageIdInt = int.Parse(messageIdStr[1]);
                                    if (!string.IsNullOrEmpty(activityInfo["text"].ToString()))
                                    {
                                        activityDetails.Add(messageIdInt,
                                            new
                                            {
                                                id = activityInfo[Name].ToString(),
                                                fromId = activityInfo["from"]["id"],
                                                fromName = activityInfo["from"]["name"],
                                                text = activityInfo["text"],
                                                timestamp = activityInfo["timestamp"]
                                            });
                                    }
                                }
                            }

                            string userMessageFullId = e.WebTest.Context[ContextVariableToValidate].ToString();
                            var userMessageIdStr = userMessageFullId.Split(new char[] { '|' });
                            int userMessageIdInt = int.Parse(userMessageIdStr[1]);
                            int maxId = activityDetails.Max(k => k.Key);
                            int botMessageIdInt = maxId > userMessageIdInt ? maxId : (userMessageIdInt + 1);
                            //string botMessageIdStr = string.Concat(Enumerable.Repeat("0", (7 - botMessageIdInt.ToString().Length))) + botMessageIdInt.ToString();
                            //string botResponseMessageId = $"{userMessageIdStr[0]}|{botMessageIdStr}";

                            string convId = e.WebTest.Context[Constants.Context_ConvId].ToString();

                            if (activityDetails.ContainsKey(botMessageIdInt))
                            {
                                e.WebTest.Context[Constants.Context_BotResponseReceived] = "true";
                                DateTime requestTime = DateTime.Parse(activityDetails[userMessageIdInt].timestamp.ToString());
                                DateTime responseTime = DateTime.Parse(activityDetails[botMessageIdInt].timestamp.ToString());
                                double timeTaken = responseTime.Subtract(requestTime).TotalMilliseconds;

                                string actualResult = activityDetails[botMessageIdInt].text.ToString();
                                e.IsValid = ExpectedResult.Equals(actualResult, StringComparison.OrdinalIgnoreCase);
                                string status = e.IsValid ? "succeeded" : "failed";
                                e.Message = $"Request [{userMessageIdInt}] and response [{botMessageIdInt}] validation " + status;
                                e.Message += string.Format(", Actual:{0} {1} equals Expected:{2}", actualResult, e.IsValid ? string.Empty : "Not", ExpectedResult);

                                ReportHelper.WriteLog(convId, activityDetails[userMessageIdInt].text.ToString(), ExpectedResult, actualResult, status, timeTaken);

                                if (e.IsValid)
                                {
                                    string userFromId = activityDetails[userMessageIdInt].fromId;
                                    string botFromId = activityDetails[botMessageIdInt].fromId;
                                    e.IsValid = !userFromId.Equals(botFromId, StringComparison.OrdinalIgnoreCase);
                                    e.Message += string.Format(", Bot Id validation {0}", e.IsValid ? "succeeded" : "failed");
                                }

                                return;
                            }
                            else
                            {
                                int retryCount = 0;
                                if (e.WebTest.Context.ContainsKey(Constants.Context_RetryCount))
                                {
                                    retryCount = int.Parse(e.WebTest.Context[Constants.Context_RetryCount].ToString());
                                }

                                e.WebTest.Context[Constants.Context_RetryCount] = ++retryCount;

                                if (retryCount == 10)
                                {
                                   

                                    ReportHelper.WriteLog(convId, activityDetails[userMessageIdInt].text.ToString(), ExpectedResult, "No response from bot,Failed", "No response from bot,Failed", 0);
                                    e.IsValid = false;
                                    e.Message = String.Format(CultureInfo.CurrentCulture, "Not Found: {0}", Name);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                e.IsValid = false;
                e.Message = ex.Message;
            }
        }
    }

    public class LoggingStorageCheckRule : ConditionalRule
    {
        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }

        public override void CheckCondition(object sender, ConditionalEventArgs e)
        {
            e.IsMet = ReportHelper.Init(StorageAccountName, StorageAccountKey);
            e.Message = "Azure Storage " + (e.IsMet ? "" : "not") + "found";
        }
    }

    public class ContextCleanupRule : ValidationRule
    {
        public override void Validate(object sender, ValidationEventArgs e)
        {
            e.WebTest.Context.Remove(Constants.Context_ConvId);
            e.WebTest.Context.Remove(Constants.Context_RetryCount);
            e.WebTest.Context.Remove(Constants.Context_MessageId);
        }
    }
}