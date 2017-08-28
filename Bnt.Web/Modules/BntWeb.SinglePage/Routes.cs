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

namespace BntWeb.SinglePage
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
            return new[]
            {
                 new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            "Page/{key}",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "SinglePage"},
                                {"action", "Page"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                 new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/Content/{key}",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "ProEdit"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages/Content",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "ProEdit"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages/Page",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "ListOnPage"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages/Edit",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "EditSinglePage"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages/EditOnPost",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "EditOnPost"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "List"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    },

                    new RouteDescriptor
                    {
                        Priority = 0,
                        Route = new Route(
                            HostConstConfig.AdminDirectory + "/SinglePages/{action}",
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area},
                                {"controller", "Admin"},
                                {"action", "List"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary
                            {
                                {"area", SinglePageModule.Area}
                            },
                            new MvcRouteHandler())
                    }

            };
        }

    }
}