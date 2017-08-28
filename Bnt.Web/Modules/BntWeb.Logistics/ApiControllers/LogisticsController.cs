using System.Web.Http;
using BntWeb.Logistics.Services;
using BntWeb.WebApi.Models;

namespace BntWeb.Logistics.ApiControllers
{
    public class LogisticsController : BaseApiController
    {
        private readonly IShippingAreaService _shippingAreaService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="shippingAreaService"></param>
        public LogisticsController(IShippingAreaService shippingAreaService)
        {
            _shippingAreaService = shippingAreaService;
        }

        /// <summary>
        /// 根据区域id获取配送费用 市
        /// </summary>
        /// <param name="provinceId">省</param>
        /// <param name="cityId">市</param>
        /// <returns></returns>
        // Api/v1/Logistics/Freight/
        [HttpGet]
        public ApiResult GetFreightByCity(string provinceId,string cityId)
        {
            var result = new ApiResult();
            var data = new
            {
                Freight = _shippingAreaService.GetAreaFreight(provinceId,cityId)
            };
            result.SetData(data);
            return result;
        }
    }
}
