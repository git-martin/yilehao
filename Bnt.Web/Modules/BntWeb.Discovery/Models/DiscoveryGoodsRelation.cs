
using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BntWeb.Data;
using BntWeb.Mall.Models;

namespace BntWeb.Discovery.Models
{
    /// <summary>
    /// 实体：DiscoveryGoodsRelation
    /// </summary>
    [Table(KeyGenerator.TablePrefix + "Discovery_Goods_Relations")]
    public partial class DiscoveryGoodsRelation
    {
        [Key]
        public Guid Id { set; get; }

        public Guid DiscoveryId { get; set; }

        public Guid GoodsId { get; set; }

        [ForeignKey("GoodsId")]
        public virtual Goods Goods { set; get; }

    }
}