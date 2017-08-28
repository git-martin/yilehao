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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Core.SystemUsers
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
                                                     Priority = 100,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Users/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Users/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Login",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "User"},
                                                                                      { "action", "Login"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/LogOff",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "User"},
                                                                                      { "action", "LogOff"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 100,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Roles/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "Role"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Roles/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemUsersModule.Area},
                                                                                      { "controller", "Role"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemUsersModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                         };
        }

    }
}