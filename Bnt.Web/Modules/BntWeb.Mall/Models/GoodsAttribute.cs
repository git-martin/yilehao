/* 
    ======================================================================== 
        File name：        GoodsAttribute
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/29 13:15:29
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

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Attributes")]
    public class GoodsAttribute
    {
        public Guid Id { get; set; }

        public Guid GoodsTypeId { get; set; }

        [ForeignKey("GoodsTypeId")]
        public virtual GoodsType GoodsType { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public AttributeType AttributeType { get; set; }

        /// <summary>
        /// 输入方式
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// 属性值，选择输入时存储
        /// </summary>
        public string Values { get; set; }

        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

    }

    /// <summary>
    /// 属性类型
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// 唯一属性
        /// </summary>
        [Description("唯一属性")]
        Unique = 0,

        /// <summary>
        /// 单选属性
        /// </summary>
        [Description("单选属性")]
        Single = 1,

        /// <summary>
        /// 多选属性
        /// </summary>
        [Description("多选属性")]
        Multiple = 2
    }

    /// <summary>
    /// 属性输入类型
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// 手工输入
        /// </summary>
        [Description("手工输入")]
        Manual = 0,

        /// <summary>
        /// 选择输入
        /// </summary>
        [Description("选择输入")]
        Select = 1
    }
}