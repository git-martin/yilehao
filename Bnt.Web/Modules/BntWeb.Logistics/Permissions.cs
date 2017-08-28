using System.Collections.Generic;
using BntWeb.Security.Permissions;

namespace BntWeb.Logistics
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = LogisticsModule.DisplayName;

        public const string ViewShippingKey = "BntWeb-Logistics-ViewShipping";
        public static readonly Permission ViewShipping = new Permission { Description = "查看配送方式", Name = ViewShippingKey, Category = CategoryKey };


        //public const string DeleteShippingKey = "BntWeb-Logistics-DeleteShipping";
        //public static readonly Permission DeleteShipping = new Permission { Description = "删除配送方式", Name = DeleteShippingKey, Category = CategoryKey };

        public const string SwitchShippingKey = "BntWeb-Logistics-SwitchShipping";
        public static readonly Permission SwitchShipping = new Permission { Description = "启用禁用", Name = SwitchShippingKey, Category = CategoryKey };

        public const string EditShippingKey = "BntWeb-Logistics-EditShipping";
        public static readonly Permission EditShipping = new Permission { Description = "编辑配送方式", Name = EditShippingKey, Category = CategoryKey };

        public const string ViewShippingAreaKey = "BntWeb-Logistics-ViewShippingArea";
        public static readonly Permission ViewShippingArea = new Permission { Description = "查看配送区域", Name = ViewShippingAreaKey, Category = CategoryKey };

        public const string DeleteShippingAreaKey = "BntWeb-Logistics-DeleteShippingArea";
        public static readonly Permission DeleteShippingArea = new Permission { Description = "删除配送区域", Name = DeleteShippingAreaKey, Category = CategoryKey };

        public const string EditShippingAreaKey = "BntWeb-Logistics-EditShippingArea";
        public static readonly Permission EditShippingArea = new Permission { Description = "编辑配送区域", Name = EditShippingAreaKey, Category = CategoryKey };



        public int Position => LogisticsModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewShipping,
                //DeleteShipping,
                EditShipping,
                ViewShippingArea,
                DeleteShippingArea,
                EditShippingArea
            };
        }
    }
}