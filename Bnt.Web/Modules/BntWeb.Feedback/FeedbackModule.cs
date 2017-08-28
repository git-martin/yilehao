using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Environment;

namespace BntWeb.Feedback
{
    public class FeedbackModule: IBntWebModule
    {
        public static readonly FeedbackModule Instance = new FeedbackModule();

        public string InnerKey => "BntWeb-Feedback";
        public static string Key => Instance.InnerKey;
        public string InnerDisplayName => "反馈管理";
        public static string DisplayName => Instance.InnerDisplayName;
        public string InnerArea => "Feedback";
        public static string Area => Instance.InnerArea;
        public int InnerPosition => 7000;
        public static int Position => Instance.InnerPosition;
    }
}