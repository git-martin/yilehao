/* 
    ======================================================================== 
        File name：        CommentService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/17 15:42:55
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Services;
using BntWeb.Security;
using System.Data.Entity;
using LinqKit;

namespace BntWeb.Comment.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        private readonly IMemberService _memberService;

        public CommentService(ICurrencyService currencyService, IStorageFileService storageFileService, IMemberService memberService)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;
            _memberService = memberService;
        }

        public void SaveComment(Models.Comment comment)
        {
            using (var dbContext = new CommentDbContext())
            {
                dbContext.Comments.Add(comment);
                if (dbContext.SaveChanges() > 0 && comment.Files != null)
                {
                    foreach (var file in comment.Files)
                    {
                        _storageFileService.AssociateFile(comment.Id, CommentModule.Key, CommentModule.DisplayName,
                            file.Id);
                    }
                }
            }
        }

        public List<Models.Comment> LoadByPage(Guid sourceId, IBntWebModule module, string sourceType, out int totalCount, int pageIndex = 1, int pageSize = 10)
        {
            var checkModule = module != null;
            var moduleKey = checkModule ? module.InnerKey : null;
            Expression<Func<Models.Comment, bool>> expression =
                                                   c => c.SourceId.Equals(sourceId)
                                                   && c.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase)
                                                   && (c.ParentId == null || c.ParentId == Guid.Empty)
                                                   && (!checkModule || (checkModule && c.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase)));
            List<Models.Comment> list;
            using (var dbContext = new CommentDbContext())
            {
                totalCount = dbContext.Comments.AsExpandable().Where(expression).Count();

                list = dbContext.Comments.AsExpandable().Where(expression).OrderByDescending(c => c.CreateTime).Include(c => c.Files).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

            foreach (var item in list)
            {
                BindMemberInfo(item);
                item.ChildComments = GetAllChilds(item.Id);
            }
            return list;
        }

        private List<Models.Comment> GetAllChilds(Guid parentId)
        {
            List<Models.Comment> list;
            using (var dbContext = new CommentDbContext())
            {
                list = dbContext.Comments.AsExpandable().Where(c => c.ParentId == parentId).OrderByDescending(c => c.CreateTime).Include(c => c.Files).ToList();
            }
            foreach (var item in list)
            {
                BindMemberInfo(item);
                item.ChildComments = GetAllChilds(item.Id);
            }
            return list;
        }

        public void DeleteComment(Guid commentId)
        {
            using (var dbContext = new CommentDbContext())
            {
                var comment = dbContext.Comments.FirstOrDefault(c => c.Id == commentId);
                DeleteCommentAndChild(comment, dbContext);
                dbContext.SaveChanges();
            }
        }

        private void DeleteCommentAndChild(Models.Comment comment, CommentDbContext dbContext)
        {
            if (comment != null)
            {
                var childs = dbContext.Comments.Where(c => c.ParentId == comment.Id).ToList();
                foreach (var child in childs)
                {
                    DeleteCommentAndChild(child, dbContext);
                }

                dbContext.Comments.Remove(comment);
                _storageFileService.DisassociateFile(comment.Id, CommentModule.Key);
            }
        }

        public void DeleteCommentBySourceId(Guid sourceId)
        {
            using (var dbContext = new CommentDbContext())
            {
                var comments = dbContext.Comments.Where(c => c.SourceId == sourceId).ToList();
                foreach (var comment in comments)
                {
                    DeleteCommentAndChild(comment, dbContext);
                }
                dbContext.SaveChanges();
            }
        }

        public Models.Comment GetComment(Guid commentId)
        {
            using (var dbContext = new CommentDbContext())
            {
                var comment = dbContext.Comments.FirstOrDefault(c => c.Id == commentId);
                return BindMemberInfo(comment);
            }
        }

        private Models.Comment BindMemberInfo(Models.Comment comment)
        {
            var member = _memberService.FindMemberById(comment.MemberId);
            if (member != null)
            {
                comment.MemberName = member.NickName;
                comment.MemberPhone = member.PhoneNumber;
            }

            return comment;
        }
    }
}