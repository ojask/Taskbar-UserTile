namespace UserTile
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Text;

    public class UserPic : UserControl
    {
        private AppWeather.Model.WeatherModel.RootObject data;
        private AppWeather.Model.LocationModel.RootObject LocationData;
        private IContainer components = null;
        private ContextMenuStrip contextMenu;
        private readonly PictureBox picture;
        private ToolStripMenuItem toolStripMenuItem1;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

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
        public void prepareWeatherToDisplay()
        {
            if (!File.Exists("cityName.txt"))
            {
                LocationData = AppWeather.Api.LocationApi.getCityCoords("London");
                string latitude = LocationData.results.latitude.ToString();
                string longitude = LocationData.results.longitude.ToString();
                data = AppWeather.Api.WeatherApi.getOneDayWeather(latitude, longitude);
            }
            else
            {
                string CityName = File.ReadAllText("cityName.txt");
                LocationData = AppWeather.Api.LocationApi.getCityCoords(CityName);
                string latitude = LocationData.results.latitude.ToString();
                string longitude = LocationData.results.longitude.ToString();
                data = AppWeather.Api.WeatherApi.getOneDayWeather(latitude, longitude);
            }

        }
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public void UpdateWeather()
        {
            if (IsConnectedToInternet())
            {
                var MyIni = new IniFile("Weather.ini");

                prepareWeatherToDisplay();
                if (data != null)
                {
                    if (!File.Exists("cityName.txt"))
                    {
                        MyIni.Write("City", "London");
                    }
                    else
                    {
                        string city = File.ReadAllText("cityName.txt");

                        MyIni.Write("City", city);
                    }
                    string wind = data.current_weather.windspeed.ToString();
                    string temp = data.current_weather.temperature.ToString();
                    string timeString = data.current_weather.time.ToString();
                    string WeatherCode = data.current_weather.weathercode.ToString();
                    int Last = timeString.LastIndexOf(timeString);
                    int First = timeString.IndexOf(timeString);
                    timeString = timeString.Remove(0, 11);
                    timeString = timeString.Remove(2, 3);
                    MyIni.Write("Wind", wind);
                    MyIni.Write("temp", temp);
                    MyIni.Write("time", timeString);
                    MyIni.Write("WCode", WeatherCode);
                }
                if (data == null)
                {
                    MyIni.Write("Wind", "");
                    MyIni.Write("temp", "Invalid City name");
                    MyIni.Write("time", "");
                    MyIni.Write("WCode", "404");
                }

            }
        }

        public UserPic()
        {
            this.InitializeComponent();
            Timer timer1 = new Timer();
            timer1.Interval = 1800000;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += Timer1_Tick;
            UpdateWeather();
            this.picture = new PictureBox();
            this.picture.Width = base.Width - 2;
            this.picture.Height = base.Height - 2;
            this.picture.Left = 1;
            this.picture.Top = 1;
            base.AutoScaleMode = AutoScaleMode.Font;
            this.picture.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picture.Parent = this;
            this.picture.MouseClick += new MouseEventHandler(this.PictureMouseClick);
            Timer timer = new Timer();
            timer.Interval = 1;
            timer.Enabled = true;
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateWeather();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.picture.Image = GetUserTile(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(104, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem1.Text = "Close";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // UserPic
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "UserPic";
            this.Load += new System.EventHandler(this.UserPic_Load);
            this.SizeChanged += new System.EventHandler(this.UserPicSizeChanged);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void PictureMouseClick(object sender, MouseEventArgs e)
        {
            UserTile.Flyout flyout = new Flyout();

            flyout.ShowInTaskbar = false;
            if (e.Button == MouseButtons.Left && !flyout.Visible)
            {
                flyout.Show();
            }
            else if (e.Button == MouseButtons.Left && flyout.Visible)
            {
                flyout.Close();
                flyout.Dispose();
            }
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Program.taskbarManager.Dispose();
            Application.Exit();
        }

        private void UserPicSizeChanged(object sender, EventArgs e)
        {
            this.picture.Width = base.Width - 2;
            this.picture.Height = base.Height - 2;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7b)
            {
                this.contextMenu.Left = Control.MousePosition.X;
                this.contextMenu.Show(this, Control.MousePosition);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void UserPic_Load(object sender, EventArgs e)
        {

        }
    }
}

