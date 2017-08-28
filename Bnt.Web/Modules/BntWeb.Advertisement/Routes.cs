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

namespace BntWeb.Advertisement
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
                                                         HostConstConfig.AdminDirectory + "/Adverts/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", AdvertisementModule.Area},
                                                                                      { "controller", "Admin"},
                                                                                      { "action", "AreaList"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", AdvertisementModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}