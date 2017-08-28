/* 
    ======================================================================== 
        File name：        FileService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/12 16:17:12
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using BntWeb.Config.Models;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Extensions;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Services;
using BntWeb.Utility.Extensions;
using BntWeb.Validation;

namespace BntWeb.Core.SystemSettings.Services
{
    public class FileService : IFileService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IConfigService _configService;

        public FileService(ICurrencyService currencyService, IConfigService configService)
        {
            _currencyService = currencyService;
            _configService = configService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 中等缩略图宽度
        /// </summary>
        public int MediumThumbnailWidth { get; set; }

        /// <summary>
        /// 中等缩略图高度
        /// </summary>
        public int MediumThumbnailHeight { get; set; }

        /// <summary>
        /// 小缩略图宽度
        /// </summary>
        public int SmallThumbnailWidth { get; set; }

        /// <summary>
        /// 小缩略图高度
        /// </summary>
        public int SmallThumbnailHeight { get; set; }

        public bool SaveFile(HttpPostedFileBase file, bool isPublic, out StorageFile outStorageFile, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = false, object extensionConfig = null)
        {
            return SaveFile(file.InputStream, file.FileName, isPublic, out outStorageFile, mediumThumbnailWidth,
                mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType, waterMark, extensionConfig);
        }

        public bool SaveFile(string base64String, string fileName, bool isPublic, out StorageFile outStorageFile, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = false, object extensionConfig = null)
        {
            string[] imageArrty = base64String.Split(',');
            string imgBase64Data = imageArrty[1];
            string base64 = imgBase64Data;
            byte[] bytes = Convert.FromBase64String(base64);
            var fileStream = new MemoryStream(bytes);

            return SaveFile(fileStream, fileName, isPublic, out outStorageFile, mediumThumbnailWidth,
                mediumThumbnailHeight, smallThumbnailWidth, smallThumbnailHeight, thumbnailType, waterMark, extensionConfig);
        }

        public bool SaveFile(Stream fileStream, string fileName, bool isPublic, out StorageFile outStorageFile, int? mediumThumbnailWidth = null, int? mediumThumbnailHeight = null, int? smallThumbnailWidth = null, int? smallThumbnailHeight = null, ThumbnailType thumbnailType = ThumbnailType.WhiteEdge, bool waterMark = false, object extensionConfig = null)
        {
            Argument.ThrowIfNullOrEmpty(fileName, "文件名");
            outStorageFile = null;
            try
            {
                var fileSize = fileStream.Length;
                if (fileSize > 0)
                {
                    var fileId = KeyGenerator.GetGuidKey();
                    string relativePath;
                    var savePath = GetFolderPath(out relativePath);
                    relativePath = relativePath.Replace("\\", "/");
                    var fileExtension = Path.GetExtension(fileName)?.ToLower();

                    var fileType = fileName.GetFileType();
                    int imageWidth = 0, imageHeight = 0;

                    //保存到文件
                    var path = $"{savePath}\\{fileId}{fileExtension}";
                    if (fileType == FileType.Image)
                    {
                        //取长宽
                        System.Drawing.Image tempimage = System.Drawing.Image.FromStream(fileStream, true);
                        imageWidth = tempimage.Width;//宽
                        imageHeight = tempimage.Height;//高

                        tempimage.Save(path);
                    }
                    else
                    {
                        fileStream.Position = 0;
                        StreamWriter sw = new StreamWriter(path);
                        fileStream.CopyTo(sw.BaseStream);
                        sw.Flush();
                        sw.Close();
                        fileStream.Dispose();
                    }

                    var storageFile = new StorageFile
                    {
                        Id = fileId,
                        FileName = fileName,
                        FileExtension = fileExtension,
                        FileSize = fileSize.Kb(),
                        RelativePath = $"{relativePath}/{fileId}{fileExtension}",
                        CreateTime = DateTime.Now,
                        FileType = fileType,
                        Width = imageWidth,
                        Height = imageHeight,
                        IsPublic = isPublic,
                        ExtensionConfig = extensionConfig?.ToJson()
                    };

                    //生成缩略图，添加水印
                    if (fileType == FileType.Image)
                    {
                        WaterMark waterMarkConfig = null;
                        //添加水印
                        if (waterMark)
                        {
                            waterMarkConfig = _configService.Get<SystemConfig>().WaterMark;
                        }

                        mediumThumbnailWidth = mediumThumbnailWidth ?? MediumThumbnailWidth;
                        mediumThumbnailHeight = mediumThumbnailHeight ?? MediumThumbnailHeight;
                        storageFile.MediumThumbnail = storageFile.RelativePath.GetThumbnail(mediumThumbnailWidth.Value, mediumThumbnailHeight.Value, thumbnailType, waterMarkConfig);

                        smallThumbnailWidth = smallThumbnailWidth ?? SmallThumbnailWidth;
                        smallThumbnailHeight = smallThumbnailHeight ?? SmallThumbnailHeight;
                        storageFile.SmallThumbnail = storageFile.RelativePath.GetThumbnail(smallThumbnailWidth.Value, smallThumbnailHeight.Value, thumbnailType, waterMarkConfig);

                        storageFile.RelativePath.BuildWaterMark(waterMarkConfig);
                    }

                    var success = _currencyService.Create(storageFile);
                    if (success)
                    {
                        outStorageFile = storageFile;
                        Logger.Operation($"添加文件：{fileName}", SystemSettingsModule.Instance);
                    }
                    return success;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "上传文件出错");
                return false;
            }
        }

        public bool DeleteFileById(Guid fileId)
        {
            _currencyService.DeleteByConditon<StorageFileRelation>(f => f.FileId.Equals(fileId));
            var file = _currencyService.GetSingleByConditon<StorageFile>(f => f.Id.Equals(fileId));
            if (file != null)
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "/" + file.RelativePath;
                if (File.Exists(filePath))
                    File.Delete(filePath);

                var success = _currencyService.DeleteByConditon<StorageFile>(f => f.Id.Equals(fileId)) > 0;
                if (success)
                {
                    Logger.Operation($"删除文件：{file.FileName}", SystemSettingsModule.Instance);
                }
                return success;
            }
            return false;
        }

        private string GetFolderPath(out string relativePath)
        {
            relativePath = $"StorageFiles\\{DateTime.Now.Year}\\{DateTime.Now.Month}\\{DateTime.Now.Day}";
            string path =
                $"{AppDomain.CurrentDomain.BaseDirectory}/{relativePath}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public int DeleteFilesForTask(DateTime outTime)
        {
            var count = 0;
            List<StorageFile> files;
            using (var dbContext = new SystemSettingsDbContext())
            {
                //取出超期未使用的私有文件，并删除
                var query = from f in dbContext.StorageFiles
                            where dbContext.StorageFileRelations.All(me => me.FileId != f.Id) && f.IsPublic == false && f.CreateTime < outTime
                            select f;

                //sql语句写法
                //var sql = "select f.* from bnt_system_files f where not EXISTS(select FileId from bnt_system_file_relations r where r.FileId = f.Id) and f.IsPublic=0";
                //files = dbContext.Database.SqlQuery<StorageFile>(sql).ToList();

                files = query.ToList();
                if (files.Count > 0)
                {
                    //循环删除本地文件
                    foreach (var file in files)
                    {
                        var filePath = AppDomain.CurrentDomain.BaseDirectory + "/" + file.RelativePath;
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        Logger.Operation($"删除文件：{file.FileName}", SystemSettingsModule.Instance);

                        count++;
                    }

                    //一次性批量删除无效的file数据， 提升性能
                    var sqlDelete = @"delete f from bnt_system_files f where not EXISTS(select FileId from bnt_system_file_relations r where r.FileId = f.Id) 
and f.IsPublic=0 and f.CreateTime<'"+ outTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    dbContext.Database.ExecuteSqlCommand(sqlDelete);
                }
            }

            return count;
        }
    }
}