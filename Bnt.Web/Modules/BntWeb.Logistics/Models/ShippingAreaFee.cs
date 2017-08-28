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
    [Table(KeyGenerator.TablePrefix + "Shippings_Areas_Fees")]
    public class ShippingAreaFee
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 配送区域主表id
        /// </summary>
        public Guid ShippingAreaId { get; set; }

        /// <summary>
        /// 区域主键id
        /// </summary>
        [MaxLength(10)]
        public string AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [MaxLength(200)]
        public string AreaName { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public decimal Freight { get; set; }

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
}