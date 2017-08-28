/* 
    ======================================================================== 
        File name：        Starter
        Module:                
        Author：            Luce
        Create Time：    2016/4/5 9:06:33
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BntWeb.WarmupStarter
{
    public class Starter<T> where T : class
    {
        private readonly Func<HttpApplication, T> _initialization;
        private readonly Action<HttpApplication, T> _beginRequest;
        private readonly Action<HttpApplication, T> _endRequest;
        private readonly object _synLock = new object();
        /// <summary>
        /// 初始化队列工作项的结果。
        /// 设置只有在没有错误的情况下完成初始化。
        /// </summary>
        private volatile T _initializationResult;
        /// <summary>
        /// 初始化线程中发生的错误，这是一次性的错误信号，
        /// 所以可以保证在另一个请求过来的时候再次重新初始化
        /// </summary>
        private volatile Exception _error;
        /// <summary>
        /// 这是前一次初始化发生的错误，需要保留这个错误直到下一次初始化成功
        /// 对外输出的说这个错误内容，即上一次初始化出错的错误信息
        /// </summary>
        private volatile Exception _previousError;

        public Starter(Func<HttpApplication, T> initialization, Action<HttpApplication, T> beginRequest, Action<HttpApplication, T> endRequest)
        {
            _initialization = initialization;
            _beginRequest = beginRequest;
            _endRequest = endRequest;
        }

        public void OnApplicationStart(HttpApplication application)
        {
            LaunchStartupThread(application);
        }

        public void OnBeginRequest(HttpApplication application)
        {
            // 初始化出错
            if (_error != null)
            {
                // 保存這次错误，并重启异步初始化
                // 需要重新尝试初始化，因为环境配置有可能已经发生了改变
                // App_Data目录需要可读写的权限
                bool restartInitialization = false;

                lock (_synLock)
                {
                    if (_error != null)
                    {
                        _previousError = _error;
                        _error = null;
                        restartInitialization = true;
                    }
                }

                if (restartInitialization)
                {
                    LaunchStartupThread(application);
                }
            }

            // 抛出之前初始化的错误
            if (_previousError != null)
            {
                throw new ApplicationException("程序初始化发生错误", _previousError);
            }

            // 通知初始化成功
            if (_initializationResult != null)
            {
                _beginRequest(application, _initializationResult);
            }
        }

        public void OnEndRequest(HttpApplication application)
        {
            // 通知初始化成功
            if (_initializationResult != null)
            {
                _endRequest(application, _initializationResult);
            }
        }

        /// <summary>
        /// 在队列工作项目中异步运行初始化委托
        /// </summary>
        public void LaunchStartupThread(HttpApplication application)
        {
            // 保证进来的请求说有序的
            WarmupHttpModule.SignalWarmupStart();

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    try
                    {
                        var result = _initialization(application);
                        _initializationResult = result;
                    }
                    catch (Exception ex)
                    {
                        lock (_synLock)
                        {
                            _error = ex;
                            _previousError = null;
                        }
                    }
                    finally
                    {
                        // 当初始化结束时执行未处理的请求
                        WarmupHttpModule.SignalWarmupDone();
                    }
                });
        }
    }
}
