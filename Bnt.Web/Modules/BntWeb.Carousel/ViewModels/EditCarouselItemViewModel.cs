/* 
    ======================================================================== 
        File name：        EditCarouselItemViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/23 16:48:28
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Carousel.ViewModels
{
    public class EditCarouselItemViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// 关联模块Key
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// 关联模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 关联数据类型，可空
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 内容标题
        /// </summary>
        public string SourceTitle { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 内容查看地址
        /// </summary>
        public string ViewUrl { get; set; }

        /// <summary>
        /// 短地址
        /// </summary>
        public string ShotUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public Guid GroupId { get; set; }

        public Guid CoverImage { get; set; }
    }
}