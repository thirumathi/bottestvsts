using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BotCustomConnectorSvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Home",
            //    url: "v3/conversations",
            //    defaults: new { controller = "Home", action = "Get", id = UrlParameter.Optional }
            //);

            routes.MapMvcAttributeRoutes();
        }
    }
}
