/* 
    ======================================================================== 
        File name：        PermissionItem
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/19 17:23:13
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BntWeb.Core.SystemUsers.Models
{
    public class PermissionItem
    {

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public PermissionItem() { }

        public PermissionItem(XElement permission)
        {
            Name = permission.Attribute("Name")?.Value;
            Description = permission.Attribute("Description")?.Value;
        }
    }
}