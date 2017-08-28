/* 
    ======================================================================== 
        File name：        ActivityModule
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/16 11:50:31
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.PaymentProcess
{
    public class PaymentProcessModule : IBntWebModule
    {
        public static readonly PaymentProcessModule Instance = new PaymentProcessModule();

        public string InnerKey => "BntWeb-PaymentProcess";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "支付管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "PaymentProcess";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8300;
        public static int Position => Instance.InnerPosition;
    }
}