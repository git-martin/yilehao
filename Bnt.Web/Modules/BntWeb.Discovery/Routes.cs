using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Discovery
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
                        HostConstConfig.AdminDirectory + "/Discovery",
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area},
                            {"controller", "Admin"},
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area}
                        },
                        new MvcRouteHandler())
                }
                ,
                new RouteDescriptor
                {
                    Priority = 0,
                    Route = new Route(
                        HostConstConfig.AdminDirectory + "/Discovery/Page",
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area},
                            {"controller", "Admin"},
                            {"action", "ListOnPage"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Priority = 0,
                    Route = new Route(
                        HostConstConfig.AdminDirectory + "/Discovery/{action}",
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area},
                            {"controller", "Admin"},
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", DiscoveryModule.Area}
                        },
                        new MvcRouteHandler())
                }
            };
        }

    }
}