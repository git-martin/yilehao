using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BntWeb.Data;

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Category_Relations")]
    public class GoodsCategoryRelation
    {
        [Key]
        public Guid Id { set; get; }
        public Guid GoodsId { get; set; }

        public Guid CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public virtual GoodsCategory GoodsCategory { set; get; }
    }
}
