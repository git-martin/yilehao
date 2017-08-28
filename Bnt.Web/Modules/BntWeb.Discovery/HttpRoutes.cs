using System;
using System.Collections.Generic;
using System.Linq;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Discovery
{
    public class HttpRoutes : IHttpRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Discovery/{id}",
                                                        Defaults = new
                                                        {
                                                            area = DiscoveryModule.Area,
                                                            controller = "Discovery"
                                                        }

                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Discovery",
                                                        Defaults = new
                                                        {
                                                            area = DiscoveryModule.Area,
                                                            controller = "Discovery"
                                                        }

                                                    }
                         };
        }
    }
}
