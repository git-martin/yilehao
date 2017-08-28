using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BntWeb.Caching;
using BntWeb.Core.SystemUsers.Services;
using BntWeb.Core.SystemUsers.ViewModels;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Utility.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemUsers.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserContainer _userContainer;

        public AdminController(IUserService userService, IRoleService roleService, IUserContainer userContainer)
        {
            _userService = userService;
            _roleService = roleService;

            _userContainer = userContainer;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditUserKey })]
        public ActionResult Edit(string id)
        {
            ViewBag.Roles = _roleService.GetAdminRoles();
            ViewBag.EditMode = false;
            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.EditMode = true;
                return View(_userService.FindUserById(id));
            }
            return View(new User { Id = string.Empty });
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditUserKey })]
        public ActionResult EditOnPost(EditUserViewModel editUser)
        {
            var result = new DataJsonResult();
            User oldUser = null;
            if (!string.IsNullOrWhiteSpace(editUser.UserId))
                oldUser = _userService.FindUserByName(editUser.UserName);

            if (oldUser == null)
            {
                //新建用户
                User user = _userService.FindUserByName(editUser.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = editUser.UserName,
                        Email = editUser.Email,
                        PhoneNumber = editUser.PhoneNumber,
                        LockoutEnabled = false
                    };

                    var identityResult = _userService.CreateAdminUser(user, editUser.Password, editUser.Roles);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
                else
                {
                    result.ErrorMessage = "此用户名的账号已经存在！";
                }
            }
            else
            {
                //编辑用户
                oldUser.Email = editUser.Email;
                oldUser.PhoneNumber = editUser.PhoneNumber;

                var identityResult = _userService.UpdateAdminUser(oldUser, editUser.Password, editUser.Password2, editUser.Roles);
                if (!identityResult.Succeeded)
                {
                    result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                }
            }

            return Json(result);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewUserKey })]
        public ActionResult List()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewUserKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            Expression<Func<User, object>> orderByExpression;
            //设置排序
            switch (sortColumn)
            {
                case "CreateTime":
                    orderByExpression = u => new { u.CreateTime };
                    break;
                case "PhoneNumber":
                    orderByExpression = u => new { u.PhoneNumber };
                    break;
                case "Email":
                    orderByExpression = u => new { u.Email };
                    break;
                case "LockoutEnabled":
                    orderByExpression = u => new { u.LockoutEnabled };
                    break;
                default:
                    orderByExpression = u => new { u.UserName };
                    break;
            }

            //分页查询
            var users = _userContainer.GetListPaged(pageIndex, pageSize, u => u.UserType == UserType.Admin && !u.UserName.Equals(_userContainer.CurrentUser.UserName, StringComparison.OrdinalIgnoreCase), orderByExpression, isDesc, out totalCount);

            foreach (var user in users)
            {
                var tempUser = _userContainer.FindUser(user.UserName);
                if (tempUser != null)
                    user.AllRoles = tempUser.AllRoles;
            }

            result.data = users;
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditUserKey })]
        public ActionResult Switch(SwitchUserViewModel switchUser)
        {
            var result = new DataJsonResult();
            User user = _userService.FindUserById(switchUser.UserId);

            if (user != null)
            {
                if (user.UserName.Equals("bocadmin", StringComparison.OrdinalIgnoreCase))
                {
                    result.ErrorMessage = "内置账号不可以禁用！";
                }
                else
                {
                    var identityResult = _userService.SetLockoutEnabled(user.Id, switchUser.Enabled);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
            }

            else
            {
                result.ErrorMessage = "此用户名的账号不存在！";
            }

            return Json(result);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteUserKey })]
        public ActionResult Delete(string userId)
        {
            var result = new DataJsonResult();
            User user = _userService.FindUserById(userId);

            if (user != null)
            {
                if (user.UserName.Equals("bocadmin", StringComparison.OrdinalIgnoreCase))
                {
                    result.ErrorMessage = "内置账号不可以删除！";
                }
                else
                {
                    var identityResult = _userService.Delete(user);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
            }
            else
            {
                result.ErrorMessage = "此用户名的账号不存在！";
            }

            return Json(result);
        }


        [AdminAuthorize]
        public ActionResult Current()
        {
            return View(_userContainer.CurrentUser);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult ModifyCurrent(EditUserViewModel editUser)
        {
            var result = new DataJsonResult();

            //编辑用户
            _userContainer.CurrentUser.Email = editUser.Email;
            _userContainer.CurrentUser.PhoneNumber = editUser.PhoneNumber;

            var identityResult = _userService.UpdateAdminUser(_userContainer.CurrentUser, editUser.Password, editUser.Password2, editUser.Roles, false);
            if (!identityResult.Succeeded)
            {
                result.ErrorMessage = identityResult.Errors.FirstOrDefault();
            }

            return Json(result);
        }
    }

}