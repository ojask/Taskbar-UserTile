using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.IO;
using System.Reflection;
using System.Xml;
using System.ServiceModel.Syndication;

namespace UserTile
{

    public partial class Flyout : Form
    {

            [DllImport("Shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, uint whatever, StringBuilder picpath, int maxLength);
        public string GetUserTilePath(string username)
        {
            StringBuilder sb;
            sb = new StringBuilder(1000);
            GetUserTilePath(username, 2147483648U, sb, sb.Capacity);
            return sb.ToString();
        }
        public Image GetUserTile(string username)
        {
            return Image.FromFile(GetUserTilePath(username));
        }
        public List<Rectangle> FindDockedTaskBars()
        {
            List<Rectangle> dockedRects = new List<Rectangle>();
            foreach (var tmpScrn in Screen.AllScreens)
            {
                if (!tmpScrn.Bounds.Equals(tmpScrn.WorkingArea))
                {
                    Rectangle rect = new Rectangle();

                    var leftDockedWidth = Math.Abs((Math.Abs(tmpScrn.Bounds.Left) - Math.Abs(tmpScrn.WorkingArea.Left)));
                    var topDockedHeight = Math.Abs((Math.Abs(tmpScrn.Bounds.Top) - Math.Abs(tmpScrn.WorkingArea.Top)));
                    var rightDockedWidth = ((tmpScrn.Bounds.Width - leftDockedWidth) - tmpScrn.WorkingArea.Width);
                    var bottomDockedHeight = ((tmpScrn.Bounds.Height - topDockedHeight) - tmpScrn.WorkingArea.Height);
                    if ((leftDockedWidth > 0))
                    {
                        rect.X = tmpScrn.Bounds.Left;
                        rect.Y = tmpScrn.Bounds.Top;
                        rect.Width = leftDockedWidth;
                        rect.Height = tmpScrn.Bounds.Height;
                        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                        this.Top = screenHeight - 347;
                        this.Left = rect.Width + 10;
                        this.Opacity = 100;

                    }
                    else if ((rightDockedWidth > 0))
                    {
                        rect.X = tmpScrn.WorkingArea.Right;
                        rect.Y = tmpScrn.Bounds.Top;
                        rect.Width = rightDockedWidth;
                        rect.Height = tmpScrn.Bounds.Height;
                        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                        this.Top = screenHeight - 347;
                        this.Left = (screenWidth - rect.Width) - 320;
                        this.Opacity = 100;
                    }
                    else if ((topDockedHeight > 0))
                    {
                        rect.X = tmpScrn.WorkingArea.Left;
                        rect.Y = tmpScrn.Bounds.Top;
                        rect.Width = tmpScrn.WorkingArea.Width;
                        rect.Height = topDockedHeight;
                        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                        this.Top = rect.Height + 10;
                        this.Left = screenWidth - 330;
                        this.Opacity = 100;
                    }
                    else if ((bottomDockedHeight > 0))
                    {
                        rect.X = tmpScrn.WorkingArea.Left;
                        rect.Y = tmpScrn.WorkingArea.Bottom;
                        rect.Width = tmpScrn.WorkingArea.Width;
                        rect.Height = bottomDockedHeight;
                        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                        this.Top = (screenHeight - rect.Height) - 347;
                        this.Left = screenWidth - 330;
                        this.Opacity = 100;
                    }
                    else
                    {
                    }

                    dockedRects.Add(rect);
                }
            }

            if (dockedRects.Count == 0)
            {
            }

            return dockedRects;
        }
        public Flyout()
        {
            InitializeComponent();
            label3.Text = Environment.MachineName;
            label2.Text = Environment.UserName;
            label4.Text = DateTime.Now.ToString("hh:mm tt") + ", " + DateTime.Now.ToString("MMM dd"); 
            Timer timer = new Timer();
            timer.Interval = 1;
            timer.Enabled = true;
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);
        }
            public void timer_Tick(object sender, EventArgs e)
        {
            FindDockedTaskBars();
            if (this.Focused == false)
            {
                this.Visible = false;
                this.Close();
            }
            if (Environment.OSVersion.Version.Build < 9888)
            {
                pictureBox1.Image = GetUserTile(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
            else if (Environment.OSVersion.Version.Build >= 9888)
            {
                pictureBox1.Visible = false;
            }
            

        }

        private void Flyout_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    return;
            }

            base.WndProc(ref m);
        }

        private void label5_hover(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font, FontStyle.Underline);
        }

        private void label5_revert(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font, FontStyle.Regular);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Process.Start("tsdiscon.exe");
        }

        private void SU_hov(object sender, EventArgs e)
        {
            label6.Font = new Font(label6.Font, FontStyle.Underline);
        }

        private void SU_Leav(object sender, EventArgs e)
        {
            label6.Font = new Font(label6.Font, FontStyle.Regular);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown.exe", "/l");
        }

        private void L_Hov(object sender, EventArgs e)
        {
            label7.Font = new Font(label7.Font, FontStyle.Underline);
        }

        private void L_Leav(object sender, EventArgs e)
        {
            label7.Font = new Font(label7.Font, FontStyle.Regular);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
        }

        private void Lo_Ent(object sender, EventArgs e)
        {
            label8.Font = new Font(label8.Font, FontStyle.Underline);
        }

        private void Le_Leav(object sender, EventArgs e)
        {
            label8.Font = new Font(label8.Font, FontStyle.Regular);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            Process process = new Process();
            process.StartInfo.FileName = windir + @"\explorer.exe";
            process.StartInfo.Arguments = "shell:::{60632754-c523-4b62-b45c-4172da012619}";
            process.Start();
        }

        private void sett_ent(object sender, EventArgs e)
        {
            label9.Font = new Font(label9.Font, FontStyle.Underline);
        }

        private void sett_leav(object sender, EventArgs e)
        {
            label9.Font = new Font(label9.Font, FontStyle.Regular);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            Process process = new Process();
            process.StartInfo.FileName = windir + @"\Systemm32\shutdown.exe";
            process.StartInfo.Arguments = "/r /f /t 0";
            process.Start();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    
}
