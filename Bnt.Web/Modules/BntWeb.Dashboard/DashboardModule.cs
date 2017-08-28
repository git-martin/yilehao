using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Dashboard
{
    public class DashboardModule : IBntWebModule
    {
        public static readonly DashboardModule Instance = new DashboardModule();

        public string InnerKey => "BntWeb-Dashboard";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "控制台管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Dashboard";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7800;
        public static int Position => Instance.InnerPosition;
    }
}