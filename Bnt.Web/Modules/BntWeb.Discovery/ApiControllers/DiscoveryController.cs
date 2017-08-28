using System;
using System.Linq;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.Discovery.ApiModels;
using BntWeb.Discovery.Models;
using BntWeb.Discovery.Services;

using BntWeb.WebApi.Models;

namespace BntWeb.Discovery.ApiControllers
{
    public class DiscoveryController : BaseApiController
    {
        private readonly IDiscoveryService _discoveryService;
        private readonly ICurrencyService _currencyService;


        public DiscoveryController(IDiscoveryService discoveryService, ICurrencyService currencyService)
        {
            _discoveryService = discoveryService;
            _currencyService = currencyService;
        }

        /// <summary>
        /// 获取发现列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetList(int pageNo = 1, int limit = 10)
        {
            int totalCount = 0;
            var list = _discoveryService.GetDiscoveryByPage(pageNo, limit, out totalCount).ToList().Select(x=> new DiscoveryListModel(x));

            var result = new ApiResult();
            var data = new
            {
                TotalCount = totalCount,
                List = list
            };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 获取发现详情 及关联商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiResult Detail(Guid id)
        {
            var result = new ApiResult();
            var discovery =_currencyService.GetSingleByConditon<Models.Discovery>(x => x.Id == id && x.Status == DiscoveryStatus.Normal);
            if (discovery!=null)
            {
                var goods = _discoveryService.GetDiscoveryRelationGoods(id).Select(x=>new DiscoveryRelationGoodsModel(x));
                var data = new
                {
                    Detail= new DiscoveryModel(discovery),
                    RelationGoods =goods
                };
                result.SetData(data);

                //添加阅读数
                discovery.ReadNum++;
                _currencyService.Update(discovery);
            }
            
            return result;
        }
    }
}