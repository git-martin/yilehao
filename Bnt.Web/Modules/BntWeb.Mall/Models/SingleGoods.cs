/* 
    ======================================================================== 
        File name：        SingleGoods
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/1 11:38:10
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
using BntWeb.FileSystems.Media;
using Newtonsoft.Json;

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Single")]
    public class SingleGoods
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        [MaxLength(50)]
        public string SingleGoodsNo { get; set; }

        /// <summary>
        /// 所属商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(10)]
        public string Unit { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 排序，从大到小
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 单品重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 团购价
        /// </summary>
        public decimal GrouponPrice { get; set; }

        [ForeignKey("GoodsId")]
        [JsonIgnore]
        public virtual Goods Goods { get; set; }


        [NotMapped]
        public virtual List<SingleGoodsAttribute> Attributes { get; set; }

        [NotMapped]
        public virtual SimplifiedStorageFile Image { get; set; }
    }
}