using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Core.SystemSettings.Services;
using BntWeb.Data.Services;
using BntWeb.Environment;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Mvc;
using BntWeb.Security;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;
using BntWeb.Web.Extensions;

namespace BntWeb.Core.SystemSettings.Controllers
{
    public class FileController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IFileService _fileService;
        private readonly IStorageFileService _storageFileService;

        public FileController(ICurrencyService currencyService, IFileService fileService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _fileService = fileService;
            _storageFileService = storageFileService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewFileKey })]
        public ActionResult Index()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.UploadFileKey })]
        public ActionResult Upload()
        {
            return View();
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.UploadFileKey })]
        public ActionResult SaveUploadedFile(bool? isPublic, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = false)
        {
            if (isPublic == null) isPublic = true;

            var result = new DataJsonResult();

            //虽然是循环，其实一次只会提交一个文件
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                StorageFile storageFile;
                result.Success = _fileService.SaveFile(file, isPublic.Value, out storageFile, mediumThumbnailWidth, mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType, waterMark);
                if (result.Success)
                {
                    result.Data = storageFile;
                    break;
                }
            }

            return Json(result);
        }

        public ActionResult UploadedFileKindEdit(bool? isPublic, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = true)
        {
            if (isPublic == null) isPublic = true;

            var result = new DataJsonResult();


            HttpPostedFileBase file = Request.Files[0];
            StorageFile storageFile;
            result.Success = _fileService.SaveFile(file, isPublic.Value, out storageFile, mediumThumbnailWidth, mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType, waterMark);
            if (result.Success)
            {
                return Json(new { error = 0, url = "/" + storageFile.RelativePath });
            }
            return Json(new { error = 1, message = "上传失败" });
        }

        [HttpPost]
        public ActionResult UploadedBase64Image(string fileName, string data, bool? isPublic, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = false)
        {
            var result = new DataJsonResult();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                result.ErrorMessage = "图片文件名为空";
                return Json(result);
            }
            if (string.IsNullOrWhiteSpace(data))
            {
                result.ErrorMessage = "图片数据为空";
                return Json(result);
            }
            if (string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
            {
                result.ErrorMessage = "文件名称没有包含扩展名";
                return Json(result);
            }

            StorageFile storageFile;
            if (!_fileService.SaveFile(data, fileName, isPublic ?? true, out storageFile, mediumThumbnailWidth, mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType, waterMark))
            {
                result.ErrorMessage = "文件名称没有包含扩展名";
                return Json(result);
            }
            result.Data = storageFile.Simplified();
            return Json(result);
        }


        [AdminAuthorize(PermissionsArray = new[] { Permissions.ViewFileKey })]
        public ActionResult ListOnPage()
        {
            var result = new DataTableJsonResult();

            //取参数值
            int draw, pageIndex, pageSize, totalCount;
            string sortColumn;
            bool isDesc;
            Request.GetDatatableParameters(out draw, out pageIndex, out pageSize, out sortColumn, out isDesc);
            result.draw = draw;

            //取查询条件
            var fileType = Request.Get("extra_search[FileType]");
            var checkFileType = string.IsNullOrWhiteSpace(fileType);
            var fileTypeInt = fileType.To<int>();

            var fileName = Request.Get("extra_search[FileName]");
            var checkFileName = string.IsNullOrWhiteSpace(fileName);

            var createTimeBegin = Request.Get("extra_search[CreateTimeBegin]");
            var checkCreateTimeBegin = string.IsNullOrWhiteSpace(createTimeBegin);
            var createTimeBeginTime = createTimeBegin.To<DateTime>();

            var createTimeEnd = Request.Get("extra_search[CreateTimeEnd]");
            var checkCreateTimeEnd = string.IsNullOrWhiteSpace(createTimeEnd);
            var createTimeEndTime = createTimeEnd.To<DateTime>();

            Expression<Func<StorageFile, bool>> expression =
                l => (checkFileType || (int)l.FileType == fileTypeInt) &&
                     (checkFileName || l.FileName.Contains(fileName)) &&
                     (checkCreateTimeBegin || l.CreateTime >= createTimeBeginTime) &&
                     (checkCreateTimeEnd || l.CreateTime <= createTimeEndTime) &&
                     l.IsPublic;

            result.data = _currencyService.GetListPaged(pageIndex, pageSize, expression, out totalCount, new OrderModelField { PropertyName = sortColumn, IsDesc = isDesc });
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AdminAuthorize(PermissionsArray = new[] { Permissions.DeleteFileKey })]
        public ActionResult Delete(Guid fileId, int confirm)
        {
            var result = new DataJsonResult();

            if (confirm == 0)
            {
                var relationCount = _currencyService.Count<StorageFileRelation>(r => r.FileId.Equals(fileId));
                if (relationCount > 0)
                {
                    result.ErrorCode = "0001";
                    result.ErrorMessage = "此文件已经被使用";
                }
                else
                {
                    _fileService.DeleteFileById(fileId);
                }
            }
            else
            {
                _fileService.DeleteFileById(fileId);
            }

            return Json(result);
        }

        public ActionResult UploadPartial(string controlId, Guid? sourceId, string moduleKey, string sourceType, bool? isPublic, int? maxFiles, int? maxFilesize, string acceptedFiles, string defaultValue = "", int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, bool? autoProcessQueue = true, bool? editMode = true, FileType? fileType = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool? waterMark = false, UploadPartialType uploadType = UploadPartialType.Both)
        {
            Argument.ThrowIfNullOrEmpty(controlId, "controlId");
            if (maxFiles == null || maxFiles == 0) maxFiles = 1;
            if (maxFilesize == null || maxFilesize == 0) maxFilesize = 4;
            if (isPublic == null) isPublic = true;
            if (string.IsNullOrWhiteSpace(acceptedFiles)) acceptedFiles = ".bmp,.jpg,.jpeg,.gif,.png";
            if (autoProcessQueue == null) autoProcessQueue = true;

            ViewBag.ControlId = controlId;
            ViewBag.IsPublic = isPublic;
            ViewBag.MaxFiles = maxFiles;
            ViewBag.MaxFilesize = maxFilesize;
            ViewBag.AcceptedFiles = acceptedFiles;
            ViewBag.AutoProcessQueue = autoProcessQueue;
            ViewBag.DefaultValue = defaultValue;
            ViewBag.EditMode = editMode;
            ViewBag.FileType = fileType;
            ViewBag.WaterMark = waterMark;
            ViewBag.UploadType = uploadType;

            ViewBag.MediumThumbnailWidth = mediumThumbnailWidth;
            ViewBag.MediumThumbnailHeight = mediumThumbnailHeight;
            ViewBag.SmallThumbnailWidth = smallThumbnailWidth;
            ViewBag.SmallThumbnailHeight = smallThumbnailHeight;
            ViewBag.ThumbnailType = thumbnailType;

            if (sourceId != null)
            {
                var currentFiles = _storageFileService.GetFiles(sourceId.Value, moduleKey, sourceType);

                ViewBag.DefaultValue = string.Join(",", currentFiles.Distinct().Select(f => f.Id));
                ViewBag.CurrentFiles = currentFiles;
            }

            return PartialView("_PartialUpload");
        }

        public ActionResult SimpleUploadPartial(string controlId, string moduleKey, string sourceType, int? maxFilesize, string acceptedFiles, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, FileType? fileType = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool? waterMark = false, UploadPartialType uploadType = UploadPartialType.Both)
        {
            Argument.ThrowIfNullOrEmpty(controlId, "controlId");
            if (maxFilesize == null || maxFilesize == 0) maxFilesize = 4;
            if (string.IsNullOrWhiteSpace(acceptedFiles)) acceptedFiles = ".bmp,.jpg,.jpeg,.gif,.png";

            ViewBag.ControlId = controlId;
            ViewBag.IsPublic = true;
            ViewBag.MaxFiles = 1;
            ViewBag.MaxFilesize = maxFilesize;
            ViewBag.AcceptedFiles = acceptedFiles;
            ViewBag.AutoProcessQueue = true;
            ViewBag.FileType = fileType;
            ViewBag.WaterMark = waterMark;
            ViewBag.UploadType = uploadType;

            ViewBag.MediumThumbnailWidth = mediumThumbnailWidth;
            ViewBag.MediumThumbnailHeight = mediumThumbnailHeight;
            ViewBag.SmallThumbnailWidth = smallThumbnailWidth;
            ViewBag.SmallThumbnailHeight = smallThumbnailHeight;
            ViewBag.ThumbnailType = thumbnailType;

            return PartialView("_PartialSimpleUpload");
        }

        /// <summary>
        /// 供选择文件对话框中加载数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectOnPage(int pageIndex, int fileType = 0)
        {
            var result = new DataTableJsonResult();
            int totalCount;
            var notCheckFileType = fileType == -1;
            Expression<Func<StorageFile, bool>> expression =
               l => l.IsPublic
               && (notCheckFileType || (int)l.FileType == fileType);

            result.data = _currencyService.GetListPaged(pageIndex, 20, expression, out totalCount, new OrderModelField { PropertyName = "CreateTime", IsDesc = true });
            result.recordsTotal = totalCount;
            result.recordsFiltered = totalCount;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    /// <summary>
    /// 上传控件类型
    /// </summary>
    public enum UploadPartialType
    {
        /// <summary>
        /// 可以传可以选
        /// </summary>
        Both = 0,

        /// <summary>
        /// 只能传
        /// </summary>
        JustUpload = 1,

        /// <summary>
        /// 只能选
        /// </summary>
        JustSelect = 2
    }
}