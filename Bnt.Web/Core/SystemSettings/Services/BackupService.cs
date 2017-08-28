
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BntWeb.Core.SystemSettings.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Extensions;
using BntWeb.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using MySql.Data.MySqlClient;

namespace BntWeb.Core.SystemSettings.Services
{
    public class BackupService : IBackupService
    {
        private readonly ICurrencyService _currencyService;

        public bool IsBusy { get; set; }

        public BackupService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        private void BackupMysqlToSqlFile(string connectionString, string dataBaseDir)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string fileName = conn.Database + ".sql";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                MySqlCommand cmd = new MySqlCommand();
                MySqlBackup mb = new MySqlBackup(cmd);
                cmd.Connection = conn;
                mb.ExportToFile(dataBaseDir + "\\" + fileName);
                conn.Close();
            }
        }

        /// <summary>
        /// 备份MsSql数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dataBaseDir"></param>
        private void BackupMssqlToSqlFile(string connectionString, string dataBaseDir)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            Server server = new Server(new ServerConnection(conn));

            var databse = server.Databases[conn.Database];
            var scripter = new Scripter(server);
            scripter.Options.IncludeHeaders = true;
            var smoObjects = new Urn[1];
            //生成表数据
            foreach (Table t in databse.Tables)
            {
                smoObjects[0] = t.Urn;
                if (t.IsSystemObject == false)
                {
                    var sb = new StringBuilder();
                    //生成Drop语句
                    scripter.Options.ScriptDrops = true;
                    scripter.Options.WithDependencies = false;
                    scripter.Options.Indexes = false;
                    scripter.Options.ScriptSchema = true;
                    scripter.Options.ScriptData = false;
                    StringCollection sc = scripter.Script(smoObjects);
                    foreach (var st in sc)
                    {
                        sb.AppendLine(st);
                    }
                    sb.AppendLine("Go");

                    //生成Create语句，包括索引
                    scripter.Options.ScriptDrops = false;
                    scripter.Options.WithDependencies = true;
                    scripter.Options.Indexes = true;
                    sc = scripter.Script(smoObjects);
                    foreach (var st in sc)
                    {
                        sb.AppendLine(st);
                    }
                    sb.AppendLine("Go");

                    //导出数据为Insert语句
                    scripter.Options.ScriptDrops = false;
                    scripter.Options.WithDependencies = false;
                    scripter.Options.Indexes = false;
                    scripter.Options.ScriptSchema = false;
                    scripter.Options.ScriptData = true;
                    var sc2 = scripter.EnumScript(smoObjects);
                    foreach (var st in sc2)
                    {
                        sb.AppendLine(st);
                    }
                    sb.AppendLine("Go");

                    //单独保存为一个sql文件
                    string tablePath = dataBaseDir + "\\" + t.Name + ".sql";
                    FileStream fs = new FileStream(tablePath, FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(sb);//写你的字符串。
                    sw.Close();
                }
            }

            //生成视图数据
            foreach (View t in databse.Views)
            {
                smoObjects[0] = t.Urn;
                if (t.IsSystemObject == false)
                {
                    var sb = new StringBuilder();
                    //生成Drop语句
                    scripter.Options.ScriptDrops = true;
                    scripter.Options.WithDependencies = false;
                    scripter.Options.Indexes = false;
                    scripter.Options.ScriptSchema = true;
                    scripter.Options.ScriptData = false;
                    StringCollection sc = scripter.Script(smoObjects);
                    foreach (var st in sc)
                    {
                        sb.AppendLine(st);
                    }
                    sb.AppendLine("Go");

                    //生成Create语句，包括索引
                    scripter.Options.ScriptDrops = false;
                    scripter.Options.WithDependencies = false;
                    scripter.Options.Indexes = true;
                    sc = scripter.Script(smoObjects);
                    foreach (var st in sc)
                    {
                        sb.AppendLine(st);
                    }
                    sb.AppendLine("Go");

                    //单独保存为一个sql文件
                    string viewPath = dataBaseDir + "\\" + t.Name + ".sql";
                    FileStream fs = new FileStream(viewPath, FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(sb);//写你的字符串。
                    sw.Close();
                }
            }

        }

        public void Backup(BackupInfo backupInfo)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var dbconn = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();

                if (!string.IsNullOrWhiteSpace(dbconn))
                {
                    //备份数据库
                    string dataBaseDir = $"{AppDomain.CurrentDomain.BaseDirectory}/DbBackup\\{DateTime.Now:yyyyMMdd}\\";
                    if (Directory.Exists(dataBaseDir))
                    {
                        Directory.Delete(dataBaseDir, true);
                    }
                    Directory.CreateDirectory(dataBaseDir);
                    Logger.Debug("创建DataBase目录完成");

                    Logger.Debug("开始备份数据库");
                    //判断数据库类型
                    if (dbconn.ToLower().Contains("host="))
                    {
                        //MySql备份
                        BackupMysqlToSqlFile(dbconn, dataBaseDir);
                    }
                    else
                    {
                        //MsSql备份
                        BackupMssqlToSqlFile(dbconn, dataBaseDir);
                    }

                    //打包
                    Logger.Debug("开始打包...");
                    var absPath = $"DbBackup/{KeyGenerator.GetOrderNumber()}.zip";
                    var zipPath = $"{AppDomain.CurrentDomain.BaseDirectory}/{absPath}";
                    new ZipFileClass().Compress(dataBaseDir, zipPath);
                    Logger.Debug("打包完成");

                    Logger.Debug("清理临时文件");
                    Directory.Delete(dataBaseDir, true);

                    Logger.Debug($"备份完成：{zipPath}");
                    Logger.Debug("数据库备份完成");
                    backupInfo.Status = BackupStatus.Succeeded;
                    backupInfo.FilePath = absPath;
                }
                else
                {
                    backupInfo.Status = BackupStatus.Failed;
                    backupInfo.Message = "数据库连接不存在或有错误";
                }
            }
            catch (Exception e)
            {
                Logger.Error($"数据库备份失败：{e.Message}");
                backupInfo.Status =  BackupStatus.Failed;
                backupInfo.Message = $"数据库备份失败：{(e.Message.Length > 500 ? e.Message.Substring(0, 500) : e.Message)}";
            }
            finally
            {
                _currencyService.Update(backupInfo);
                IsBusy = false;
            }
        }
    }
}