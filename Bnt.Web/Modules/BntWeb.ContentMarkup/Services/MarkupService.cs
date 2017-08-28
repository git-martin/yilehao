/* 
    ======================================================================== 
        File name：        MarkupService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 14:14:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.ContentMarkup.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;

namespace BntWeb.ContentMarkup.Services
{
    public class MarkupService : IMarkupService
    {
        private readonly ICurrencyService _currencyService;

        public MarkupService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool CreateMarkup(Guid sourceId, string moduleKey, string memberId, MarkupType markupType, string sourceType = null, string moduleName = null)
        {
            try
            {
                var markup = new Markup
                {
                    Id = KeyGenerator.GetGuidKey(),
                    SourceId = sourceId,
                    ModuleKey = moduleKey,
                    MemberId = memberId,
                    MarkupType = markupType,
                    SourceType = sourceType,
                    CreateTime = DateTime.Now,
                    ModuleName = moduleName
                };

                return _currencyService.Create(markup);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "创建标记失败");
                return false;
            }
        }

        public bool CancelMarkup(Guid sourceId, string moduleKey, string memberId, MarkupType markupType)
        {
            try
            {
                return _currencyService.DeleteByConditon<Markup>(
                      m =>
                          m.SourceId == sourceId && m.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) &&
                          m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && m.MarkupType == markupType) > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "取消标记出错了");
                return false;
            }
        }

        public bool ClearMarkup(string moduleKey, string memberId, MarkupType markupType)
        {
            try
            {
                return _currencyService.DeleteByConditon<Markup>(
                      m =>
                          m.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) &&
                          m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && m.MarkupType == markupType) > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "清理标记出错了");
                return false;
            }
        }

        public bool MarkupExist(Guid sourceId, string moduleKey, string memberId, MarkupType markupType)
        {
            return _currencyService.Count<Markup>(m => m.SourceId == sourceId && m.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) &&
                          m.MemberId.Equals(memberId, StringComparison.OrdinalIgnoreCase) && m.MarkupType == markupType) > 0;
        }

        public int MarkupCount(Guid sourceId, string moduleKey, MarkupType markupType)
        {
            return
                _currencyService.Count<Markup>(
                    m =>
                        m.SourceId == sourceId && m.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) &&
                        m.MarkupType == markupType);
        }

        public List<Member> LoadByPage(Guid sourceId, string moduleKey, MarkupType markupType, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = MarkupCount(sourceId, moduleKey, markupType);

            using (var dbContext = new MarkupDbContext())
            {
                var members = (from m in dbContext.Markups
                               join member in dbContext.Members
                                   on m.MemberId equals member.Id
                               where
                                   m.SourceId == sourceId && m.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase) &&
                                   m.MarkupType == markupType
                                   orderby m.CreateTime descending
                               select member).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                return members.ToList();
            }
        }
    }
}