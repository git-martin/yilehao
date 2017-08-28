/* 
    ======================================================================== 
        File name：        RoleService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/5/24 9:53:18
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using BntWeb.Caching;
using BntWeb.Core.SystemUsers.Models;
using BntWeb.Data.Services;
using BntWeb.Logging;
using BntWeb.Security;
using BntWeb.Security.Identity;
using BntWeb.Security.Permissions;
using BntWeb.UI.Navigation;
using Microsoft.AspNet.Identity;

namespace BntWeb.Core.SystemUsers.Services
{
    public class RoleService : IRoleService
    {
        private readonly DefaultRoleManager _roleManager;
        private readonly ICurrencyService _currencyService;
        private readonly IUserContainer _userContainer;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public RoleService(DefaultRoleManager roleManager, ICurrencyService currencyService, IUserContainer userContainer, ICacheManager cacheManager, ISignals signals)
        {
            _roleManager = roleManager;
            _currencyService = currencyService;
            _userContainer = userContainer;
            _cacheManager = cacheManager;
            _signals = signals;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public List<Role> GetAdminRoles()
        {
            return _roleManager.Roles.Where(r => !r.Hidden && r.RoleType == RoleType.Admin).ToList();
        }

        public Role FindById(string roleId)
        {
            var role = _roleManager.FindByIdAsync(roleId).Result;
            var permissions = _currencyService.GetList<RolePermission>(rp => rp.RoleId.Equals(role.Id, StringComparison.OrdinalIgnoreCase)) ??
                              new List<RolePermission>();
            role.Permissions = permissions;
            return role;
        }


        public IdentityResult CreateRole(Role role, List<string> permissions)
        {
            var result = _roleManager.Create(role);
            if (permissions != null)
                foreach (var permission in permissions)
                {
                    _currencyService.Create(new RolePermission { RoleId = role.Id, PermissionName = permission });
                }

            Logger.Operation($"添加后台用户角色-{role.DisplayName}:{role.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
            return result;
        }

        public IdentityResult UpdateRole(Role role, List<string> permissions)
        {
            var result = _roleManager.Update(role);
            if (result.Succeeded)
            {
                //删除旧权限
                _currencyService.DeleteByConditon<RolePermission>(rp => rp.RoleId.Equals(role.Id, StringComparison.OrdinalIgnoreCase));
                if (permissions != null)
                    foreach (var permission in permissions)
                    {
                        _currencyService.Create(new RolePermission { RoleId = role.Id, PermissionName = permission });
                    }

                Logger.Operation($"修改后台用户角色-{role.DisplayName}:{role.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
                _signals.Trigger(_userContainer.UserChangedSignalName);
            }
            return result;
        }

        public IdentityResult Delete(Role role)
        {
            var result = _roleManager.Delete(role);
            if (result.Succeeded)
            {
                //删除旧权限
                _currencyService.DeleteByConditon<RolePermission>(
                    rp => rp.RoleId.Equals(role.Id, StringComparison.OrdinalIgnoreCase));

                Logger.Operation($"删除后台用户角色-{role.DisplayName}:{role.Id}", SystemUsersModule.Instance, SecurityLevel.Warning);
                _signals.Trigger("user_changed");
            }
            return result;
        }

        public List<PermissionGroup> MatchPermissions(IEnumerable<IPermissionProvider> permissionProviders)
        {
            //加载权限配置文件，根据自定义从新排序
            return _cacheManager.Get("BntWeb-Permissions-Config", ctx =>
            {
                var permissionsConfig = HostingEnvironment.MapPath("~/Config/Permissions.config");
                if (System.IO.File.Exists(permissionsConfig))
                {
                    var doc = XDocument.Load(permissionsConfig);
                    var groups = doc.Elements("Permissions").Elements("Group").Select(group => new PermissionGroup(group)).ToList();
                    var newGroups = new List<PermissionGroup>();
                    foreach (var group in groups)
                    {
                        var newGroup = new PermissionGroup() { Category = group.Category };
                        foreach (var item in group.Items)
                        {
                            var isIn = false;
                            foreach (var permissionProvider in permissionProviders)
                            {
                                isIn =
                                    permissionProvider.GetPermissions()
                                        .Any(p => p.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

                                if (isIn) break;
                            }

                            if (isIn)
                            {
                                newGroup.Items.Add(item);
                            }
                        }

                        if (newGroup.Items.Count > 0)
                            newGroups.Add(newGroup);
                    }

                    return newGroups;
                }
                else
                {
                    var groups = new List<PermissionGroup>();
                    var doc = new XmlDocument();
                    var permissionsElement = doc.CreateElement("Permissions");
                    foreach (var permissionProvider in permissionProviders)
                    {
                        var group = new PermissionGroup()
                        {
                            Category = permissionProvider.Category
                        };

                        var groupElement = doc.CreateElement("Group");
                        groupElement.SetAttribute("Category", group.Category);

                        foreach (var permission in permissionProvider.GetPermissions())
                        {
                            group.Items.Add(new PermissionItem { Name = permission.Name, Description = permission.Description });

                            var permissionElement = doc.CreateElement("Permission");
                            permissionElement.SetAttribute("Name", permission.Name);
                            permissionElement.SetAttribute("Description", permission.Description);
                            groupElement.AppendChild(permissionElement);
                        }

                        groups.Add(group);

                        permissionsElement.AppendChild(groupElement);
                    }
                    doc.AppendChild(permissionsElement);
                    doc.Save(permissionsConfig);
                    return groups;
                }
            });
        }
    }
}