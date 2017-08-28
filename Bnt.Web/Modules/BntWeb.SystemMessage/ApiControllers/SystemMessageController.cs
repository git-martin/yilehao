/* 
    ======================================================================== 
        File name：		TagController
        Module:			
        Author：		        罗嗣宝
        Create Time：		2016/6/22 10:19:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Http;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using Autofac;
using BntWeb.MemberBase;
using BntWeb.MemberBase.Services;
using BntWeb.SystemMessage.ApiModels;
using BntWeb.SystemMessage.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;


namespace BntWeb.SystemMessage.ApiControllers
{
    public class SystemMessageController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISystemMessageService _systemMessageService;

        public SystemMessageController(ICurrencyService currencyService, ISystemMessageService systemMessageService)
        {
            _currencyService = currencyService;
            _systemMessageService = systemMessageService;
        }

        /// <summary>
        /// 加载系统消息分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult SystemMessageService()
        {
            var result = new ApiResult();
            var items = typeof(MessageCategory).GetItems();
            var categoryList = new List<SystemMessageCategory>();
            foreach (var item in items)
            {
                var category = new SystemMessageCategory();
                category.Category = (MessageCategory)((int)item.Value);
                category.Name = category.Category.Description();
                int unreadCount;
                category.FirstMessage = _systemMessageService.GetFirstMessage(category.Category,
                    AuthorizedUser.Id, out unreadCount);
                category.UnreadCount = unreadCount;

                categoryList.Add(category);
            }

            result.SetData(categoryList);

            return result;
        }

        /// <summary>
        /// 分页加载系统消息
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuthentication]
        public ApiResult SystemMessageService(MessageCategory category, int pageNo = 1, int limit = 10)
        {
            var result = new ApiResult();
            int totalCount;
            var list = _systemMessageService.LoadSystemMessageByPage(category, AuthorizedUser.Id, pageNo, limit, out totalCount);
            var data = new { TotalCount = totalCount, Messages = list };
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult SetRead(Guid messageId)
        {
            if (messageId == Guid.Empty)
                throw new WebApiInnerException("0001", "消息Id不合法");

            var result = new ApiResult();
            var data = _systemMessageService.SetRead(AuthorizedUser.Id, messageId);
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPatch]
        [BasicAuthentication]
        public ApiResult SetReadAll(MessageCategory category)
        {
            var result = new ApiResult();
            var data = _systemMessageService.SetRead(AuthorizedUser.Id, category);
            result.SetData(data);
            return result;
        }

        /// <summary>
        /// 删除系统消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete]
        [BasicAuthentication]
        public ApiResult DeleteMessage(Guid messageId)
        {
            var result = new ApiResult();
            if (messageId.Equals(Guid.Empty))
                throw new WebApiInnerException("0001", "消息Id不合法");
            if (!_systemMessageService.Delete(AuthorizedUser.Id, messageId))
                throw new WebApiInnerException("0002", "系统消息删除失败");

            return result;

        }

    }
}