﻿/* 
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

namespace BntWeb.Dashboard
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
                                                     Priority = -20,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory,
                                                         new RouteValueDictionary {
                                                                                      { "area", "Dashboard"},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Dashboard"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                                                         new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory+ "/Statistics/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", "Dashboard"},
                                                                                      { "controller", "Statistics"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Dashboard"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}