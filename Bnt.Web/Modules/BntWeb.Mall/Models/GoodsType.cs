/* 
    ======================================================================== 
        File name：        GoodsType
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/29 13:11:44
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

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Types")]
    public class GoodsType
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}