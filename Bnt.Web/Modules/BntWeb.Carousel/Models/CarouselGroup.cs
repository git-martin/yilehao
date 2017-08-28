/* 
    ======================================================================== 
        File name：        CarouselGroup
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 17:30:31
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

namespace BntWeb.Carousel.Models
{
    [Table(KeyGenerator.TablePrefix + "Carousel_Groups")]
    public class CarouselGroup
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 轮播组名
        /// </summary>
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 查询关键Key
        /// </summary>
        [MaxLength(20)]
        public string Key { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 是否内置的
        /// </summary>
        public bool BuiltIn { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        public virtual List<CarouselItem> CarouselItems { get; set; }
    }
}