/* 
    ======================================================================== 
        File name：        Carousel
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 17:29:25
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

namespace BntWeb.Carousel.Models
{

    [Table(KeyGenerator.TablePrefix + "Carousels")]
    public class CarouselItem
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 关联主信息Id
        /// </summary>
        public Guid? SourceId { get; set; }

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
        /// 关联数据类型，可空
        /// </summary>
        [MaxLength(50)]
        public string SourceType { get; set; }

        /// <summary>
        /// 内容标题
        /// </summary>
        [MaxLength(80)]
        public string SourceTitle { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [MaxLength(200)]
        public string Summary { get; set; }

        /// <summary>
        /// 内容查看地址
        /// </summary>
        [MaxLength(200)]
        public string ViewUrl { get; set; }

        /// <summary>
        /// 短地址
        /// </summary>
        [MaxLength(200)]
        public string ShotUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 所属分组Id
        /// </summary>
        public Guid GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual CarouselGroup CarouselGroup { get; set; }

        /// <summary>
        /// 轮播图
        /// </summary>
        [NotMapped]
        public StorageFile CoverImage { get; set; }
    }
}