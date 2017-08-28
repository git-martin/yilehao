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
    [Table(KeyGenerator.TablePrefix + "Goods_Commission")]
    public class GoodsCommission
    {
        [Key]
        public Guid Id { get; set; }

        public Guid GoodsId { get; set; }

        public int Level { set; get; }

        public decimal Value { get; set; }
    }
}