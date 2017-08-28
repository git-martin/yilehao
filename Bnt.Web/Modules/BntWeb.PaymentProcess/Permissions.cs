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

namespace BntWeb.PaymentProcess
{
    public class Permissions : IPermissionProvider
    {
        private static readonly string CategoryKey = PaymentProcessModule.DisplayName;

        public const string ConfigPaymentKey = "BntWeb-PaymentProcess-ConfigPayment";
        public static readonly Permission ConfigPayment = new Permission { Description = "支付配置", Name = ConfigPaymentKey, Category = CategoryKey };

        public const string TransferKey = "BntWeb-PaymentProcess-Transfer";
        public static readonly Permission Transfer = new Permission { Description = "批量转账", Name = TransferKey, Category = CategoryKey };

        public int Position => PaymentProcessModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ConfigPayment,
                Transfer
            };
        }
    }
}