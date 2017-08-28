using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BntWeb.Data;

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Categories")]
    public class GoodsCategory
    {
        [Key]
        public Guid Id { set; get; }

        [MaxLength(100)]
        public string Name { set; get; }

        [MaxLength(300)]
        public string Descirption { set; get; }

        public Guid ParentId { set; get; }

        public int Sort { set; get; }

        public Guid? GoodsTypeId { get; set; }

        public bool ShowIndex { get; set; }

        /// <summary>
        /// 分类等级
        /// </summary>
        public int Level { set; get; }

        /// <summary>
        /// 合并id
        /// </summary>
        [MaxLength(500)]
        public string MergerId { get; set; }

        [ForeignKey("ParentId")]
        public virtual List<GoodsCategory> ChildCategories { get; set; }
    }
}
