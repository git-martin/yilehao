using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Evaluate
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
                                                         HostConstConfig.AdminDirectory + "/{sourceType}/{sourceId}/Evaluate",
                                                         new RouteValueDictionary {
                                                                                      { "area", EvaluateModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", EvaluateModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Evaluate/Delete",
                                                         new RouteValueDictionary {
                                                                                      { "area",EvaluateModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", EvaluateModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/{sourceType}/{sourceId}/Evaluate/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", EvaluateModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", EvaluateModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                              new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Evaluate/Deletes",
                                                         new RouteValueDictionary {
                                                                                      { "area", EvaluateModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "BatchDeleteEvaluates"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", EvaluateModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                               new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Evaluate/Approve",
                                                         new RouteValueDictionary {
                                                                                      { "area", EvaluateModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "BatchApproveEvaluates"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", EvaluateModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}