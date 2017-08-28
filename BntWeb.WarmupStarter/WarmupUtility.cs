/* 
    ======================================================================== 
        File name：        WarmupUtility
        Module:                
        Author：            Luce
        Create Time：    2016/4/5 9:06:04
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace BntWeb.WarmupStarter
{
    public static class WarmupUtility
    {
        public static readonly string WarmupFilesPath = "~/App_Data/Warmup/";
        /// <summary>
        /// 请求被挂起（直到被调用）返回true，直接进入执行返回false
        /// </summary>
        /// <param name="httpApplication"></param>
        /// <returns></returns>
        public static bool DoBeginRequest(HttpApplication httpApplication)
        {
            // 使用客户端访问的地址
            // 真是的地址可能不同，如果客户端使用了代理的话。
            var url = ToUrlString(httpApplication.Request);
            var virtualFileCopy = WarmupUtility.EncodeUrl(url.Trim('/'));
            var localCopy = Path.Combine(HostingEnvironment.MapPath(WarmupFilesPath), virtualFileCopy);

            if (File.Exists(localCopy))
            {
                // 不缓存
                httpApplication.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                httpApplication.Response.Cache.SetValidUntilExpires(false);
                httpApplication.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                httpApplication.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                httpApplication.Response.Cache.SetNoStore();

                httpApplication.Response.WriteFile(localCopy);
                httpApplication.Response.End();
                return true;
            }

            // 存在静态缓存文件
            if (File.Exists(httpApplication.Request.PhysicalPath))
            {
                return true;
            }

            return false;
        }

        public static string ToUrlString(HttpRequest request)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Headers["Host"], request.RawUrl);
        }

        public static string EncodeUrl(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("url can't be empty");
            }

            var sb = new StringBuilder();
            foreach (var c in url.ToLowerInvariant())
            {
                // 只接受字母数字字符
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                }
                // 否则utf8编码
                else
                {
                    sb.Append("_");
                    foreach (var b in Encoding.UTF8.GetBytes(new[] { c }))
                    {
                        sb.Append(b.ToString("X"));
                    }
                }
            }

            return sb.ToString();
        }
    }
}
