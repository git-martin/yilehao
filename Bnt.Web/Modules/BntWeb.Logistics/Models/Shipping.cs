using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Logistics.Models
{
    [Table(KeyGenerator.TablePrefix + "Shippings")]
    public class Shipping
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 配送方式代码
        /// </summary>
        [MaxLength(30)]
        public string Code { get; set; }

        /// <summary>
        /// 配送方式名称
        /// </summary>
        [MaxLength(120)]
        public string Name { get; set; }

        /// <summary>
        /// 配送方式描述
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public ShippingStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

    }

    public enum ShippingStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Delete = -1,

        /// <summary>
        /// 已禁用
        /// </summary>
        [Description("已禁用")]
        Disabled = 0,

        /// <summary>
        /// 已启用
        /// </summary>
        [Description("已启用")]
        Enabled = 1
    }

}