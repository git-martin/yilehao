/* 
    ======================================================================== 
        File name：        Markup
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 11:56:13
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.ContentMarkup.Models
{
    [Table(KeyGenerator.TablePrefix + "Markups")]
    public class Markup
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// 关联模块Key
        /// </summary>
        [MaxLength(50)]
        public string ModuleKey { get; set; }

        /// <summary>
        /// 关联模块名称
        /// </summary>
        [MaxLength(50)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 关联数据类型，可空
        /// </summary>
        [MaxLength(50)]
        public string SourceType { get; set; }

        /// <summary>
        /// 标记人编号
        /// </summary>
        [MaxLength(36)]
        public string MemberId { get; set; }

        /// <summary>
        /// 标记类型
        /// </summary>
        public MarkupType MarkupType { get; set; }

        /// <summary>
        /// 标记时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MarkupType
    {
        /// <summary>
        /// 已读
        /// </summary>
        [Description("已读")]
        Read = 0,

        /// <summary>
        /// 点赞
        /// </summary>
        [Description("点赞")]
        Like = 1,

        /// <summary>
        /// 收藏
        /// </summary>
        [Description("收藏")]
        Collect = 2,
        /// <summary>
        /// 浏览
        /// </summary>
        [Description("浏览")]
        Browse = 3
    }
}