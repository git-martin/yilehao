/* 
    ======================================================================== 
        File name：        IMarkupService
        Module:                
        Author：            夏保华
        Create Time：    2016/7/11 15:27
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Wallet.Models
{
    [Table(KeyGenerator.TablePrefix + "wallet_crash_applys")]
    public class CrashApply
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        public string TransactionNo { get; set; }

        /// <summary>
        /// 会员Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 提现类型
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public ApplyState ApplyState { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 打款时间
        /// </summary>
        public DateTime? TransferTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
    public enum PaymentType
    {
        /// <summary>
        /// 余额支付
        /// </summary>
        [Description("余额支付")]
        Balance = 0,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay = 1,
        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        WeiXin = 2
    }
    public enum ApplyState
    {
        /// <summary>
        /// 申请中
        /// </summary>
        [Description("申请中")]
        Applying = 0,

        /// <summary>
        /// 申请通过
        /// </summary>
        [Description("申请通过")]
        ApplyPassed = 1,

        /// <summary>
        /// 打款中
        /// </summary>
        [Description("打款中")]
        Transfering = 2,

        /// <summary>
        /// 已打款
        /// </summary>
        [Description("已打款")]
        Transferred = 3,

        /// <summary>
        /// 提现失败
        /// </summary>
        [Description("提现失败")]
        Failure = 4
    }
}

