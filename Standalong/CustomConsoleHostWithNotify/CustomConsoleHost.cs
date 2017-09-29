using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Topshelf;
using Topshelf.Runtime;

namespace Topshelf.Extension
{
    public class CustomConsoleHost : Host, HostControl
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        IntPtr hWnd;
        bool isConsoleHiden;
        NotifyIcon notifyIcon;

        readonly HostEnvironment environment;
        readonly ServiceHandle serviceHandle;
        readonly HostSettings settings;

        public CustomConsoleHost(HostSettings settings, HostEnvironment environment, ServiceHandle serviceHandle)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (environment == null)
                throw new ArgumentNullException("environment");

            this.settings = settings;
            this.environment = environment;
            this.serviceHandle = serviceHandle;
        }

        #region Host Members

        public TopshelfExitCode Run()
        {
            hWnd = FindWindow(null, Console.Title);

            notifyIcon = new NotifyIcon();

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            notifyIcon.Visible = true;

            notifyIcon.MouseDoubleClick += icon_MouseDoubleClick;

            ShowWindow(hWnd, 0);

            isConsoleHiden = true;

            SetConsoleCtrlHandler(Handler, true);

            Console.WriteLine("Start services...");

            if (!serviceHandle.Start(this))
                Console.WriteLine("Services failed to start.");
            else
                Console.WriteLine("Close the window to stop the services.");

            Application.Run();

            return TopshelfExitCode.Ok;
        }

        #endregion
        void icon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isConsoleHiden)
            {
                ShowWindow(hWnd, 1);

                isConsoleHiden = false;
            }
            else
            {
                ShowWindow(hWnd, 0);

                isConsoleHiden = true;
            }
        }

        bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    notifyIcon.Visible = false;
                    return true;
                default:
                    return true;
            }
        }

        #region HostControl Members

        void HostControl.RequestAdditionalTime(TimeSpan timeRemaining)
        {

        }

        void HostControl.Restart()
        {

        }

        void HostControl.Stop()
        {

        }

        #endregion
    }
}
