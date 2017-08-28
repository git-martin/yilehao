using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BntWeb.Data.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.ApiModels;
using BntWeb.Wallet.Models;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.ApiControllers
{
    public class WalletBillController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;

        public WalletBillController(ICurrencyService currencyService, IWalletService walletService)
        {
            _currencyService = currencyService;
            _walletService = walletService;
        }

        /// <summary>
        /// 获取我的钱包账单
        /// </summary>
        /// <param name="walletType"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult Get(WalletType walletType = WalletType.Cash, int pageNo = 1, int limit = 10)
        {
            var result = new ApiResult();
            int totalCount;
            var listWalletBill = _walletService.GetWalletBillByMemberId(AuthorizedUser.Id, pageNo, limit, out totalCount, walletType);
            var data = new
            {
                TotalCount = totalCount,
                Bills = listWalletBill
            };
            result.SetData(data);
            return result;
        }
    }
}