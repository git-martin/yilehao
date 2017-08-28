using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BntWeb.SinglePage.ViewModels
{
    public class EditSinglePageViewModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        [MaxLength(1000)]
        public string SubTitle { get; set; }

        /// <summary>
        /// 标签内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Key值
        /// </summary>
        [MaxLength(50)]
        public string Key { get; set; }


        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

    }
}