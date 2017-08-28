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

namespace BntWeb.OrderProcess
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = OrderProcessModule.DisplayName;

        public const string ViewOrderKey = "BntWeb-OrderProcess-ViewOrder";
        public static readonly Permission ViewOrder = new Permission { Description = "查看订单", Name = ViewOrderKey, Category = CategoryKey };

        public const string ManageOrderKey = "BntWeb-OrderProcess-ManageOrder";
        public static readonly Permission ManageOrder = new Permission { Description = "管理订单", Name = ManageOrderKey, Category = CategoryKey };

        public const string ManageComplaintKey = "BntWeb-OrderProcess-ManageComplaint";
        public static readonly Permission ManageComplaint = new Permission { Description = "处理投诉", Name = ManageComplaintKey, Category = CategoryKey };

        public const string ManageRefundKey = "BntWeb-OrderProcess-ManageRefund";
        public static readonly Permission ManageRefund = new Permission { Description = "处理退款", Name = ManageRefundKey, Category = CategoryKey };

        public const string ManageEvaluateKey = "BntWeb-OrderProcess-ManageEvaluate";
        public static readonly Permission ManagerEvaluate = new Permission { Description = "评论回复", Name = ManageEvaluateKey, Category = CategoryKey };

        public int Position => OrderProcessModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewOrder,
                ManageOrder,
                ManageComplaint,
                ManageRefund,
                ManagerEvaluate
            };
        }
    }
}