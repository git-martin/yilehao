using System.Data.Entity;
using BntWeb.Feedback.Models;
using BntWeb.Data;


namespace BntWeb.Feedback
{
    public class FeedbackDbContext: BaseDbContext
    {
        public DbSet<Models.Feedback> Feedback { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}