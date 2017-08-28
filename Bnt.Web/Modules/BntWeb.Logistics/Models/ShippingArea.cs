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
    [Table(KeyGenerator.TablePrefix + "Shippings_Areas")]
    public class ShippingArea
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 配送区域的名字
        /// </summary>
        [MaxLength(120)]
        public string Name { get; set; }

        /// <summary>
        /// 区域名字 多个,隔开
        /// </summary>
        [MaxLength(200)]
        public string AreaNames { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public ShippingAreaStatus Status { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 续费
        /// </summary>
        public decimal SFreight { get; set; }
        /// <summary>
        /// 首重
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public DefaultStatus IsDefualt { get; set; }
    }

    public enum DefaultStatus
    {
        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Normal = 0,
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Default = 1,
        /// <summary>
        /// 不配送
        /// </summary>
        [Description("不配送")]
        NotShipping = 2
    }

    public enum ShippingAreaStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Delete = -1,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1

    }
}