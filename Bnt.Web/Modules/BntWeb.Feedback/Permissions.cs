using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BntWeb.Security.Permissions;

namespace BntWeb.Feedback
{
    public class Permissions: IPermissionProvider
    {
        private static readonly string CategoryKey = FeedbackModule.DisplayName;

        public const string ViewFeedbackKey = "BntWeb-Feedback-ViewFeedback";
        public static readonly Permission ViewFeedback = new Permission { Description = "查看反馈", Name = ViewFeedbackKey, Category = CategoryKey };

        public const string ProcesseFeedbackKey = "BntWeb-Feedback-ProcesseFeedback";
        public static readonly Permission ProcesseFeedback = new Permission { Description = "处理反馈", Name = ProcesseFeedbackKey, Category = CategoryKey };

        public const string DeleteFeedbackKey = "BntWeb-Feedback-DeleteFeedback";
        public static readonly Permission DeleteFeedback = new Permission { Description = "删除反馈", Name = DeleteFeedbackKey, Category = CategoryKey };

        public int Position => FeedbackModule.Position;

        public string Category => CategoryKey;

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewFeedback,
                ProcesseFeedback,
                DeleteFeedback
            };
        }
    }
}