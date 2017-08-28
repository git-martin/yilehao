/* 
    ======================================================================== 
        File name：		Routes
        Module:			
        Author：            罗嗣宝
        Create Time：    2016/6/29 13:11:44
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;


namespace BntWeb.Mall
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
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsRecycle/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsRecycle"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                            new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsRecycle/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsRecycle"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                            new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsShortage/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsShortage"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                            new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsShortage/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsShortage"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsBrand/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsBrand"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsBrand/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsBrand"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsType/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsType"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsType/Attribute/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "Attribute"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsType/Attribute/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "Attribute"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsType/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsType"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/Goods/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "Goods"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/Goods/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "Goods"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Mall/GoodsCategory/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", MallModule.Area},
                                                                                      { "controller", "GoodsCategory"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", MallModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}