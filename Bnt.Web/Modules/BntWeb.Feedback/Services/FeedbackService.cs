using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Feedback.Models;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Models;

namespace BntWeb.Feedback.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IFileService _fileService;
        private readonly IStorageFileService _storageFileService;
        private const string FileTypeName = "FeedbackImage";

        public ILogger Logger { get; set; }
        public FeedbackService(ICurrencyService currencyService, IStorageFileService storageFileService, IFileService fileService)
        {
            _currencyService = currencyService;
            _storageFileService = storageFileService;
            _fileService = fileService;
        }

        public List<Models.Feedback> GetListPaged<TKey>(int pageIndex, int pageSize, Expression<Func<Models.Feedback, bool>> expression, Expression<Func<Models.Feedback, TKey>> orderByExpression, bool isDesc, out int totalCount)
        {
            using (var dbContext = new FeedbackDbContext())
            {
                var query = dbContext.Feedback.Where(expression);
                totalCount = query.Count();
                query = isDesc ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        public Models.Feedback GetFeedbackById(Guid id)
        {
            var feedback = _currencyService.GetSingleById<Models.Feedback>(id);
            feedback.Images = _storageFileService.GetFiles(id, FeedbackModule.Key, FileTypeName).ToList();
            return feedback;
        }

        public bool UpdateFeedback(Models.Feedback feedback)
        {
            var result = _currencyService.Update<Models.Feedback>(feedback);
            if (result)
                Logger.Operation($"处理反馈:{feedback.Id}", FeedbackModule.Instance, SecurityLevel.Normal);
            return result;
        }

        public bool Delete(Models.Feedback feedback)
        {
            //逻辑删除反馈
            feedback.Status = FeedbackStatus.Delete;
            var result = _currencyService.Update(feedback);

            if (result)
                Logger.Operation($"删除反馈:{feedback.Id}", FeedbackModule.Instance, SecurityLevel.Warning);

            return result;
        }

        /// <summary>
        /// 新增反馈
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="imageFiles"></param>
        /// <returns></returns>
        public void CreateFeedback(Models.Feedback feedback, List<Base64Image> imageFiles = null)
        {
            Argument.ThrowIfNull(imageFiles, "反馈照片");
            if (imageFiles != null)
                foreach (var imgFile in imageFiles)
                {
                    Argument.ThrowIfNullOrEmpty(imgFile.Data, "图片数据");
                    Argument.ThrowIfNullOrEmpty(imgFile.FileName, "图片文件名");
                    if (string.IsNullOrWhiteSpace(Path.GetExtension(imgFile.FileName)))
                        throw new WebApiInnerException("0003", "文件名称没有包含扩展名");
                }

            feedback.Id = KeyGenerator.GetGuidKey();
            feedback.CreateTime = DateTime.Now;
            feedback.Status = FeedbackStatus.Normal;
            feedback.ProcesseStatus = (int)ProcesseStatus.Untreated;
            var result = _currencyService.Create<Models.Feedback>(feedback);
            if (!result)
            {
                throw new WebApiInnerException("1002", "参数值有误");//return Guid.Empty;
            }

            if (imageFiles != null)
                foreach (var imgFile in imageFiles)
                {
                    //上传图片
                    StorageFile storageFile;
                    if (_fileService.SaveFile(imgFile.Data, imgFile.FileName, false, out storageFile, 200, 200, 100, 100, ThumbnailType.TakeCenter))
                    {
                        //关联图片
                        if (!_storageFileService.AssociateFile(feedback.Id, FeedbackModule.Key, FeedbackModule.DisplayName, storageFile.Id, FileTypeName))
                        {
                            throw new WebApiInnerException("1003", "图片关联失败");
                        }
                    }
                    else
                    {
                        throw new WebApiInnerException("1004", "图片上传失败");
                    }
                }

            Logger.Operation($"创建反馈:{feedback.Id}", FeedbackModule.Instance, SecurityLevel.Normal);
        }

        /// <summary>
        /// 获取反馈列表 平台类
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="feedbackType"></param>
        /// <param name="sourceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Models.Feedback> GetFeedbackList(Guid memberId, FeedbackType feedbackType, string sourceType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new FeedbackDbContext())
            {
                var query = dbContext.Feedback.Where((Expression<Func<Models.Feedback, bool>>)(o => o.Status == FeedbackStatus.Normal && o.FeedbackType == feedbackType && o.SourceType == sourceType && o.MemberId == memberId));

                totalCount = query.Count();

                query = query.OrderByDescending(me => me.CreateTime);
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }

        /// <summary>
        /// 根据来源id获取反馈列表 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="souceId"></param>
        /// <param name="feedbackType"></param>
        /// <param name="sourceType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Models.Feedback> GetFeedbackListBySourceId(Guid memberId, Guid souceId, FeedbackType feedbackType, string sourceType, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new FeedbackDbContext())
            {
                var query = dbContext.Feedback.Where(o => o.Status == FeedbackStatus.Normal && o.SourceId == souceId && o.FeedbackType == feedbackType && o.SourceType == sourceType && o.MemberId == memberId);

                totalCount = query.Count();

                query = query.OrderByDescending(me => me.CreateTime);
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return list;
            }
        }
    }
}