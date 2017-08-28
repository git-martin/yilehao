using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.Logistics.Models;

namespace BntWeb.Logistics.ViewModels
{
    public class ShippingAreaViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 区域主键id
        /// </summary>
        public List<string> AreaId { get; set; }

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

    public class ShippingAreaListViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 区域名称 多个用,隔开
        /// </summary>
        public string AreaNames { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

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

        //public ShippingAreaListViewModel(Models.ShippingArea model)
        //{
        //    Id = model.Id;
        //    Name = model.Name;
        //    //var currencyService = HostConstObject.Container.Resolve<IDictionary>();
        //    //AreaName = currencyService.get;
        //    Freight = model.Freight;
        //    Sort = model.Sort;
        //    CreateTime = model.CreateTime;
        //}
    }
}