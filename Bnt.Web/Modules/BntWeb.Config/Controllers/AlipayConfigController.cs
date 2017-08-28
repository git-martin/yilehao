using System;
using System.Linq;
using System.Web.Mvc;
using BntWeb.Config.Models;
using BntWeb.FileSystems.Media;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Services;

namespace BntWeb.Config.Controllers
{
    public class AlipayConfigController : Controller
    {
        private readonly IConfigService _configService;
        private readonly IStorageFileService _storageFileService;

        public AlipayConfigController(IConfigService configService, IStorageFileService storageFileService)
        {
            _configService = configService;
            _storageFileService = storageFileService;
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.SystemConfigKey })]
        public ActionResult Config()
        {
            var config = _configService.Get<AlipayConfig>();
            return View(config);
        }

        [HttpPost]
        [AdminAuthorize(PermissionsArray = new[] { Permissions.SystemConfigKey })]
        public ActionResult SaveConfig(AlipayConfig configViewModel)
        {
            var result = new DataJsonResult();
            if (!_configService.Save(configViewModel))
            {
                result.ErrorMessage = "异常错误，配置文件保存失败";
            }
            return Json(result);
        }
    }
}