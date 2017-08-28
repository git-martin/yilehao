/* 
    ======================================================================== 
        File name：        WalletModule
        Module:                
        Author：            夏保华20:13:31
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Wallet
{
    public class WalletModule : IBntWebModule
    {
        public static readonly WalletModule Instance = new WalletModule();

        public string InnerKey => "BntWeb-Wallet";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "钱包管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Wallet";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 9100;
        public static int Position => Instance.InnerPosition;
    }
}