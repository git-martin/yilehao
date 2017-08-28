/* 
    ======================================================================== 
        File name：        Permissions
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/9 15:34:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System.Collections.Generic;
using BntWeb.Security.Permissions;


namespace BntWeb.SystemMessage
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = SystemMessageModule.DisplayName;

        public const string ViewSystemMessageKey = "BntWeb-SystemMessage-ViewSystemMessage";
        public static readonly Permission ViewSystemMessage = new Permission { Description = "查看系统消息", Name = ViewSystemMessageKey, Category = CategoryKey };

        public const string EditSystemMessageKey = "BntWeb-SystemMessage-EditSystemMessage";
        public static readonly Permission EditSystemMessage = new Permission { Description = "编辑系统消息", Name = EditSystemMessageKey, Category = CategoryKey };

        public const string DeleteSystemMessageKey = "BntWeb-SystemMessage-DeleteSystemMessage";
        public static readonly Permission DeleteSystemMessage = new Permission { Description = "删除系统消息", Name = DeleteSystemMessageKey, Category = CategoryKey };


        public int Position => SystemMessageModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewSystemMessage,
                EditSystemMessage,
                DeleteSystemMessage
            };
        }
    }
}