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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BntWeb.Environment.Configuration;
using BntWeb.Mvc.Routes;

namespace BntWeb.Core.SystemSettings
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
                                                         HostConstConfig.AdminDirectory + "/Settings/Sms/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "Sms"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Sms/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "Sms"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/DbBackup/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "DbBackup"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Logs/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "Log"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Logs/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "Log"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/SoftReleases/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "Log"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/SoftReleases/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "SoftRelease"},
                                                                                      { "action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Files/Page",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "File"},
                                                                                      { "action", "ListOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "BntWeb/Files/UploadedFileKindEdit",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "File"},
                                                                                      { "action", "UploadedFileKindEdit"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "BntWeb/Files/UploadedBase64Image",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "File"},
                                                                                      { "action", "UploadedBase64Image"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         "BntWeb/Files/SelectOnPage",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "File"},
                                                                                      { "action", "SelectOnPage"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Files/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "File"},
                                                                                      { "action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 0,
                                                     Route = new Route(
                                                         HostConstConfig.AdminDirectory + "/Settings/Districts/{action}",
                                                         new RouteValueDictionary {
                                                                                      { "area", SystemSettingsModule.Area},
                                                                                      { "controller", "District"},
                                                                                      { "action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", SystemSettingsModule.Area}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }

    }
}