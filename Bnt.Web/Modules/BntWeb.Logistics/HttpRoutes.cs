using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Logistics
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
                                                        RouteTemplate = "Api/v1/Logistics/Freight/",
                                                        Defaults = new
                                                        {
                                                            area = LogisticsModule.Area,
                                                            controller = "Logistics",
                                                            action="GetFreightByCity"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Logistics",
                                                        Defaults = new
                                                        {
                                                            area = LogisticsModule.Area,
                                                            controller = "Logistics"
                                                        }
                                                    }
                         };
        }

    }
}
