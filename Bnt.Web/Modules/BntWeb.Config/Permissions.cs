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

namespace BntWeb.Config
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = ConfigModule.DisplayName;

        public const string WeiXinConfigKey = "BntWeb-Config-WeiXinConfig";
        public static readonly Permission WeiXinConfig = new Permission { Description = "微信配置", Name = WeiXinConfigKey, Category = CategoryKey };

        public const string AlipayConfigKey = "BntWeb-Config-AlipayConfig";
        public static readonly Permission AlipayConfig = new Permission { Description = "支付宝配置", Name = AlipayConfigKey, Category = CategoryKey };

        public const string SystemConfigKey = "BntWeb-Config-SystemConfig";
        public static readonly Permission SystemConfig = new Permission { Description = "系统配置", Name = SystemConfigKey, Category = CategoryKey };

        public int Position => ConfigModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                WeiXinConfig,
                AlipayConfig,
                SystemConfig
            };
        }
    }
}