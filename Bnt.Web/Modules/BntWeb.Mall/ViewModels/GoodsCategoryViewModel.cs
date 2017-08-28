using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Mall.ViewModels
{
    public class GoodsCategoryViewModel
    {
        public Guid Id { set; get; }

        public string Name { set; get; }

        public string Descirption { set; get; }

        public Guid ParentId { set; get; }

        public int Sort { set; get; }

        public Guid? GoodsTypeId { get; set; }

        public bool ShowIndex { get; set; }

        public int Level { set; get; }

        /// <summary>
        /// 主图
        /// </summary>
        public string CategoryImage { set; get; }
    }
}