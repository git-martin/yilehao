using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.OrderProcess.Models;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet;
using BntWeb.Wallet.Services;
using BntWeb.WebApi.Models;

namespace BntWeb.OrderProcess.Services
{
    public class RefundService : IRefundService
    {
        private readonly IUserContainer _userContainer;
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;

        public RefundService(IUserContainer userContainer, ICurrencyService currencyService, IWalletService walletService)
        {
            _userContainer = userContainer;
            _currencyService = currencyService;
            _walletService = walletService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 创建订单退款申请
        /// </summary>
        /// <param name="refund"></param>
        public void CreateOrderRefund(OrderRefund refund)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                //修改订单退款状态为
                var order = dbContext.Orders.FirstOrDefault(x => x.Id == refund.OrderId);
                if (order == null)
                    throw new WebApiInnerException("0002", "订单不存在");
                if (order.RefundStatus != OrderRefundStatus.Refunding)
                {
                    order.RefundStatus = OrderRefundStatus.Refunding;
                }

                var singleGoods = dbContext.OrderGoods.FirstOrDefault(x => x.OrderId == refund.OrderId && x.SingleGoodsId == refund.SingleGoodsId);
                if (singleGoods == null)
                    throw new WebApiInnerException("0006", "商品不存在");
                singleGoods.RefundStatus = OrderRefundStatus.Refunding;

                //新增退款申请
                refund.Id = KeyGenerator.GetGuidKey();
                refund.RefundNo = KeyGenerator.GetOrderNumber();
                refund.CreateTime = DateTime.Now;
                refund.RefundStatus = RefundStatus.Applying;
                dbContext.OrderRefunds.Add(refund);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 撤销退款申请 申请中=>已撤销
        /// </summary>
        /// <param name="refund"></param>
        public void RevokeOrderRefund(OrderRefund refund)
        {
            using (var dbContext = new OrderProcessDbContext())
            {
                //修改退款申请状态
                refund.RefundStatus = RefundStatus.Revoked;
                dbContext.Entry<OrderRefund>(refund).State = System.Data.Entity.EntityState.Modified;

                //修改订单商品退款状态
                var singleGoods = dbContext.OrderGoods.FirstOrDefault(x => x.OrderId == refund.OrderId && x.SingleGoodsId == refund.SingleGoodsId);
                if (singleGoods != null)
                    singleGoods.RefundStatus = OrderRefundStatus.NoRefund;

                //修改订单状态
                var order = dbContext.Orders.FirstOrDefault(x => x.Id == refund.OrderId);
                if (order != null)
                {
                    order.RefundStatus = GetOrderRefundStatus(refund, RefundStatus.Applying);
                }

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 退款申请审核
        /// 若为仅退款且审核通过则直接打款
        /// 审核通过 仅退款  直接打款  退款申请退款状态：已完成 ，订单商品退款状态：已退款 ，订单退款状态:根据条件判断
        /// 审核通过 退款并退货  退款申请退款状态：已处理
        /// 审核不通过 退款申请退款状态：已完成 ，订单商品退款状态：未退款 ，订单退款状态:根据条件判断
        /// </summary>
        /// <param name="refund"></param>
        public void AuditRefund(OrderRefund refund)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var logMsg = "";
                var memo = "退款审核";
                var currentUser = _userContainer.CurrentUser;
                refund.ReviewUserId = currentUser.Id;
                refund.ReviewTime = DateTime.Now;

                bool isPass = true;
                //订单
                var order = _currencyService.GetSingleById<Order>(refund.OrderId);
                //退款订单商品
                var goods = _currencyService.GetSingleByConditon<OrderGoods>(x => x.OrderId == refund.OrderId && x.SingleGoodsId == refund.SingleGoodsId);
                //审核通过
                if (refund.ReviewResult == ReviewResult.Passed)
                {
                    //退款类型仅退款 审核结果已通过 直接打款
                    if (refund.RefundType == RefundType.OnlyRefund)
                    {
                        memo = "退款审核通过并打款";
                        refund.PayUserId = _userContainer.CurrentUser.Id;
                        refund.PayTime = DateTime.Now;

                        string error;

                        #region 积分退款
                        if (order.Integral > 0)
                        {
                            var maxRefundAmount = goods.Price * goods.Quantity - order.IntegralMoney * (goods.Price * goods.Quantity / order.GoodsAmount);
                            var maxIntegral = order.Integral * (goods.Price * goods.Quantity / order.GoodsAmount);
                            var refundIntegral = maxIntegral * (refund.RefundAmount / maxRefundAmount);

                            //积分打款
                            _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Integral, refundIntegral,
                                $"订单{order.OrderNo}退款，积分退还", out error);
                            if (string.IsNullOrWhiteSpace(error))
                            {
                                Logger.Operation($"订单{order.OrderNo}退款，用户{order.MemberId}获得{refundIntegral}积分退款",
                                    WalletModule.Instance, SecurityLevel.Danger);
                            }
                            else
                            {
                                isPass = false;
                            }
                        }
                        #endregion

                        #region 现金退款
                        if (isPass)
                        {
                            //钱包打款
                            _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Cash, refund.RefundAmount,
                                $"订单{order.OrderNo}退款", out error);
                            if (string.IsNullOrWhiteSpace(error))
                            {
                                logMsg = $"，向用户{order.MemberId}打款{refund.RefundAmount}元";
                                Logger.Operation($"订单{order.OrderNo}退款，用户{order.MemberId}获得{refund.RefundAmount}元退款", WalletModule.Instance, SecurityLevel.Danger);

                                //修改退款申请状态为已完成
                                refund.RefundStatus = RefundStatus.Completed;
                                //修改订单商品退款状态为已退款
                                goods.RefundStatus = OrderRefundStatus.Refunded;
                                order.RefundStatus = GetOrderRefundStatus(refund, RefundStatus.Applying);
                                //订单退款金额累加
                                order.RefundFee = order.RefundFee + refund.RefundAmount;
                            }
                            else
                            {
                                isPass = false;
                            }
                        }
                        #endregion

                        #region 判断订单所有商品是否均已退款，关闭订单，退运费
                        if (isPass)
                        {
                            var goodsCount = _currencyService.Count<OrderGoods>(x => x.OrderId == refund.OrderId);
                            var refundCount =
                                _currencyService.Count<OrderRefund>(
                                    x => x.OrderId == refund.OrderId && x.ReviewResult == ReviewResult.Passed && x.RefundStatus == RefundStatus.Completed);
                            if (goodsCount - refundCount == 1)
                            {
                                memo = "退款审核通过并打款,且关闭订单";
                                //修改订单状态 已关闭
                                order.OrderStatus = OrderStatus.Closed;
                                Logger.Operation($"订单{order.OrderNo}商品已全部退款成功，关闭订单并退还运费{order.ShippingFee}", OrderProcessModule.Instance, SecurityLevel.Warning);
                                if (order.OrderStatus == OrderStatus.WaitingForDelivery && order.ShippingFee > 0)
                                {
                                    //退运费
                                    _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Cash, order.ShippingFee, $"订单{order.OrderNo}退运费", out error);
                                    if (string.IsNullOrWhiteSpace(error))
                                    {
                                        Logger.Operation($"订单{order.OrderNo}退运费，用户{order.MemberId}获得{order.ShippingFee}元退款", WalletModule.Instance, SecurityLevel.Danger);
                                    }
                                    else
                                    {
                                        isPass = false;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        refund.RefundStatus = RefundStatus.Processed;
                    }
                }
                //审核不通过
                else
                {
                    //修改退款申请状态为已完成
                    refund.RefundStatus = RefundStatus.Completed;
                    //修改订单商品退款状态为未退款
                    goods.RefundStatus = OrderRefundStatus.NoRefund;
                    order.RefundStatus = GetOrderRefundStatus(refund, RefundStatus.Applying);
                }
                if (isPass)
                {
                    var orderAction = new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = order.Id,
                        Memo = memo,
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        RefundStatus = order.RefundStatus,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName
                    };

                    Logger.Operation($"退款编号{refund.RefundNo}审核{refund.ReviewResult.Description()}{logMsg}", OrderProcessModule.Instance, SecurityLevel.Warning);
                    if (_currencyService.Update(refund) && _currencyService.Update(goods) && _currencyService.Update(order) && _currencyService.Create(orderAction))
                        //提交
                        scope.Complete();
                }
            }
        }

        /// <summary>
        /// 退款打款
        /// 只有退款类型为退款仅退货且审核通过的退款申请才有打款操作
        /// </summary>
        /// <param name="refund"></param>
        public void PayRefund(OrderRefund refund)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var memo = "退款打款";

                //订单
                var order = _currencyService.GetSingleById<Order>(refund.OrderId);
                //退款订单商品
                var goods = _currencyService.GetSingleByConditon<OrderGoods>(x => x.OrderId == refund.OrderId && x.SingleGoodsId == refund.SingleGoodsId);

                string error;
                bool isPass = true;

                #region 积分退款
                if (order.Integral > 0)
                {
                    var maxRefundAmount = goods.Price * goods.Quantity - order.IntegralMoney * (goods.Price * goods.Quantity / order.GoodsAmount);
                    var maxIntegral = order.Integral * (goods.Price * goods.Quantity / order.GoodsAmount);
                    var refundIntegral = maxIntegral * (refund.RefundAmount / maxRefundAmount);
                    //积分打款
                    _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Integral, refundIntegral,
                        $"订单{order.OrderNo}退款，积分退还", out error);
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        Logger.Operation($"订单{order.OrderNo}退款，用户{order.MemberId}获得{refundIntegral}积分退款",
                            WalletModule.Instance, SecurityLevel.Danger);
                    }
                    else
                    {
                        isPass = false;
                    }
                }
                #endregion

                #region 现金退款
                if (isPass)
                {
                    //钱包打款 
                    _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Cash, refund.RefundAmount,
                    $"订单{order.OrderNo}退款", out error);

                    if (string.IsNullOrWhiteSpace(error))
                    {
                        Logger.Operation($"订单{order.OrderNo}退款，用户{order.MemberId}获得{refund.RefundAmount}元退款",
                            WalletModule.Instance, SecurityLevel.Danger);
                        Logger.Operation($"退款编号{refund.RefundNo}打款，向用户{order.MemberId}打款{refund.RefundAmount}元",
                            OrderProcessModule.Instance, SecurityLevel.Danger);
                        //修改退款申请状态
                        refund.RefundStatus = RefundStatus.Completed;

                        //修改订单单品的退款状态
                        goods.RefundStatus = OrderRefundStatus.Refunded;

                        //修改订单退款状态
                        order.RefundStatus = GetOrderRefundStatus(refund, RefundStatus.Processed);
                        //订单退款金额累加
                        order.RefundFee = order.RefundFee + refund.RefundAmount;
                    }
                    else
                    {
                        isPass = false;
                    }
                }
                #endregion

                #region 判断订单所有商品是否均已退款，关闭订单，退运费
                if (isPass)
                {
                    var goodsCount = _currencyService.Count<OrderGoods>(x => x.OrderId == refund.OrderId);
                    var refundCount =
                                _currencyService.Count<OrderRefund>(
                                    x => x.OrderId == refund.OrderId && x.ReviewResult == ReviewResult.Passed && x.RefundStatus == RefundStatus.Completed);

                    if (goodsCount - refundCount == 1)
                    {
                        memo = "退款打款,且关闭订单";
                        //修改订单状态 已关闭
                        order.OrderStatus = OrderStatus.Closed;
                        Logger.Operation($"订单{order.OrderNo}商品已全部退款成功，关闭订单并退还运费{order.ShippingFee}", OrderProcessModule.Instance, SecurityLevel.Warning);

                        //待发货状态订单 退运费
                        if (order.OrderStatus == OrderStatus.WaitingForDelivery && order.ShippingFee > 0)
                        {
                            _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Cash, order.ShippingFee, $"订单{order.OrderNo}退运费", out error);
                            if (string.IsNullOrWhiteSpace(error))
                            {
                                Logger.Operation($"订单{order.OrderNo}退运费，用户{order.MemberId}获得{order.ShippingFee}元退款", WalletModule.Instance, SecurityLevel.Danger);
                            }
                            else
                            {
                                isPass = false;
                            }
                        }
                    }
                }
                #endregion

                if (isPass)
                {
                    var currentUser = _userContainer.CurrentUser;
                    refund.PayUserId = currentUser.Id;
                    refund.PayTime = DateTime.Now;

                    var orderAction = new OrderAction
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        OrderId = order.Id,
                        Memo = memo,
                        CreateTime = DateTime.Now,
                        OrderStatus = order.OrderStatus,
                        PayStatus = order.PayStatus,
                        ShippingStatus = order.ShippingStatus,
                        EvaluateStatus = order.EvaluateStatus,
                        RefundStatus = order.RefundStatus,
                        UserId = currentUser.Id,
                        UserName = currentUser.UserName
                    };

                    if (_currencyService.Update(refund) && _currencyService.Update(goods) && _currencyService.Update(order) && _currencyService.Create(orderAction))
                        //提交
                        scope.Complete();
                }
            }
        }

        /// <summary>
        /// 修改退款申请状态 获取订单退款状态
        /// 退款申请状态：申请中=>已撤销 （申请中1，已处理0，已完成0，申请中的申请为自身 ===>订单状态更改为无退款）
        /// 退款申请状态：申请中=>已撤销 （申请中1，已处理0，已完成>0，申请中的申请为自身 ===>订单状态更改为已退款）
        /// 退款申请状态：申请中=>已处理 （对订单退款状态无影响 不需处理 放回原订单退款状态）
        /// 退款申请状态：申请中=>已完成 （申请中1，已处理0，已完成0，申请中的申请为自身 ===>订单状态更改为无退款）
        /// 退款申请状态：申请中=>已完成 （申请中1，已处理0，已完成>0，申请中的申请为自身 ===>订单状态更改为已退款）
        /// 退款申请状态：已处理=>已完成 （申请中0，已处理1，处理中的申请为自身 ===>订单状态更改为已退款）
        /// </summary>
        /// <param name="refund"></param>
        /// <param name="oldRefundStatus"></param>
        /// <returns></returns>
        private OrderRefundStatus GetOrderRefundStatus(OrderRefund refund, RefundStatus oldRefundStatus)
        {
            var order = _currencyService.GetSingleById<Order>(refund.OrderId);
            var applyingList = _currencyService.GetList<OrderRefund>(x => x.OrderId == refund.OrderId && x.RefundStatus == RefundStatus.Applying);
            var processedList = _currencyService.GetList<OrderRefund>(x => x.OrderId == refund.OrderId && x.RefundStatus == RefundStatus.Processed);
            var completedList = _currencyService.GetList<OrderRefund>(x => x.OrderId == refund.OrderId && x.RefundStatus == RefundStatus.Completed);

            //原退款状态 申请中
            if (oldRefundStatus == RefundStatus.Applying)
            {
                //申请数为1 已审核为0 且申请为自身
                if (applyingList.Count == 1 && processedList.Count == 0 && applyingList[0].Id == refund.Id)
                {
                    //状态更改为 已完成(审核不通过) 已撤销
                    if (refund.RefundStatus == RefundStatus.Revoked || (refund.RefundStatus == RefundStatus.Completed && refund.ReviewResult == ReviewResult.UnPassed))
                    {
                        if (completedList.Count == 0)
                        {
                            return OrderRefundStatus.NoRefund;
                        }
                        else
                        {
                            return OrderRefundStatus.Refunded;
                        }
                    }
                    //状态更改为 已完成（审核通过）
                    else if (refund.RefundStatus == RefundStatus.Completed && refund.ReviewResult == ReviewResult.Passed)
                    {
                        return OrderRefundStatus.Refunded;
                    }
                }
            }
            else if (oldRefundStatus == RefundStatus.Processed)
            {
                if (applyingList.Count == 0 && processedList.Count == 1 && processedList[0].Id == refund.Id)
                {
                    if (refund.RefundStatus == RefundStatus.Completed)
                    {
                        return OrderRefundStatus.Refunded;
                    }
                }
            }
            return order.RefundStatus;
        }
    }
}