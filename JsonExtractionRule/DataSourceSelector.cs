using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLib
{
    public class DataSourceSelector : WebTestPlugin
    {
        public override void PreRequest(object sender, PreRequestEventArgs e)
        {
            string env = e.WebTest.Context["Environment"].ToString().ToLower();
            string csvPath = $"D:\\Dev\\Git\\Dev\\BotLoadTestProject\\{env}Data.csv";

            e.WebTest.DataSources[0].SetConnection("Microsoft.VisualStudio.TestTools.DataSource.CSV", csvPath);
            e.WebTest.ReloadDataTable(e.WebTest.DataSources[0].Name, e.WebTest.DataSources[0].Tables[0].Name);
        }
    }
}
