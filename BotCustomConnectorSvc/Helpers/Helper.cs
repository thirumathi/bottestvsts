using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BotCustomConnectorSvc.Helpers
{
    public class Helper
    {
        public static bool PostToBotEnabled = bool.Parse(ConfigurationManager.AppSettings["PostToBotEnabled"]);
    }
}