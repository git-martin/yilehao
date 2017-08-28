/* 
    ======================================================================== 
        File name：        Routes
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/4/29 8:47:08
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Config
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Config/WeiXin/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", ConfigModule.Area},
                                                                                      { "controller", "WeiXinConfig"},
                                                                                      { "action", "Config"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", ConfigModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Config/Alipay/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", ConfigModule.Area},
                                                                                      { "controller", "AlipayConfig"},
                                                                                      { "action", "Config"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", ConfigModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Config/System/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", ConfigModule.Area},
                                                                                      { "controller", "SystemConfig"},
                                                                                      { "action", "Config"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", ConfigModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}