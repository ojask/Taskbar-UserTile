namespace UserTile
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using UserTileLib;

    internal class Program
    {

        public static string AvatarPath;
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

    class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}


