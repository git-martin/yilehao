/* 
    ======================================================================== 
        File name：        AdvertArea
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/27 16:39:21
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

namespace BntWeb.Advertisement.Models
{
    [Table(KeyGenerator.TablePrefix + "Advert_Areas")]
    public class AdvertArea
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
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        public virtual List<Advert> Adverts { get; set; }
    }
}