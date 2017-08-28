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
    [Table(KeyGenerator.TablePrefix + "wallet_bills")]
    public class WalletBill
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 会员Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 钱包类型
        /// </summary>
        public WalletType WalletType { get; set; }

        /// <summary>
        /// 账单类型
        /// </summary>
        public BillType BillType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string BillAccount { get; set; }

        public string FromMemberId { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 流水标签，用于自定义区别流水数据类别
        /// </summary>
        public string BillTag { get; set; }
    }

    public enum BillType
    {
        /// <summary>
        /// 收入
        /// </summary>
        [Description("收入")]
        TakeIn = 1,
        /// <summary>
        /// 支出
        /// </summary>
        [Description("支出")]
        TakeOut = 2
    }
}