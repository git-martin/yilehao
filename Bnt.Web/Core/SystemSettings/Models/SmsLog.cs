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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Security;

namespace BntWeb.Core.SystemSettings.Models
{
    [Table(KeyGenerator.TablePrefix + "Sms_Logs")]
    public class SmsLog
    {
        [Key]
        public Guid Id { get; set; }

        public string Message { get; set; }

        public string ModuleKey { get; set; }

        public string ModuleName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public DateTime CreateTime { get; set; }
    }
}