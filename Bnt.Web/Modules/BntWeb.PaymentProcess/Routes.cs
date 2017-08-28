/* 
    ======================================================================== 
        File name：		Routes
        Module:			
        Author：		罗嗣宝
        Create Time：		2016/7/7 9:13:28
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;


namespace BntWeb.PaymentProcess
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
                                                         "Payment/Process/WeiXin/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "WeiXin"},
                                                                                      { "action", "PayOAuth"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Payment/H5/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "H5"},
                                                                                      { "action", "Pay"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Payment/Web/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Alipay"},
                                                                                      { "action", "Pay"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Payment/Receive/{paymentCode}/AsyncReturn",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Receive"},
                                                                                      { "action", "AsyncReturn"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Payment/Receive/{paymentCode}/SyncReturn",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Receive"},
                                                                                      { "action", "SyncReturn"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Payment/Receive/{paymentCode}/AsyncReturn/Transfer",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Receive"},
                                                                                      { "action", "TransferAsyncReturn"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Payments/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "Pay/Receive/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", PaymentProcessModule.Area},
                                                                                      { "controller", "Receive"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", PaymentProcessModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}