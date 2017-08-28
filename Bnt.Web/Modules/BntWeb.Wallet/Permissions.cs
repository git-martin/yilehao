using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Security.Permissions;

namespace BntWeb.Wallet
{
    public class Permissions: IPermissionProvider
    {
        private static readonly string CategoryKey = WalletModule.DisplayName;

        public const string ViewWalletKey = "BntWeb-Wallet-ViewWallet";
        public static readonly Permission ViewWallet = new Permission { Description = "查看提现申请", Name = ViewWalletKey, Category = CategoryKey };

        public const string ProcesseWalletKey = "BntWeb-Wallet-ProcesseWallet";
        public static readonly Permission ProcesseWallet = new Permission { Description = "处理提现申请", Name = ProcesseWalletKey, Category = CategoryKey };


        public const string ManageMemberIntegralKey = "BntWeb-Wallet-ManageMemberIntegral";
        public static readonly Permission ManageMemberIntegral = new Permission { Description = "会员积分管理", Name = ManageMemberIntegralKey, Category = CategoryKey };

        public int Position => WalletModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewWallet,
                ProcesseWallet,
                ManageMemberIntegral
            };
        }
    }
}