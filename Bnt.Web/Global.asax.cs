using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using BntWeb.Environment;
using BntWeb.WarmupStarter;

namespace Bnt.Web
{
    public class MvcApplication : HttpApplication
    {
        private static Starter<IBntWebHost> _starter;

        public MvcApplication()
        {
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            //跨域访问
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //GlobalConfiguration.Configuration.EnableCors(cors);

            //配置返回的时间类型数据格式  
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
                new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                }
            );

            RegisterRoutes(RouteTable.Routes);
            _starter = new Starter<IBntWebHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            _starter.OnApplicationStart(this);
        }

        protected void Application_BeginRequest()
        {
            _starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest()
        {
            _starter.OnEndRequest(this);
        }

        private static void HostBeginRequest(HttpApplication application, IBntWebHost host)
        {
            application.Context.Items["originalHttpContext"] = application.Context;
            host.BeginRequest();
        }

        private static void HostEndRequest(HttpApplication application, IBntWebHost host)
        {
            host.EndRequest();
        }

        private static IBntWebHost HostInitialization(HttpApplication application)
        {
            var host = BntWebStarter.CreateHost(MvcSingletons);

            host.Initialize();

            host.BeginRequest();
            host.EndRequest();

            return host;
        }

        static void MvcSingletons(ContainerBuilder builder)
        {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }
    }
}
