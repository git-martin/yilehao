using System.Collections.Generic;
using BntWeb.UI.Navigation;

namespace BntWeb.Feedback
{
    public class AdminMenu: INavigationProvider
    {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(FeedbackModule.Key, FeedbackModule.DisplayName, FeedbackModule.Position.ToString(), BuildMenu, new List<string> { "icon-bullhorn" });
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(FeedbackModule.Key + "-FeedbackList", "意见列表", "10",
                item => item
                    .Action("List", "Admin", new { area = FeedbackModule.Area, feedbackType=1, sourceType = "Common" })
                    .Permission(Permissions.ViewFeedback)
                );
            menu.Add(FeedbackModule.Key + "-MessageList", "留言列表", "20",
                item => item
                    .Action("List", "Admin", new { area = FeedbackModule.Area, feedbackType = 2, sourceType = "Common" })
                    .Permission(Permissions.ViewFeedback)
                );
            menu.Add(FeedbackModule.Key + "-ComplaintList", "投诉列表", "30",
                item => item
                    .Action("List", "Admin", new { area = FeedbackModule.Area, feedbackType = 3, sourceType = "Common" })
                    .Permission(Permissions.ViewFeedback)
                );
            
        }
    }
}