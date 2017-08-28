using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.MemberBase.Services;
using BntWeb.Security;
using LinqKit;


namespace BntWeb.Evaluate.Services
{
    public class EvaluateService : IEvaluateService
    {
        private readonly IUserContainer _userContainer;
        private readonly ICurrencyService _currencyService;
        private readonly IStorageFileService _storageFileService;
        private readonly IMemberService _memberService;
        private readonly IEnumerable<IEvaluateServiceProxy> _evaluateServiceProxy;
        

        public EvaluateService(IUserContainer userContainer, ICurrencyService currencyService, IStorageFileService storageFileService, IMemberService memberService, IEnumerable<IEvaluateServiceProxy> evaluateServiceProxy)
        {
            _userContainer = userContainer;
            _currencyService = currencyService;
            _storageFileService = storageFileService;
            _memberService = memberService;
            _evaluateServiceProxy = evaluateServiceProxy;
        }

        public void SaveEvaluate(Models.Evaluate evaluate)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                if (dbContext.Evaluate.Count(
                        e =>
                            e.MemberId.Equals(evaluate.MemberId, StringComparison.OrdinalIgnoreCase) &&
                            e.SourceId.Equals(evaluate.SourceId)) > 0)
                {
                    throw new Exception("不可以重复评价");
                }

                dbContext.Evaluate.Add(evaluate);
                if (dbContext.SaveChanges() > 0 && evaluate.Files != null)
                {
                    foreach (var file in evaluate.Files)
                    {
                        _storageFileService.AssociateFile(evaluate.Id, EvaluateModule.Key, EvaluateModule.DisplayName,
                            file.Id);
                    }
                }
            }
        }

        public List<Models.Evaluate> LoadByPage(Guid sourceId, IBntWebModule module, string sourceType, out int totalCount, int pageIndex = 1, int pageSize = 10)
        {
            var checkModule = module != null;
            var moduleKey = checkModule ? module.InnerKey : null;
            Expression<Func<Models.Evaluate, bool>> expression =
                                                   c => c.SourceId.Equals(sourceId)
                                                   && c.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase)
                                                   && (!checkModule || (checkModule && c.ModuleKey.Equals(moduleKey, StringComparison.OrdinalIgnoreCase)));


            using (var dbContext = new EvaluateDbContext())
            {
                totalCount = dbContext.Evaluate.AsExpandable().Where(expression).Count();

                return dbContext.Evaluate.AsExpandable().Where(expression).OrderByDescending(c => c.CreateTime).Include(c => c.Files).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public void DeleteEvaluate(Guid evaluateId)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                var evaluate = dbContext.Evaluate.FirstOrDefault(c => c.Id == evaluateId);
                dbContext.Evaluate.Remove(evaluate);
                dbContext.SaveChanges();
            }
        }

        public bool ApprvoeEvaluate(List<Guid> evaluateId)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                var arrEvaluateIds = evaluateId.ToArray();
                var evaluateList = dbContext.Evaluate.Where(e => arrEvaluateIds.Contains(e.Id)).ToList();
                if (evaluateList.Any())
                {
                    foreach (var evaluate in evaluateList)
                    {
                        evaluate.Status = Models.ApproveStatus.Approved;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool DeleteEvaluates(List<Guid> evaluateIds)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                var arrGoodsIds = evaluateIds.ToArray();
                var goodsList = dbContext.Evaluate.Where(g => arrGoodsIds.Contains(g.Id)).ToList();
                if (goodsList.Any())
                {
                    foreach (var goods in goodsList)
                    {
                        dbContext.Evaluate.Remove(goods);
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public List<StorageFile> GetEvaluateFile(Guid evaluateId, string fileTypeName)
        {
            return _storageFileService.GetFiles(evaluateId, EvaluateModule.Key, fileTypeName).ToList();
        }


        public Models.Evaluate GetEvaluate(Guid evaluateId)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                var evaluate = dbContext.Evaluate.FirstOrDefault(c => c.Id == evaluateId);
                return evaluate;
            }
        }

        /// <summary>
        /// 添加订单评论
        /// </summary>
        /// <param name="evaluates"></param>
        /// <param name="orderId"></param>
        public void CreateOrderEvaluates(List<Models.Evaluate> evaluates, Guid orderId)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                if (evaluates.Count > 0)
                {
                    foreach (var evaluateInfo in evaluates)
                    {
                        if (dbContext.Evaluate.Count(
                        e =>
                            e.MemberId.Equals(evaluateInfo.MemberId, StringComparison.OrdinalIgnoreCase) &&
                            e.SourceId.Equals(evaluateInfo.SourceId)) > 0)
                        {
                            throw new Exception("不可以重复评价");
                        }
                        evaluateInfo.Id = KeyGenerator.GetGuidKey();
                        evaluateInfo.CreateTime = DateTime.Now;
                        dbContext.Evaluate.Add(evaluateInfo);
                    }

                    if (dbContext.SaveChanges() > 0)
                    {
                        foreach (var evaluateModel in evaluates)
                        {
                            if (evaluateModel.FilesId != null)
                            {
                                foreach (var fileId in evaluateModel.FilesId)
                                {
                                    _storageFileService.AssociateFile(evaluateModel.Id, EvaluateModule.Key, EvaluateModule.DisplayName,
                                        fileId, "Evaluate");
                                }
                            }
                        }

                        foreach (var proxy in _evaluateServiceProxy)
                        {
                            if (proxy.ModuleKey().Equals(evaluates[0].ModuleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                proxy.AfterCreateOrderEvaluates(orderId);
                            }
                        }
                    }
                }
            }
        }

        public void ReplayOrderEvaluates(List<Models.Evaluate> evaluates, Guid orderId,string moduleKey)
        {
            using (var dbContext = new EvaluateDbContext())
            {
                if (evaluates.Count > 0)
                {
                    var currentUser = _userContainer.CurrentUser;
                    
                    foreach (var evaluateInfo in evaluates)
                    {
                        var model = dbContext.Evaluate.FirstOrDefault(x => x.Id == evaluateInfo.Id);
                        if (model != null)
                        {
                            model.ReplyContent = evaluateInfo.ReplyContent;
                            model.ReplyTime = DateTime.Now;
                            model.ReplyUserId = currentUser.Id;
                            model.ReplyUserName = currentUser.UserName;
                        }
                    }

                    if (dbContext.SaveChanges() > 0)
                    {
                        foreach (var proxy in _evaluateServiceProxy)
                        {
                            if (proxy.ModuleKey().Equals(moduleKey, StringComparison.OrdinalIgnoreCase))
                            {
                                proxy.AfterReplayOrderEvaluates(orderId);
                            }
                        }
                    }
                }
            }
        }
    }
}