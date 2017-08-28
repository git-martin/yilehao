/* 
    ======================================================================== 
        File name：        IMarkupService
        Module:                
        Author：            夏保华
        Create Time：    2016/7/6 20:14:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Wallet.Models;
using BntWeb.WebApi.Models;

namespace BntWeb.Wallet.Services
{
    public class WalletService : IWalletService
    {
        private readonly ICurrencyService _currencyService;

        public WalletService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 获取我的钱包
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <returns></returns>
        public Models.Wallet GetWalletByMemberId(string memberId, WalletType walletType = WalletType.Cash)
        {
            return
                _currencyService.GetSingleByConditon<Models.Wallet>(
                    w => w.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && w.WalletType == walletType);
        }

        /// <summary>
        /// 获取我的钱包账单
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <param name="totalCount"></param>
        /// <param name="walletType"></param>
        /// <param name="billType"></param>
        /// <param name="billTag"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<WalletBill> GetWalletBillByMemberId(string memberId, int pageNo, int limit, out int totalCount, WalletType walletType = WalletType.Cash, BillType? billType = null, string billTag = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            Expression<Func<WalletBill, bool>> expression = wb => wb.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && wb.WalletType == walletType;

            using (var dbContext = new WalletDbContext())
            {
                var query = dbContext.WalletBills.Where(expression);

                if (billType != null)
                {
                    var billTypeValue = billType.Value;
                    query = query.Where(wb => wb.BillType == billTypeValue);
                }

                if (!string.IsNullOrWhiteSpace(billTag))
                    query = query.Where(wb => wb.BillTag.Equals(billTag, StringComparison.OrdinalIgnoreCase));

                if (startTime != null)
                {
                    var startTimeValue = startTime.Value;
                    query = query.Where(wb => wb.CreateTime >= startTimeValue);
                }

                if (endTime != null)
                {
                    var endTimeValue = endTime.Value;
                    query = query.Where(wb => wb.CreateTime <= endTimeValue);
                }
                totalCount = query.Count();

                query = query.OrderByDescending(wb => wb.CreateTime);

                return query.AsQueryable().Skip((pageNo - 1) * limit).Take(limit).ToList();
            }
        }

        public List<CrashApply> GetCrashApplyByPage<TKey>(int pageNo, int limit, Expression<Func<CrashApply, bool>> expression, Expression<Func<CrashApply, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new WalletDbContext())
            {
                var query = dbContext.CrashApplys.Where(expression);
                totalCount = query.Count();
                query = isDesc ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

                var list = query.Skip((pageNo - 1) * limit).Take(limit).ToList();
                return list;
            }
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        public bool ApplyCrash(string memberId, string account, decimal money, PaymentType paymentType, string name)
        {

            using (var dbContext = new WalletDbContext())
            {
                CrashApply crashApply = new CrashApply();
                crashApply.Id = KeyGenerator.GetGuidKey();
                crashApply.MemberId = memberId;
                crashApply.Account = account;
                crashApply.TransactionNo = KeyGenerator.GetOrderNumber();
                crashApply.RealName = name;
                crashApply.Money = money;
                crashApply.PaymentType = paymentType;
                crashApply.ApplyState = ApplyState.Applying;
                crashApply.CreateTime = DateTime.Now;
                dbContext.CrashApplys.Add(crashApply);

                if (paymentType == PaymentType.WeiXin)
                {
                    //判断是否绑定了微信
                }

                Models.Wallet wallet = GetWalletByMemberId(memberId);
                if (wallet == null)
                {
                    throw new WebApiInnerException("0003", "钱包没有可以提现的余额");
                }
                else
                {
                    if (money > wallet.Available)
                    {
                        throw new WebApiInnerException("0004", "提现的金额不能大于钱包的余额");
                    }
                    wallet.Frozen += money;
                    wallet.Available -= money;
                }
                dbContext.Set<Models.Wallet>().Attach(wallet);
                dbContext.Entry(wallet).State = EntityState.Modified;

                return dbContext.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 提现申请
        /// </summary>
        /// <returns></returns>
        public bool AuditApply(Guid id)
        {
            using (var dbContext = new WalletDbContext())
            {
                CrashApply crashApply = dbContext.CrashApplys.FirstOrDefault(c => c.Id == id);
                if (crashApply == null)
                {
                    throw new Exception("找不到提现的记录");
                }
                crashApply.ApplyState = ApplyState.ApplyPassed;
                crashApply.AuditTime = DateTime.Now;
                dbContext.Set<CrashApply>().Attach(crashApply);
                dbContext.Entry(crashApply).State = EntityState.Modified;

                Models.Wallet wallet = GetWalletByMemberId(crashApply.MemberId);
                if (wallet == null)
                {
                    throw new Exception("钱包是空的");
                }
                else if (crashApply.Money > wallet.Frozen)
                {
                    throw new Exception("提现的金额不能大于冻结的金额");
                }
                
                return dbContext.SaveChanges() > 0;
            }
        }

        public bool Draw(string memberId, WalletType walletType, decimal money, string remark, out string error, string tag = null, string toMemberId = null, bool drawFrozen = false)
        {
            error = string.Empty;
            if (money == 0) return true;

            using (var dbContext = new WalletDbContext())
            {
                var wallet =
                    dbContext.Set<Models.Wallet>()
                        .FirstOrDefault(w => w.WalletType == walletType && w.MemberId == memberId);

                if (drawFrozen)
                {
                    if (wallet == null || wallet.Frozen < money)
                    {
                        error = "冻结余额不足";
                        return false;
                    }

                    wallet.Frozen -= money;
                }
                else
                {
                    if (wallet == null || wallet.Available < money)
                    {
                        error = "余额不足";
                        return false;
                    }

                    wallet.Available -= money;
                }
                dbContext.Set<Models.Wallet>().Attach(wallet);
                dbContext.Entry(wallet).State = EntityState.Modified;

                WalletBill walletBill = new WalletBill();
                walletBill.Id = KeyGenerator.GetGuidKey();
                walletBill.MemberId = memberId;
                walletBill.BillType = BillType.TakeOut;
                walletBill.WalletType = walletType;
                walletBill.Money = money;
                walletBill.Remark = remark;
                walletBill.BillTag = tag;
                walletBill.CreateTime = DateTime.Now;
                walletBill.FromMemberId = toMemberId;
                dbContext.WalletBills.Add(walletBill);
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool Deposit(string memberId, WalletType walletType, decimal money, string remark, out string error, string tag = null, string fromMemberId = null)
        {
            error = string.Empty;
            if (money == 0) return true;
            using (var dbContext = new WalletDbContext())
            {
                var wallet =
                    dbContext.Set<Models.Wallet>()
                        .FirstOrDefault(w => w.WalletType == walletType && w.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                if (wallet == null)
                {
                    wallet = new Models.Wallet();
                    wallet.Id = KeyGenerator.GetGuidKey();
                    wallet.MemberId = memberId;
                    wallet.WalletType = walletType;
                    wallet.Frozen = 0;
                    wallet.Available = money;
                    dbContext.Wallets.Add(wallet);
                }
                else
                {
                    wallet.Available += money;
                    dbContext.Set<Models.Wallet>().Attach(wallet);
                    dbContext.Entry(wallet).State = EntityState.Modified;
                }

                WalletBill walletBill = new WalletBill();
                walletBill.Id = KeyGenerator.GetGuidKey();
                walletBill.MemberId = memberId;
                walletBill.BillType = BillType.TakeIn;
                walletBill.WalletType = walletType;
                walletBill.Money = money;
                walletBill.Remark = remark;
                walletBill.BillTag = tag;
                walletBill.CreateTime = DateTime.Now;
                walletBill.FromMemberId = fromMemberId;
                dbContext.WalletBills.Add(walletBill);
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool Frozen(string memberId, WalletType walletType, decimal money, string remark, out string error)
        {
            error = string.Empty;
            if (money == 0) return true;
            using (var dbContext = new WalletDbContext())
            {
                var wallet =
                    dbContext.Set<Models.Wallet>()
                        .FirstOrDefault(w => w.WalletType == walletType && w.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                if (wallet == null)
                {
                    error = "余额不足";
                    return false;
                }
                else
                {
                    if (wallet.Available < money)
                    {
                        error = "余额不足";
                        return false;
                    }
                    wallet.Available -= money;
                    wallet.Frozen += money;
                    dbContext.Set<Models.Wallet>().Attach(wallet);
                    dbContext.Entry(wallet).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
                return true;
            }
        }

        public bool Thaw(string memberId, WalletType walletType, decimal money, string remark, out string error)
        {
            error = string.Empty;
            if (money == 0) return true;
            using (var dbContext = new WalletDbContext())
            {
                var wallet =
                    dbContext.Set<Models.Wallet>()
                        .FirstOrDefault(w => w.WalletType == walletType && w.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase));
                if (wallet == null)
                {
                    error = "冻结金额不足";
                    return false;
                }
                else
                {
                    if (wallet.Frozen < money)
                    {
                        error = "冻结金额不足";
                        return false;
                    }
                    wallet.Available += money;
                    wallet.Frozen -= money;
                    dbContext.Set<Models.Wallet>().Attach(wallet);
                    dbContext.Entry(wallet).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
                return true;
            }
        }

        public decimal Sum(string memberId, WalletType walletType, BillType? billType, string billTag = null, DateTime? startTime = null, DateTime? endTime = null, string fromMemberId = null)
        {
            Expression<Func<WalletBill, bool>> expression = wb => wb.WalletType == walletType && wb.MemberId.Equals(memberId);

            using (var dbContext = new WalletDbContext())
            {
                var query = dbContext.WalletBills.Where(expression);

                if (billType != null)
                {
                    var billTypeValue = billType.Value;
                    query = query.Where(wb => wb.BillType == billTypeValue);
                }

                if (!string.IsNullOrWhiteSpace(billTag))
                    query = query.Where(wb => wb.BillTag.Equals(billTag, StringComparison.OrdinalIgnoreCase));

                if (startTime != null)
                {
                    var startTimeValue = startTime.Value;
                    query = query.Where(wb => wb.CreateTime >= startTimeValue);
                }

                if (endTime != null)
                {
                    var endTimeValue = endTime.Value;
                    query = query.Where(wb => wb.CreateTime <= endTimeValue);
                }

                if (!string.IsNullOrWhiteSpace(fromMemberId))
                    query = query.Where(wb => wb.FromMemberId.Equals(fromMemberId, StringComparison.OrdinalIgnoreCase));

                return query.Sum(wb => (decimal?)wb.Money) ?? 0;
            }
        }

        public int CancelCrashApply(DateTime outTime)
        {
            using (var dbContext = new WalletDbContext())
            {
                var query =
                    dbContext.CrashApplys.Where(ca => ca.ApplyState == ApplyState.Applying && ca.CreateTime < outTime).ToList();

                int success = 0;
                foreach (var apply in query)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        string error;
                        if (Thaw(apply.MemberId, WalletType.Cash, apply.Money, "申请超时未审核，自动取消", out error))
                        {
                            apply.ApplyState = ApplyState.Failure;
                            _currencyService.Update(apply);
                            Logger.Operation($"申请超时未审核，自动取消，单号：{apply.TransactionNo}", WalletModule.Instance,
                                Security.SecurityLevel.Warning);
                            success++;
                        }
                        //提交
                        scope.Complete();
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// 获取提现记录
        /// </summary>
        /// <returns></returns>
        public CrashApply GetCrashApplyById(Guid id)
        {
            return _currencyService.GetSingleById<CrashApply>(id);
        }
    }
}