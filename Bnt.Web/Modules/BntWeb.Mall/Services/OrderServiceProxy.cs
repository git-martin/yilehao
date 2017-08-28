/* 
    ======================================================================== 
        File name：        OrderServiceProxy
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/11 13:03:02
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Mall.ApiModels;
using BntWeb.Mall.Models;
using BntWeb.MemberBase.Models;
using BntWeb.MemberBase.Services;
using BntWeb.OrderProcess.Models;
using BntWeb.OrderProcess.Services;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Wallet;
using BntWeb.Wallet.Services;

namespace BntWeb.Mall.Services
{
    public class OrderServiceProxy : IOrderServiceProxy
    {
        private readonly ICurrencyService _currencyService;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;
        private readonly IMemberService _memberService;

        public OrderServiceProxy(ICurrencyService currencyService, IWalletService walletService, IConfigService configService, IMemberService memberService)
        {
            _currencyService = currencyService;
            _walletService = walletService;
            _configService = configService;
            _memberService = memberService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string ModuleKey()
        {
            return MallModule.Key;
        }

        public void AfterSubmitOrder(Order order)
        {
            var systemConfig = _configService.Get<SystemConfig>();

            //下单减库存
            if (systemConfig.ReduceStock == BntWeb.Config.Models.ReduceStock.AfterOrder)
            {
                ReduceStock(order);
            }

            //积分完全付款，减库存
            if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterPay && order.PayStatus == PayStatus.Paid)
            {
                ReduceStock(order);
            }

            //订单付款减库存 已付款减库存
            if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterDeliver && order.PayStatus == PayStatus.Paid)
            {
                ReduceStock(order);
            }

            //增加商品的销量
            //如果订单已经付款，增加已付款计数
            using (var dbContext = new MallDbContext())
            {
                foreach (var orderGoods in order.OrderGoods)
                {
                    var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(orderGoods.GoodsId));
                    if (goods != null)
                    {
                        goods.SalesVolume += orderGoods.Quantity;
                        if (order.PayStatus == PayStatus.Paid)
                        {
                            goods.PaymentAmount += 1;
                        }
                        dbContext.Entry(goods).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                var changeCount = dbContext.SaveChanges();
                Logger.Operation($"生成新订单{order.OrderNo}，修改了{changeCount}个产品的销量统计", MallModule.Instance);
            }
        }

        public void AfterChangeOrderStatus(Order order, OrderStatus oldStatus)
        {
            var systemConfig = _configService.Get<SystemConfig>();

            #region 订单完成计算积分、佣金、减库存

            if (oldStatus != OrderStatus.Completed && order.OrderStatus == OrderStatus.Completed)
            {
                //订单完成减库存
                if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterCompleted)
                {
                    ReduceStock(order);
                }

                var member = _memberService.FindMemberById(order.MemberId);

                if (member != null)
                {
                    if (member.MemberType == MemberType.General)
                    {
                        //修改会员为合伙人
                        member.MemberType = MemberType.Partner;
                        _memberService.UpdateMember(member);
                    }
                    //计算用户积分
                    //计算佣金
                    //事务控制
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var integral = (int)(order.PayFee / systemConfig.ConsumptionIntegral);
                        string error;
                        _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Integral, integral,
                            $"订单{order.OrderNo}积分奖励", out error);
                        if (string.IsNullOrWhiteSpace(error))
                        {
                            Logger.Operation($"订单{order.OrderNo}完成，用户{order.MemberId}获得{integral}积分奖励",
                                WalletModule.Instance);
                        }

                        //计算佣金
                        //如果实际支付-快递费-退款<=0，则不计算佣金
                        var orderAmountForCommission = order.PayFee - order.ShippingFee - order.RefundFee;
                        if (orderAmountForCommission > 0)
                        {
                            for (int i = 1; i <= systemConfig.MaxLevel; i++)
                            {
                                if (string.IsNullOrWhiteSpace(member.ReferrerId))
                                    break;
                                var referrer = _memberService.FindMemberById(member.ReferrerId);
                                if (referrer == null)
                                    break;
                                //计算佣金
                                decimal reward = 0;
                                foreach (var orderGoods in order.OrderGoods)
                                {
                                    var commission =
                                        _currencyService.GetSingleByConditon<GoodsCommission>(
                                            c => c.GoodsId.Equals(orderGoods.GoodsId) && c.Level == i);
                                    if (commission == null || commission.Value == -1)
                                    {
                                        //取系统配置百分比
                                        reward += orderAmountForCommission *
                                                  (orderGoods.Price * orderGoods.Quantity / order.GoodsAmount) *
                                                  (decimal)(systemConfig.Rates[i - 1] / 100.0);
                                    }
                                    else
                                    {
                                        //取商品配置
                                        reward += commission.Value * orderGoods.Quantity;
                                    }

                                }

                                _walletService.Deposit(referrer.Id, Wallet.Models.WalletType.Cash, reward,
                                    $"订单{order.OrderNo}佣金奖励", out error, "commision", order.MemberId);
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    Logger.Operation($"订单{order.OrderNo}完成，用户{referrer.Id}获得{integral}元佣金奖励",
                                        WalletModule.Instance);
                                }

                                member = referrer;
                            }
                        }
                        scope.Complete();
                    }
                }
            }

            #endregion

            #region 订单取消，返回抵扣积分，返回使用的余额付款部分

            if (oldStatus == OrderStatus.PendingPayment && order.OrderStatus == OrderStatus.Closed)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string error;
                    _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Integral, order.Integral, $"订单{order.OrderNo}取消，返还积分", out error);
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        Logger.Operation($"订单{order.OrderNo}取消，用户{order.MemberId}返还{order.Integral}积分", WalletModule.Instance);
                    }

                    //待付款的时候不再扣除余额，所以不做退还操作，扣除余额动作在支付的时候执行
                    //_walletService.Deposit(order.MemberId.ToGuid(), Wallet.Models.WalletType.Cash, order.BalancePay, $"订单{order.OrderNo}取消，返还余额支付金额", out error);
                    //if (string.IsNullOrWhiteSpace(error))
                    //{
                    //    Logger.Operation($"订单{order.OrderNo}取消，用户{order.MemberId}返还{order.BalancePay}返还余额支付金额", WalletModule.Instance);
                    //}
                    scope.Complete();
                }

                //还原库存
                if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterOrder)
                {
                    RestoreStock(order);
                }
            }

            #endregion
        }

        public void AfterChangePayStatus(Order order, PayStatus oldStatus)
        {
            var systemConfig = _configService.Get<SystemConfig>();

            if (oldStatus != PayStatus.Paid && order.PayStatus == PayStatus.Paid)
            {
                //订单付款减库存
                if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterPay)
                {
                    ReduceStock(order);
                }

                //增加已付款计数
                using (var dbContext = new MallDbContext())
                {
                    foreach (var orderGoods in order.OrderGoods)
                    {
                        var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(orderGoods.GoodsId));
                        if (goods != null)
                        {
                            goods.PaymentAmount += 1;
                            dbContext.Entry(goods).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    var changeCount = dbContext.SaveChanges();
                    Logger.Operation($"订单{order.OrderNo}付款，修改了{changeCount}个产品的付款计数", MallModule.Instance);
                }
            }

            if (oldStatus == PayStatus.Paid && order.PayStatus == PayStatus.Unpaid)
            {
                //如果存在余额支付，退货余额，并设置订单余额支付为0，累计回未支付总额
                if (order.BalancePay > 0)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        string error;
                        _walletService.Deposit(order.MemberId, Wallet.Models.WalletType.Cash, order.BalancePay,
                            $"订单{order.OrderNo}退款", out error);

                        if (string.IsNullOrWhiteSpace(error))
                        {
                            Logger.Operation($"订单{order.OrderNo}退款，用户{order.MemberId}获得{order.BalancePay}元退款", WalletModule.Instance);
                            order.UnpayFee = order.PayFee;
                            order.BalancePay = 0;
                            if (_currencyService.Update(order))
                                //提交
                                scope.Complete();
                        }
                    }
                }
            }

        }

        public void AfterChangeShippingStatus(Order order, ShippingStatus oldStatus)
        {
            var systemConfig = _configService.Get<SystemConfig>();

            if (oldStatus == ShippingStatus.Unshipped && order.ShippingStatus == ShippingStatus.Shipped)
            {
                //订单付款减库存
                if (systemConfig.ReduceStock == Config.Models.ReduceStock.AfterDeliver)
                {
                    ReduceStock(order);
                }
            }
        }

        public void AfterChangeEvaluateStatus(Order order, EvaluateStatus oldEvaluateStatus)
        {
            using (var dbContext = new MallDbContext())
            {

                if (oldEvaluateStatus == EvaluateStatus.NotEvaluated && order.EvaluateStatus == EvaluateStatus.Evaluated)
                {
                    var query = _currencyService.GetList<OrderGoods>(x => x.OrderId == order.Id).GroupBy(x => x.GoodsId);
                    foreach (IGrouping<Guid, OrderGoods> group in query)
                    {
                        var num = 0;
                        foreach (OrderGoods orderGoods in group)
                        {
                            //判断商品是否已退款
                            var evaluates = _currencyService.Count<Evaluate.Models.Evaluate>(x => x.SourceId == orderGoods.Id);
                            if (evaluates > 0)
                            {
                                //商品评价数+1
                                num++;
                            }
                        }
                        if (num > 0)
                        {
                            var goods = dbContext.Goods.FirstOrDefault(x => x.Id == group.Key);

                            if (goods != null)
                            {
                                var entry = dbContext.Entry(goods);
                                entry.State = System.Data.Entity.EntityState.Unchanged;
                                entry.Property(o => o.EvaluateCount).IsModified = true;
                                goods.EvaluateCount = goods.EvaluateCount + num;
                            }
                        }
                    }
                    dbContext.SaveChanges();
                }
                else if (oldEvaluateStatus == EvaluateStatus.Evaluated && order.EvaluateStatus == EvaluateStatus.Replied)
                {

                }

            }
        }

        /// <summary>
        /// 减少库存
        /// </summary>
        /// <param name="order"></param>
        private void ReduceStock(Order order)
        {
            //减少库存
            using (var dbContext = new MallDbContext())
            {
                foreach (var orderGoods in order.OrderGoods)
                {
                    var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(orderGoods.GoodsId));
                    if (goods != null)
                    {
                        goods.Stock -= orderGoods.Quantity;
                        dbContext.Entry(goods).State = System.Data.Entity.EntityState.Modified;
                    }

                    var singleGoods = dbContext.SingleGoods.FirstOrDefault(g => g.Id.Equals(orderGoods.SingleGoodsId));
                    if (singleGoods != null)
                    {
                        singleGoods.Stock -= orderGoods.Quantity;
                        dbContext.Entry(singleGoods).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                var changeCount = dbContext.SaveChanges();

                Logger.Operation($"订单{order.OrderNo}发货，修改了{changeCount}个产品的库存", MallModule.Instance);
            }
        }


        /// <summary>
        /// 还原库存
        /// </summary>
        /// <param name="order"></param>
        private void RestoreStock(Order order)
        {
            //还原库存
            using (var dbContext = new MallDbContext())
            {
                foreach (var orderGoods in order.OrderGoods)
                {
                    var goods = dbContext.Goods.FirstOrDefault(g => g.Id.Equals(orderGoods.GoodsId));
                    if (goods != null)
                    {
                        goods.Stock += orderGoods.Quantity;
                        dbContext.Entry(goods).State = System.Data.Entity.EntityState.Modified;
                    }

                    var singleGoods = dbContext.SingleGoods.FirstOrDefault(g => g.Id.Equals(orderGoods.SingleGoodsId));
                    if (singleGoods != null)
                    {
                        singleGoods.Stock += orderGoods.Quantity;
                        dbContext.Entry(singleGoods).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                var changeCount = dbContext.SaveChanges();

                Logger.Operation($"订单{order.OrderNo}取消发货，修改了{changeCount}个产品的库存", MallModule.Instance);
            }
        }
    }
}
