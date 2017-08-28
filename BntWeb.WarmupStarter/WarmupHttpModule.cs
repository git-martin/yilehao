/* 
    ======================================================================== 
        File name：        WarmupHttpModule
        Module:                
        Author：            Luce
        Create Time：    2016/4/5 9:06:20
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
    public class WarmupHttpModule : IHttpModule
    {
        private HttpApplication _context;
        private static object _synLock = new object();
        private static IList<Action> _awaiting = new List<Action>();

        public void Init(HttpApplication context)
        {
            _context = context;
            context.AddOnBeginRequestAsync(BeginBeginRequest, EndBeginRequest, null);
        }

        public void Dispose() { }

        private static bool InWarmup()
        {
            lock (_synLock)
            {
                return _awaiting != null;
            }
        }

        /// <summary>
        /// 程序启动中，新来的请求都放入队列，直到“SignalWarmupDone”被调用
        /// </summary>
        public static void SignalWarmupStart()
        {
            lock (_synLock)
            {
                if (_awaiting == null)
                {
                    _awaiting = new List<Action>();
                }
            }
        }

        /// <summary>
        /// 程序启动完成，所有等待中的请求开始执行，新来的请求将会被执行
        /// </summary>
        public static void SignalWarmupDone()
        {
            IList<Action> temp;

            lock (_synLock)
            {
                temp = _awaiting;
                _awaiting = null;
            }

            if (temp != null)
            {
                foreach (var action in temp)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// 根据当前状态判断是放入队列还是直接执行
        /// </summary>
        private void Await(Action action)
        {
            Action temp = action;

            lock (_synLock)
            {
                if (_awaiting != null)
                {
                    temp = null;
                    _awaiting.Add(action);
                }
            }

            if (temp != null)
            {
                temp();
            }
        }

        private IAsyncResult BeginBeginRequest(object sender, EventArgs e, AsyncCallback cb, object extradata)
        {
            // 已经启动，处理请求
            if (!InWarmup() || WarmupUtility.DoBeginRequest(_context))
            {
                var asyncResult = new DoneAsyncResult(extradata);
                cb(asyncResult);
                return asyncResult;
            }
            else
            {
                // 挂起状态下的执行路径
                var asyncResult = new WarmupAsyncResult(cb, extradata);
                Await(asyncResult.Completed);
                return asyncResult;
            }
        }

        private static void EndBeginRequest(IAsyncResult ar)
        {
        }

        /// <summary>
        /// 挂起状态下的异步执行结果，挂起的请求会在程序启动后执行
        /// </summary>
        private class WarmupAsyncResult : IAsyncResult
        {
            private readonly EventWaitHandle _eventWaitHandle = new AutoResetEvent(false/*初始化的状态*/);
            private readonly AsyncCallback _cb;
            private readonly object _asyncState;
            private bool _isCompleted;

            public WarmupAsyncResult(AsyncCallback cb, object asyncState)
            {
                _cb = cb;
                _asyncState = asyncState;
                _isCompleted = false;
            }

            public void Completed()
            {
                _isCompleted = true;
                _eventWaitHandle.Set();
                _cb(this);
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return _isCompleted; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return _eventWaitHandle; }
            }
        }

        /// <summary>
        /// 处理完成后的请求结果
        /// </summary>
        private class DoneAsyncResult : IAsyncResult
        {
            private readonly object _asyncState;
            private static readonly WaitHandle _waitHandle = new ManualResetEvent(true/*initialState*/);

            public DoneAsyncResult(object asyncState)
            {
                _asyncState = asyncState;
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return true; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return true; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return _waitHandle; }
            }

            object IAsyncResult.AsyncState
            {
                get { return _asyncState; }
            }
        }
    }
}
