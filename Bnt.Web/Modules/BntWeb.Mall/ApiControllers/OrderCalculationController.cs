using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using BntWeb.Config.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logistics.Services;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.Mall.Services;
using BntWeb.MemberBase.Models;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Mall.ApiControllers
{
    public class OrderCalculationController : BaseApiController
    {
        private readonly IGoodsService _goodsService;
        private readonly ICurrencyService _currencyService;
        private readonly IShippingAreaService _shippingAreaService;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        public OrderCalculationController(IGoodsService goodsService, ICurrencyService currencyService, IShippingAreaService shippingAreaService, IWalletService walletService, IConfigService configService)
        {
            _goodsService = goodsService;
            _currencyService = currencyService;
            _shippingAreaService = shippingAreaService;
            _walletService = walletService;
            _configService = configService;
        }

        /// <summary>
        /// 订单计算
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [BasicAuthentication]
        public ApiResult OrderCalculation([FromBody]OrderCalculationModel calculationModel)
        {
            //if ((calculationModel.CartIds == null || calculationModel.CartIds.Count < 1) && (calculationModel.SingleGoods == null || calculationModel.SingleGoods.Count < 1))
            //    throw new WebApiInnerException("0001", "没有选择商品");

            //计算商品总价
            var goodsAmount = 0M;
            var isFreeShipping = false;
            var goodsWeight = 0M;

            int goodsNum = 0;
            List<OrderCalculationGoodsModel> goods = new List<OrderCalculationGoodsModel>();

            if (calculationModel.IsFromCart)
            {
                if (calculationModel.CartIds == null || calculationModel.CartIds.Count < 1)
                    throw new WebApiInnerException("0001", "没有选择商品");

                foreach (var item in calculationModel.CartIds)
                {
                    var cartInfo = _currencyService.GetSingleById<Cart>(item);
                    if (cartInfo == null || cartInfo.MemberId != AuthorizedUser.Id)
                        throw new WebApiInnerException("0002", "存在非法商品");

                    var singleGoods = _goodsService.LoadFullSingleGoods(cartInfo.SingleGoodsId);
                    if (singleGoods?.Goods == null || singleGoods.Goods.Status != GoodsStatus.InSale)
                        throw new WebApiInnerException("0003", "存在非法或者失效的商品");
                    if (singleGoods.Stock < cartInfo.Quantity)
                        throw new WebApiInnerException("0004", $"【{singleGoods.Goods.Name}】库存不足");

                    //goodsAmount += (singleGoods.Goods.IsGroupon ? (singleGoods.Goods.GrouponEndTime <= DateTime.Now ? singleGoods.Price : singleGoods.GrouponPrice) : singleGoods.Price) * cartInfo.Quantity;
                    goodsAmount += (singleGoods.Goods.IsGroupon ? ((singleGoods.Goods.GrouponStartTime <= DateTime.Now && singleGoods.Goods.GrouponEndTime >= DateTime.Now) ? singleGoods.GrouponPrice : singleGoods.Price) : singleGoods.Price) * cartInfo.Quantity;
                   goodsWeight += (singleGoods.Weight == 0 ? singleGoods.Goods.UsualWeight : singleGoods.Weight);
                    if (singleGoods.Goods.FreeShipping)
                    {
                        isFreeShipping = true;
                    }
                    goodsNum += cartInfo.Quantity;
                    goods.Add(new OrderCalculationGoodsModel(cartInfo));
                }
            }
            else
            {
                if (calculationModel.SingleGoods == null || calculationModel.SingleGoods.Count < 1)
                    throw new WebApiInnerException("0001", "没有选择商品");

                foreach (var item in calculationModel.SingleGoods)
                {
                    var singleGoods = _goodsService.LoadFullSingleGoods(item.SingleGoodsId);
                    if (singleGoods?.Goods == null || singleGoods.Goods.Status != GoodsStatus.InSale)
                        throw new WebApiInnerException("0003", "存在非法或者失效的商品");
                    if (singleGoods.Stock < item.Quantity)
                        throw new WebApiInnerException("0004", $"【{singleGoods.Goods.Name}】库存不足");

                    //goodsAmount += (singleGoods.Goods.IsGroupon ? (singleGoods.Goods.GrouponEndTime <= DateTime.Now ? singleGoods.Price : singleGoods.GrouponPrice) : singleGoods.Price) * item.Quantity;
                    goodsAmount += (singleGoods.Goods.IsGroupon ? ((singleGoods.Goods.GrouponStartTime <= DateTime.Now && singleGoods.Goods.GrouponEndTime >= DateTime.Now) ? singleGoods.GrouponPrice : singleGoods.Price) : singleGoods.Price) * item.Quantity;
                    goodsWeight += (singleGoods.Weight == 0 ? singleGoods.Goods.UsualWeight : singleGoods.Weight);
                    if (singleGoods.Goods.FreeShipping)
                    {
                        isFreeShipping = true;
                    }
                    goodsNum += item.Quantity;
                    goods.Add(new OrderCalculationGoodsModel(singleGoods, item.Quantity));
                }
            }

            //获取收货地址
            MemberAddress address;
            if (calculationModel.AddressId == null)
            {
                address = _currencyService.GetList<MemberAddress>(me => me.MemberId == AuthorizedUser.Id)
                    .OrderByDescending(x => x.IsDefault)
                    .FirstOrDefault();
            }
            else
            {
                address = _currencyService.GetSingleById<MemberAddress>(calculationModel.AddressId);
            }

            //计算物流费用
            var shippingFee = address == null || isFreeShipping ? 0 : _shippingAreaService.GetAreaFreight(address.Province, address.City, goodsWeight);

            //可用积分
            var integralWallet = _walletService.GetWalletByMemberId(AuthorizedUser.Id, Wallet.Models.WalletType.Integral);

            var systemConfig = _configService.Get<SystemConfig>();
            var result = new ApiResult();
            var data = new
            {
                Goods = new
                {
                    TotalQuantity = goodsNum,
                    List = goods
                },
                Addresses = address == null ? null : new
                {
                    Id = address.Id,
                    Contacts = address.Contacts,
                    Phone = address.Phone,
                    RegionName = address.RegionName.Replace(",", ""),
                    Address = address.Address,
                    Postcode = address.Postcode,
                    Province = address.Province,
                    City = address.City,
                    District = address.District,
                    Street = address.Street,
                    IsDefault = address.IsDefault
                },
                GoodsAmount = goodsAmount,
                ShippingFee = shippingFee,
                AvailableIntegral = integralWallet?.Available ?? 0,
                IntegralDiscountRate = systemConfig.DiscountRate
            };
            result.SetData(data);
            return result;
        }



    }
}
