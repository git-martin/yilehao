/* 
    ======================================================================== 
        File name：        SystemLog
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/27 9:58:08
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Security;

namespace BntWeb.Core.SystemSettings.Models
{
    [Table(KeyGenerator.TablePrefix + "Soft_Releases")]
    public class SoftRelease
    {
        [Key]
        public Guid Id { get; set; }

        public string SoftName { get; set; }

        public string SoftKey { get; set; }

        public SoftType SoftType { get; set; }

        public string Version { get; set; }

        public bool ForceUpdating { get; set; }

        public string Description { get; set; }

        public string DownloadUrl { get; set; }

        public DateTime CreateTime { get; set; }

        [NotMapped]
        public Guid SoftFile { get; set; }
    }

    public enum SoftType
    {
        /// <summary>
        /// Windows
        /// </summary>
        [Description("Windows")]
        Windows = 0,

        /// <summary>
        /// 安卓
        /// </summary>
        [Description("安卓")]
        Android = 1,

        /// <summary>
        /// 苹果
        /// </summary>
        [Description("苹果")]
        IOS = 2
    }
}