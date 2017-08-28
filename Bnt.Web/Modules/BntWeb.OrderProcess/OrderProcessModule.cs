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

namespace BntWeb.OrderProcess
{
    public class OrderProcessModule : IBntWebModule
    {
        public static readonly OrderProcessModule Instance = new OrderProcessModule();

        public string InnerKey => "BntWeb-OrderProcess";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "订单管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "OrderProcess";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8200;
        public static int Position => Instance.InnerPosition;
    }
}