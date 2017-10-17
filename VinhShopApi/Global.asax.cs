using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using VinhShopApi.Mappings;

namespace VinhShopApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoMapperConfiguration.Configure();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration
                .Formatters
                .JsonFormatter
                .SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration
            .Formatters
            .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
            var referrer = Request.UrlReferrer;
            if (Context.Request.Path.Contains("signalr/") && referrer != null)
            {
                Context.Response.AppendHeader("Access-Control-Allow-Origin", referrer.Scheme + "://" + referrer.Authority);
            }
        }
    }
}