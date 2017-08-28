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

namespace BntWeb.Advertisement
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = AdvertisementModule.DisplayName;

        public const string ViewAdvertisementKey = "BntWeb-Advertisement-ViewMember";
        public static readonly Permission ViewAdvertisement = new Permission { Description = "查看广告", Name = ViewAdvertisementKey, Category = CategoryKey };

        public const string EditAdvertisementKey = "BntWeb-Advertisement-EditAdvertisement";
        public static readonly Permission EditAdvertisement = new Permission { Description = "编辑广告", Name = EditAdvertisementKey, Category = CategoryKey };


        public int Position => AdvertisementModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewAdvertisement,
                EditAdvertisement
            };
        }
    }
}