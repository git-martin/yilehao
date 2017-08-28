/* 
    ======================================================================== 
        File name：        PermissionGroup
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/19 17:23:24
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Xml.Linq;
using BntWeb.UI.Navigation;

namespace BntWeb.Core.SystemUsers.Models
{
    public class PermissionGroup
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 所属的权限项
        /// </summary>
        public List<PermissionItem> Items = new List<PermissionItem>();

        public PermissionGroup() { }

        public PermissionGroup(XElement group)
        {
            Category = group.Attribute("Category")?.Value;

            var childPermissions = group.Elements("Permission");
            var permissions = childPermissions as XElement[] ?? childPermissions.ToArray();
            if (permissions.Any())
            {
                foreach (var childPermission in permissions)
                {
                    Items.Add(new PermissionItem(childPermission));
                }
            }
        }
    }
}