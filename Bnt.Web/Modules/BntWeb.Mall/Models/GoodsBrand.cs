/* 
    ======================================================================== 
        File name：        GoodsBrand
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/8/4 10:24:33
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

namespace BntWeb.Mall.Models
{
    [Table(KeyGenerator.TablePrefix + "Goods_Brands")]
    public class GoodsBrand
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Name { set; get; }

        [MaxLength(200)]
        public string SiteUrl { set; get; }

        [MaxLength(1000)]
        public string Description { set; get; }

        [NotMapped]
        public string Logo { get; set; }

        public int Sort { set; get; }

    }
}