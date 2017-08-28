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
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data;
using BntWeb.Wallet.Models;

namespace BntWeb.Wallet.Services
{
    public interface IWalletService : IDependency
    {
        /// <summary>
        /// 获取会员钱包
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <returns></returns>
        Models.Wallet GetWalletByMemberId(string memberId, WalletType walletType);

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
        List<WalletBill> GetWalletBillByMemberId(string memberId, int pageNo, int limit, out int totalCount, WalletType walletType = WalletType.Cash, BillType? billType = null, string billTag = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// 获取提现申请
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <param name="isDesc"></param>
        /// <param name="totalCount"></param>
        /// <param name="expression"></param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        List<CrashApply> GetCrashApplyByPage<TKey>(int pageNo, int limit, Expression<Func<CrashApply, bool>> expression, Expression<Func<CrashApply, TKey>> orderByExpression, bool isDesc, out int totalCount);

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="account">账号</param>
        /// <param name="money">金额</param>
        /// <param name="paymentType"></param>
        /// <param name="name">账号名称</param>
        /// <returns></returns>
        bool ApplyCrash(string memberId, string account, decimal money, PaymentType paymentType, string name);

        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool AuditApply(Guid id);

        /// <summary>
        /// 支用
        /// 请使用以下示例的事务来操作钱包
        /// using (TransactionScope scope = new TransactionScope())
        /// {
        ///         //其他操作
        ///          
        ///         Draw(memberId, walletType, money, remark, out error);
        ///  
        ///         //提交
        ///         scope.Complete();
        /// }
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <param name="money"></param>
        /// <param name="remark"></param>
        /// <param name="error"></param>
        /// <param name="tag"></param>
        /// <param name="toMemberId">转给谁</param>
        /// <param name="drawFrozen">支出冻结的金额</param>
        /// <returns></returns>
        bool Draw(string memberId, WalletType walletType, decimal money, string remark, out string error, string tag = null, string toMemberId = null, bool drawFrozen = false);

        /// <summary>
        /// 存款
        /// 请使用以下示例的事务来操作钱包
        /// using (TransactionScope scope = new TransactionScope())
        /// {
        ///         //其他操作
        ///          
        ///         Deposit(memberId, walletType, money, remark, out error);
        ///  
        ///         //提交
        ///         scope.Complete();
        /// }
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <param name="money"></param>
        /// <param name="remark"></param>
        /// <param name="error"></param>
        /// <param name="tag"></param>
        /// <param name="fromMemberId">谁转来</param>
        /// <returns></returns>
        bool Deposit(string memberId, WalletType walletType, decimal money, string remark, out string error, string tag = null, string fromMemberId = null);


        /// <summary>
        /// 冻结
        /// 请使用以下示例的事务来操作钱包
        /// using (TransactionScope scope = new TransactionScope())
        /// {
        ///         //其他操作
        ///          
        ///         Frozen(memberId, walletType, money, remark, out error);
        ///  
        ///         //提交
        ///         scope.Complete();
        /// }
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <param name="money"></param>
        /// <param name="remark"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool Frozen(string memberId, WalletType walletType, decimal money, string remark, out string error);

        /// <summary>
        /// 解冻
        /// 请使用以下示例的事务来操作钱包
        /// using (TransactionScope scope = new TransactionScope())
        /// {
        ///         //其他操作
        ///          
        ///         Thaw(memberId, walletType, money, remark, out error);
        ///  
        ///         //提交
        ///         scope.Complete();
        /// }
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="walletType"></param>
        /// <param name="money"></param>
        /// <param name="remark"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool Thaw(string memberId, WalletType walletType, decimal money, string remark, out string error);

        /// <summary>
        /// 计算总数
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <param name="walletType">钱包类型</param>
        /// <param name="billType">流水类型，null，所有</param>
        /// <param name="billTag"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="fromMemberId">谁转来</param>
        /// <returns></returns>
        decimal Sum(string memberId, WalletType walletType, BillType? billType, string billTag = null, DateTime? startTime = null, DateTime? endTime = null, string fromMemberId = null);

        /// <summary>
        /// 取消超时未处理的提现申请
        /// </summary>
        /// <param name="outTime"></param>
        /// <returns></returns>
        int CancelCrashApply(DateTime outTime);
        /// <summary>
        /// 获取提现记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CrashApply GetCrashApplyById(Guid id);
    }
}