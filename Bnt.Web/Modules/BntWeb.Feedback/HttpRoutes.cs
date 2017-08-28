using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.Feedback
{
    public class HttpRoutes: IHttpRouteProvider
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
                                                        RouteTemplate = "Api/v1/Feedback/{type}/",
                                                        Defaults = new
                                                        {
                                                            area = FeedbackModule.Area,
                                                            controller = "Feedback",
                                                            action="GetListByType"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Feedback",
                                                        Defaults = new
                                                        {
                                                            area = FeedbackModule.Area,
                                                            controller = "Feedback"
                                                        }
                                                    }
                         };
        }

    }
}