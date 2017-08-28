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

namespace BntWeb.MemberCenter
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = MemberCenterModule.DisplayName;

        public const string ViewMemberKey = "BntWeb-MemberCenter-ViewMember";
        public static readonly Permission ViewMember = new Permission { Description = "查看会员", Name = ViewMemberKey, Category = CategoryKey };

        public const string EditMemberKey = "BntWeb-MemberCenter-EditMember";
        public static readonly Permission EditMember = new Permission { Description = "编辑会员", Name = EditMemberKey, Category = CategoryKey };

        public const string DeleteMemberKey = "BntWeb-MemberCenter-DeleteMember";
        public static readonly Permission DeleteMember = new Permission { Description = "删除会员", Name = DeleteMemberKey, Category = CategoryKey };

        public const string SwitchMemberKey = "BntWeb-MemberCenter-SwitchMemberKey";
        public static readonly Permission SwitchMember = new Permission { Description = "启用禁用", Name = SwitchMemberKey, Category = CategoryKey };


        public int Position => MemberCenterModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewMember,
                EditMember,
                DeleteMember,
                SwitchMember
            };
        }
    }
}