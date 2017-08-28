using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Validation;
using BntWeb.WebApi.Filters;
using BntWeb.WebApi.Models;

namespace BntWeb.Core.SystemSettings.ApiControllers
{
    public class FileController : BaseApiController
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [HttpPost]
        public async Task<ApiResult> PostFile(bool isPublic = true, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge)
        {
            var result = new ApiResult();
            var list = new List<SimplifiedStorageFile>();
            int allCount = 0;
            try
            {

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                allCount = provider.Contents.Count;
                foreach (var item in provider.Contents)
                {
                    var fileName = item.Headers.ContentDisposition.FileName.Trim('"');

                    var ms = item.ReadAsStreamAsync().Result;
                    StorageFile outStorageFile;
                    _fileService.SaveFile(ms, fileName, isPublic, out outStorageFile, mediumThumbnailWidth, mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType);

                    if (outStorageFile != null)
                        list.Add(outStorageFile.Simplified());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "文件上传失败");
                if (list.Count == 0)
                {
                    result.returnCode = "0000";
                    result.msg = "文件上传出错";
                }
                else if (list.Count > 0 && list.Count < allCount)
                {
                    result.returnCode = "0001";
                    result.msg = "部分文件上传出错";
                }
            }

            result.SetData(list);
            return result;
        }


        [HttpPost]
        //[BasicAuthentication]
        public ApiResult SaveUploadedFile(bool isPublic = true, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge)
        {
            var result = new ApiResult();
            var list = new List<SimplifiedStorageFile>();
            int allCount = 0;
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;
                allCount = files.Count;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    if (file.ContentLength > 0 && CheckIsAllowedExtension(file))
                    {
                        var fileName = file.FileName.Trim('"');
                        StorageFile outStorageFile;
                        _fileService.SaveFile(file.InputStream, fileName, isPublic, out outStorageFile, mediumThumbnailWidth, mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType);
                        if (outStorageFile != null)
                            list.Add(outStorageFile.Simplified());
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "文件上传失败");
            }
            if (list.Count == 0)
                throw new WebApiInnerException("0000", "文件上传出错");
            else if (list.Count > 0 && list.Count < allCount)
                throw new WebApiInnerException("0001", "部分文件上传出错");

            result.SetData(list);
            return result;
        }


        private bool CheckIsAllowedExtension(HttpPostedFile file)
        {
            String fileExt = Path.GetExtension(file.FileName).ToLower();
            //int maxSize = 5 * 1024 * 1024;

            //if (file.InputStream.Length > maxSize)
            //{
            //    return false;
            //}

            Hashtable extTable = new Hashtable
            {
                {"image", "gif,jpg,jpeg,png,bmp"},
                {"flash", "swf,flv"},
                {"media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb"},
                {"file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2,pdf"}
            };
            if (!String.IsNullOrEmpty(fileExt) &&((Array.IndexOf(((String)extTable["image"]).Split(','), fileExt.Substring(1).ToLower()) > -1 ||
                 Array.IndexOf(((String)extTable["file"]).Split(','), fileExt.Substring(1).ToLower()) > -1)))
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedExtension(HttpPostedFile file)
        {

            BinaryReader r = new BinaryReader(file.InputStream);

            string fileclass = "";
            byte buffer;
            buffer = r.ReadByte();
            fileclass = buffer.ToString();
            buffer = r.ReadByte();
            fileclass += buffer.ToString();


            /*文件扩展名说明
            * 255216 jpg
            * 208207 doc xls ppt wps
            * 8075 docx pptx xlsx zip
            * 5150 txt
            * 8297 rar
            * 7790 exe
            * 3780 pdf      
            * 
            * 4946/104116 txt
            * 7173        gif 
            * 255216      jpg
            * 13780       png
            * 6677        bmp
            * 239187      txt,aspx,asp,sql
            * 208207      xls.doc.ppt
            * 6063        xml
            * 6033        htm,html
            * 4742        js
            * 8075        xlsx,zip,pptx,mmap,zip
            * 8297        rar   
            * 01          accdb,mdb
            * 7790        exe,dll
            * 5666        psd 
            * 255254      rdp 
            * 10056       bt种子 
            * 64101       bat 
            * 4059        sgf    
            */

            //添加允许的文件类型
            Dictionary<String, String> ftype = new Dictionary<string, string>
            {
                {"7173", "gif"},
                {"255216", "jpg"},
                {"13780", "png"},
                {"6677", "bmp"},
                {"208207", "doc xls ppt wps"},
                {"8075", "docx pptx xlsx zip"},
                {"8297","rar" }
            };

            if (ftype.ContainsKey(fileclass))
                return true;

            return false;
        }
    }
}
