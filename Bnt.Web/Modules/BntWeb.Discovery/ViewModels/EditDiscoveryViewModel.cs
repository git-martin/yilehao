using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BntWeb.Discovery.ViewModels
{
    public class EditDiscoveryViewModel
    {

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [Display(Name = "标题")]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Required]
        [Display(Name = "作者")]
        public string Author { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Required]
        [Display(Name = "简介")]
        public string Blurb { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Required]
        [Display(Name = "来源")]
        public string Source { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 读取数
        /// </summary>
        public int ReadNum { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 文章状态：1－正常，0－其他
        /// </summary>
        public int Status { get; set; }

        public string DiscoveryImages { set; get; }

        public List<Guid> Goods { get; set; }

    }


}