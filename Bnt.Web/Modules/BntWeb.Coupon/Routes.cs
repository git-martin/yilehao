using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Coupon
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

                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/List",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/ListOnPage",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/Edit",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/EditOnPost",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "EditOnPost"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/Create",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/Delete",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
     new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/Send",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "SendCoupon"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Coupon/SendPost",
                                                         new RouteValueDictionary {
                                                                                      { "area", CouponModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "SendCouponOnPost"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", CouponModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                         };
        }
    }
}