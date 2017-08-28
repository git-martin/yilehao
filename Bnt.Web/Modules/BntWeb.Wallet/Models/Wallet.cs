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
    [Table(KeyGenerator.TablePrefix + "wallets")]
    public class Wallet
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
        /// 可用余额
        /// </summary>
        public decimal Available { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public decimal Frozen { get; set; }
    }

    public enum WalletType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 1,
        /// <summary>
        /// 积分
        /// </summary>
        [Description("积分")]
        Integral = 2
    }
}