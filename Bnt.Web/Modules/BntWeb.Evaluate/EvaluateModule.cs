using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Evaluate
{
    public class EvaluateModule : IBntWebModule
    {
        public static readonly EvaluateModule Instance = new EvaluateModule();

        public string InnerKey => "BntWeb-Evaluate";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "评价管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Evaluate";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 8000;
        public static int Position => Instance.InnerPosition;
    }
}