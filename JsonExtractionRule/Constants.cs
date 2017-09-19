using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExtractionRule
{
    public class Constants
    {
        public const string Context_BotConnectorBaseUrl = "BotConnectorBaseUrl";
        public const string Context_RetryCount = "RetryCount";
        public const string Context_ConvId = "ConvID";
        public const string Context_ReuseConvId = "ReuseConvId";
        public const string Context_ReuseUserId = "ReuseUserId";
        public const string Context_BotResponseReceived = "BotResponseReceived";
        public const string Context_MessageId = "MessageId";
        public const string Context_InstanceId = "instanceId";
        public const string Context_TestStatus = "testStatus";
        public const string Context_TestMatch = "testMatch";
        public const string Context_TestStatusMessage = "testStatusMessage";

        public const string Context_Utterance = "Utterance";
        public const string Context_LuisQnA = "LuisQnA";
        public const string Context_BusinessArea = "BusinessArea";
        public const string Context_ExpectedResult = "ExpectedResult";
        public const string Context_ActualResult = "ActualResult";
        public const string Context_Channel = "Channel";
        public const string Context_Duration = "Duration";
        public const string Context_ActivityCount = "ActivityCount";

        public const string Context_DataRowIndex = "DataRowIndex";
        public const string Context_DataRowCount = "DataRowCount";
        public const string Context_UserActivity = "UserActivity";
        public const string Context_MessagePostedToBot = "MessagePostedToBot";
        public const string Context_MessagePostedToBotTimeStamp = "MessagePostedToBotTimeStamp";
        public const string Context_ConnectorNotified = "ConnectorNotified";
        public const string Context_Watermark = "Watermark";
        public const string Context_IsCustomConnector = "IsCustomConnector";
        public const string Context_UserId = "UserId";
        public const string Context_AccessToken = "AccessToken";

        public const string RequestMethod_Post = "POST";
        public const string RequestMethod_Get = "GET";

        public const string ContentType_ApplicationJson = "application/json";
    }
}
