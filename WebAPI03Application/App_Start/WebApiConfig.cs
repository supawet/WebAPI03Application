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

            // New code
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Asset Allocate route

            config.Routes.MapHttpRoute(
                name: "WealthPCustomize",
                //routeTemplate: "api/WealthPCustomize/{id}",
                routeTemplate: "api/WealthPCustomize",
                defaults: new { controller = "WealthPCustomizeTest", id = RouteParameter.Optional }
                //defaults: new { controller = "WealthPCustomize2", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
            );
            config.Routes.MapHttpRoute(
                name: "WealthPCustomizeTest",
                //routeTemplate: "api/WealthPCustomize/{id}",
                routeTemplate: "api/WealthPCustomizeTest",
                defaults: new { controller = "WealthPCustomizeTest", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
            );
            /*
            config.Routes.MapHttpRoute(
                name: "WealthPCustomize",
                //routeTemplate: "api/WealthPCustomize/{id}",
                routeTemplate: "api/WealthPCustomize",
                defaults: new { controller = "WealthPCustomize", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
            );

            config.Routes.MapHttpRoute(
                name: "WealthPCustomize2",
                //routeTemplate: "api/WealthPCustomize/{id}",
                routeTemplate: "api/WealthPCustomize2",
                defaults: new { controller = "WealthPCustomize2", id = RouteParameter.Optional }//,
                //constraints: new { id="length(2)"}
            );
            */

        }
    }
}
