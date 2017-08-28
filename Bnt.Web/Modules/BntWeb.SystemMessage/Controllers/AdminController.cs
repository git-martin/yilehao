using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;

using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Logging;
using BntWeb.MemberBase.Models;
using BntWeb.SystemMessage.Services;
using BntWeb.SystemMessage.ViewModels;

namespace BntWeb.SystemMessage.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISystemMessageService _systemMessageService;

        public AdminController(ICurrencyService currencyService, ISystemMessageService systemMessageService)
        {
            _currencyService = currencyService;
            _systemMessageService = systemMessageService;

            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewSystemMessageKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewSystemMessageKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            var list = _systemMessageService.GetSystemMessageListByPage(pageIndex, pageSize, out totalCount);
            result.data = list;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditSystemMessageKey })]
        public ActionResult CreateSystemMessage()
        {
            var systemMessage = new Models.SystemMessage { Id = Guid.Empty };
            return View("Edit", systemMessage);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditSystemMessageKey })]
        public ActionResult EditOnPost(SystemMessageViewModel systemMessage)
        {
            var result = new DataJsonResult();
            if (systemMessage.SentType == 1)
            {
                List<Member> members=null;
                if (systemMessage.MemberType == 0)
                    members = _currencyService.GetList<Member>(me => me.UserType == Security.Identity.UserType.Member);
                //else
                //    members = _currencyService.GetList<Member>(me => me.MemberType == systemMessage.MemberType && me.UserType == Security.Identity.UserType.Member);
                if (members == null || members.Count == 0)
                    throw new Exception("该分类没有会员");

           if (!_systemMessageService.CreatePushSystemMessage(systemMessage.Title, systemMessage.Content, null,null, null, null, "System", SystemMessageModule.Key))
                        throw new Exception("发送消息失败");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(systemMessage.UserName))
                    throw new Exception("请输入用户名");

                var member = _currencyService.GetSingleByConditon<Member>(me => me.UserName == systemMessage.UserName);
                if (member == null)
                    throw new Exception("会员不存在");

                if (!_systemMessageService.CreatePushSystemMessage(systemMessage.Title, systemMessage.Content, null, member.Id, null, null, "System", SystemMessageModule.Key))
                    throw new Exception("发送消息失败");

            }



            //if (!_systemMessageService.CreatePushSystemMessage(systemMessage.Title, systemMessage.Content, null, null, null, null, "System", SystemMessageModule.Key))
            //{
            //    result.ErrorMessage = "保存失败";
            //}

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteSystemMessageKey })]
        public ActionResult Delete(Guid systemMessageId)
        {
            var result = new DataJsonResult();
            _currencyService.DeleteByConditon<Models.SystemMessage>(s => s.Id.Equals(systemMessageId));
            _currencyService.DeleteByConditon<Models.SystemMessageReciever>(s => s.MessageId.Equals(systemMessageId));
            return Json(result);
        }

    }
}