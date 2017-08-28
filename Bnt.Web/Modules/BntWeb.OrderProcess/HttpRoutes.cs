using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.OrderProcess
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
                                                        RouteTemplate = "Api/v1/Order/RefundLogistics",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "RefundLogistics"
                                                        }
                                                    },
                            new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/Refund/{applyId}",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Refund"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/Refund",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Refund"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}/Cancel",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Order",
                                                            action="Cancel"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}/Complete",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Order",
                                                            action="Complete"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}/Complaint",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Complaint"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Order",
                                                            orderId = RouteParameter.Optional
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}/Evaluate",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "Evaluate"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Order/{orderId}/Remind",
                                                        Defaults = new
                                                        {
                                                            area = OrderProcessModule.Area,
                                                            controller = "DeliveryReminder"
                                                        }
                                                    }
                         };
        }

    }
}
