using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Logistics
{
    public class LogisticsModule : IBntWebModule
    {
        public static readonly LogisticsModule Instance = new LogisticsModule();

        public string InnerKey => "BntWeb-Logistics";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "物流管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Logistics";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8100;
        public static int Position => Instance.InnerPosition;
    }
}