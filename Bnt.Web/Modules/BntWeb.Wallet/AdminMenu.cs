using System.Collections.Generic;
using BntWeb.UI.Navigation;

namespace BntWeb.Wallet
{
    public class AdminMenu: INavigationProvider
    {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(WalletModule.Key, WalletModule.DisplayName, WalletModule.Position.ToString(), BuildMenu, new List<string> { "icon-beer" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(WalletModule.Key + "-CrashApplyList", "提现申请", "10",
                item => item
                    .Action("CrashApplyList", "Admin", new { area = WalletModule.Area})
                    .Permission(Permissions.ViewWallet)
                );
            
        }
    }
}