/* 
    ======================================================================== 
        File name：        Routes
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/21 16:01:54
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

namespace BntWeb.Tag
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
                                                         HostConstConfig.AdminDirectory + "/{sourceType}/Tags/{moduleKey}/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", TagModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", TagModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Tags/Delete",
                                                         new RouteValueDictionary {
                                                                                      { "area", TagModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", TagModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}