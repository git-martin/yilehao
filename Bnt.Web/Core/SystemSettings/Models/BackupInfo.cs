using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;

namespace BntWeb.Core.SystemSettings.Models
{
    [Table(KeyGenerator.TablePrefix + "Backup")]
    public class BackupInfo
    {
        [Key]
        public Guid Id { get; set; }

        public string CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public string FilePath { get; set; }

        public BackupStatus Status { get; set; }

        public string Message { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public enum BackupStatus
    {
        /// <summary>
        /// 备份中
        /// </summary>
        [Description("备份中")]
        Doing = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Succeeded = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Failed = 2
    }
}