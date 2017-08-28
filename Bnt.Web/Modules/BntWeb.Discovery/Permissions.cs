
using System.Collections.Generic;
using BntWeb.Security.Permissions;

namespace BntWeb.Discovery
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = DiscoveryModule.DisplayName;

        public const string ViewDiscoveryKey = "BntWeb-Discovery-ViewDiscovery";
        public static readonly Permission ViewDiscovery = new Permission { Description = "查看发现", Name = ViewDiscoveryKey, Category = CategoryKey };

        public const string DeleteDiscoveryKey = "BntWeb-Discovery-DeleteDiscovery";
        public static readonly Permission DeleteDiscovery = new Permission { Description = "删除发现", Name = DeleteDiscoveryKey, Category = CategoryKey };

        public const string EditDiscoveryKey = "BntWeb-Discovery-EditDiscovery";
        public static readonly Permission EditDiscovery = new Permission { Description = "编辑发现", Name = EditDiscoveryKey, Category = CategoryKey };

        public int Position => DiscoveryModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewDiscovery,
                EditDiscovery,
                DeleteDiscovery

            };
        }
    }
}