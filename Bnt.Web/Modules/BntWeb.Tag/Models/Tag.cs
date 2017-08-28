/* 
    ======================================================================== 
        File name：        Tag
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/21 14:54:29
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Tag.Models
{
    [Table(KeyGenerator.TablePrefix + "Tags")]
    public class Tag
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 标签内容
        /// </summary>
        [MaxLength(20)]
        public string Content { get; set; }

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
        /// 类型，可以为空
        /// </summary>
        [MaxLength(20)]
        public string SourceType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}