using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebAPI03Application
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Asset Allocate route
            config.Routes.MapHttpRoute(
                name: "WealthPCustomize",
                routeTemplate: "api/WealthPCustomize/{id}",
                defaults: new { controller = "WealthPCustomize", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
                               );

        }
    }
}
