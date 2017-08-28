/* 
    ======================================================================== 
        File name：        Advert
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/27 16:39:30
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

namespace BntWeb.Advertisement.Models
{

    [Table(KeyGenerator.TablePrefix + "Adverts")]
    public class Advert
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 广告名
        /// </summary>
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// Key值
        /// </summary>
        [MaxLength(20)]
        public string Key { get; set; }

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
        /// 广告图宽度
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// 广告图高度
        /// </summary>
        public int ImageHeight { get; set; }

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
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 是否内置的
        /// </summary>
        public bool BuiltIn { get; set; }

        /// <summary>
        /// 所属分组Id
        /// </summary>
        public Guid AreaId { get; set; }

        [ForeignKey("AreaId")]
        public virtual AdvertArea Area { get; set; }

        /// <summary>
        /// 广告图
        /// </summary>
        [NotMapped]
        public StorageFile AdvertImage { get; set; }
    }
}