using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Core.SystemUsers.Services;
using BntWeb.Core.SystemUsers.ViewModels;
using BntWeb.Data;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Security.Permissions;
using BntWeb.Utility.Extensions;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemUsers.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        public RoleController(IEnumerable<IPermissionProvider> permissionProviders, IRoleService roleService)
        {
            _permissionProviders = permissionProviders.OrderBy(p => p.Position);
            _roleService = roleService;
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditRoleKey })]
        public ActionResult Edit(string id)
        {
            //加载所有权限
            ViewBag.PermissionGroups = _roleService.MatchPermissions(_permissionProviders);
            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.EditMode = true;
                return View(_roleService.FindById(id));
            }
            ViewBag.EditMode = false;
            return View(new Role { Id = string.Empty });
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.EditUserKey })]
        public ActionResult EditOnPost(EditRoleViewModel editRole)
        {
            var result = new DataJsonResult();
            Role oldRole = null;
            if (!string.IsNullOrWhiteSpace(editRole.RoleId))
                oldRole = _roleService.FindById(editRole.RoleId);

            if (oldRole == null)
            {
                var role = new Role
                {
                    Name = KeyGenerator.GetGuidKey().ToString(),
                    DisplayName = editRole.DisplayName,
                    Description = editRole.Description

                };
                var identityResult = _roleService.CreateRole(role, editRole.Permissions);
                if (!identityResult.Succeeded)
                {
                    result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                }
            }
            else
            {
                oldRole.DisplayName = editRole.DisplayName;
                oldRole.Description = editRole.Description;

                var identityResult = _roleService.UpdateRole(oldRole, editRole.Permissions);
                if (!identityResult.Succeeded)
                {
                    result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                }
            }

            return Json(result);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewRoleKey })]
        public ActionResult List()
        {
            return View();
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewRoleKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();
            //取参数值
            result.draw = Request["draw"].To<int>();
            var roles = _roleService.GetAdminRoles();
            result.data = roles;
            result.recordsTotal = roles.Count;
            result.recordsFiltered = roles.Count;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteRoleKey })]
        public ActionResult Delete(string roleId)
        {
            var result = new DataJsonResult();
            var role = _roleService.FindById(roleId);

            if (role != null)
            {
                if (!role.BuiltIn)
                {
                    var identityResult = _roleService.Delete(role);

                    if (!identityResult.Succeeded)
                    {
                        result.ErrorMessage = identityResult.Errors.FirstOrDefault();
                    }
                }
                else
                {
                    result.ErrorMessage = "内置角色不可以删除！";
                }
            }
            else
            {
                result.ErrorMessage = "此角色不存在！";
            }

            return Json(result);
        }
    }
}