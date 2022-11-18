namespace UserTile
{
    using Nini.Config;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using UserTileLib;
    using Microsoft.Win32;
    using System.Diagnostics;

    internal class Program
    {
        public static string AvatarPath;
        private static IConfigSource config;
        public static WinAPI.RECT defaultTrayRect;
        private static IntPtr langbarHwnd;
        private static IntPtr showDesktopButtonHwnd;
        public static WinAPI.WINDOWPLACEMENT showDesktopDefaultPlacement;
        public static IntPtr taskbarHwnd;
        public static TaskbarManager taskbarManager;
        private static System.Windows.Forms.Timer timer;
        public static IntPtr trayHwnd;
        private static UserPic userPic;

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log(e.Exception);
        }

        public static void Log(Exception e)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(directoryName + @"\log.txt"))
            {
                File.WriteAllText(directoryName + @"\log.txt", string.Empty);
            }
            try
            {
                File.AppendAllText(directoryName + @"\log.txt", string.Concat(new object[] { DateTime.Now, " -------------- ", '\r', '\n', "OS: ", Environment.OSVersion.VersionString, '\r', '\n', e.ToString(), '\r', '\n' }));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't write log. " + exception.Message);
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            taskbarManager = new TaskbarManager();
            userPic = new UserPic();
            if (!taskbarManager.IsTaskbarSmall())
            {
                userPic.Width = 0x25;
                userPic.Height = 0x24;
                taskbarManager.ReserveSpace(0x25);
            }
            else
            {
                userPic.Width = 0x1b;
                userPic.Height = 0x1a;
                taskbarManager.ReserveSpace(0x1b);
            }
            userPic.Top = 3;
            taskbarManager.AddControl(userPic);
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(Program.TimerTick);
            timer.Enabled = true;
            timer.Start();
            Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
            Application.Run();
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            taskbarManager.CheckTaskbar();
            if (!taskbarManager.CheckTrayWidth())
            {
                if (!taskbarManager.IsTaskbarSmall())
                {
                    userPic.Width = 0x25;
                    userPic.Height = 0x24;
                    taskbarManager.ReserveSpace(0x25);
                }
                else
                {
                    userPic.Width = 0x1b;
                    userPic.Height = 0x1a;
                    taskbarManager.ReserveSpace(0x1b);
                }
                userPic.Top = 3;
                taskbarManager.AddControl(userPic);
            }
            taskbarManager.DetectTaskbarPos();
        }
    }
}

