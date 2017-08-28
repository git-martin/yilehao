using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Feedback
{
    public class Routes: IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {

                             //new RouteDescriptor {
                             //                        Priority = 0,
                             //                        Route = new Route(
                             //                            HostConstConfig.AdminDirectory + "/Feedbacks/{action}",
                             //                            new RouteValueDictionary {
                             //                                                         { "area", FeedbackModule.Area},
                             //                                                         { "controller", "Admin"},
                             //                                                         { "action", "List"},
                             //                                                         { "sourceType", "Feedbacks"}
                             //                                                     },
                             //                            new RouteValueDictionary(),
                             //                            new RouteValueDictionary {
                             //                                                         {"area", FeedbackModule.Area}
                             //                                                     },
                             //                            new MvcRouteHandler())
                             //                    },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/{sourceType}/Feedbacks/{feedbackType}/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", FeedbackModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", FeedbackModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Feedbacks/",
                                                         new RouteValueDictionary {
                                                                                      { "area", FeedbackModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", FeedbackModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Feedbacks/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", FeedbackModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", FeedbackModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Feedbacks/Delete",
                                                         new RouteValueDictionary {
                                                                                      { "area", FeedbackModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", FeedbackModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Feedbacks/Processe",
                                                         new RouteValueDictionary {
                                                                                      { "area", FeedbackModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Processe"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", FeedbackModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             //new RouteDescriptor {
                             //                        Priority = 0,
                             //                        Route = new Route(
                             //                            HostConstConfig.AdminDirectory + "/Feedbacks/{action}",
                             //                            new RouteValueDictionary {
                             //                                                         { "area", FeedbackModule.Area},
                             //                                                         { "controller", "Admin"},
                             //                                                         { "action", "List"}
                             //                                                     },
                             //                            new RouteValueDictionary(),
                             //                            new RouteValueDictionary {
                             //                                                         {"area", FeedbackModule.Area}
                             //                                                     },
                             //                            new MvcRouteHandler())
                             //                    }
                              
                         };
        }
    }
}