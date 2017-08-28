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

namespace BntWeb.SinglePage
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = SinglePageModule.DisplayName;

        public const string ViewPageKey = "BntWeb-SinglePage-ViewPage";
        public static readonly Permission ViewPage = new Permission { Description = "查看单页", Name = ViewPageKey, Category = CategoryKey };

        public const string EditPageKey = "BntWeb-SinglePage-EditPage";
        public static readonly Permission EditPage = new Permission { Description = "编辑单页", Name = EditPageKey, Category = CategoryKey };

       
        public int Position => SinglePageModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewPage,
                EditPage
            };
        }
    }
}