/* 
    ======================================================================== 
        File name：        EditAdvertViewModel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/28 15:36:14
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BntWeb.Advertisement.ViewModels
{
    public class EditAdvertViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

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
        /// 内容查看地址
        /// </summary>
        public string ViewUrl { get; set; }

        /// <summary>
        /// 短地址
        /// </summary>
        public string ShotUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid AdvertImage { get; set; }
    }
}