using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BntWeb.Mvc.Routes;
using BntWeb.WebApi.Routes;

namespace BntWeb.PaymentProcess
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
                                                        RouteTemplate = "Api/v1/Payment/BalancePay/{orderId}",
                                                        Defaults = new
                                                        {
                                                            area = PaymentProcessModule.Area,
                                                            controller = "Process",
                                                            action="BalancePay"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Payment/{paymentCode}/SignInfo/{orderId}",
                                                        Defaults = new
                                                        {
                                                            area = PaymentProcessModule.Area,
                                                            controller = "Process",
                                                            action="SignInfo"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Payment/{paymentCode}/ScanCodePay/{orderId}",
                                                        Defaults = new
                                                        {
                                                            area = PaymentProcessModule.Area,
                                                            controller = "Process",
                                                            action="ScanCodePay"
                                                        }
                                                    },
                             new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Withdrawals/PaymentType",
                                                        Defaults = new
                                                        {
                                                            area = PaymentProcessModule.Area,
                                                            controller = "Withdrawals",
                                                            action="GetWithdrawalsPaymentType"
                                                        }
                                                    },
                              new HttpRouteDescriptor {
                                                        Priority = 0,
                                                        RouteTemplate = "Api/v1/Payment/WeiXin/JsApiParam",
                                                        Defaults = new
                                                        {
                                                            area = PaymentProcessModule.Area,
                                                            controller = "Process",
                                                            action="GetWxPayJsApiParam"
                                                        }
                                                    }
                         };
        }

    }
}
