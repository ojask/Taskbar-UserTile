using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace UserTile
{

    public partial class Flyout : Form
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        [DllImport("Shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username, uint whatever, StringBuilder picpath, int maxLength);

        private AppWeather.Model.WeatherModel.RootObject data;
        private AppWeather.Model.LocationModel.RootObject LocationData;

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
        public void DisplayWeather()
        {
            if (!IsConnectedToInternet())
            {
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                panel3.Visible = false;
                label5.Visible = false;
            }
            else if (IsConnectedToInternet())
            {
                var MyIni = new IniFile("Weather.ini");
                label12.Text = MyIni.Read("temp") + " °C";
                float TempFloat = float.Parse(MyIni.Read("temp"));
                label13.Text = MyIni.Read("City");
                string WCode = MyIni.Read("WCode");
                int WeatherCode = Int32.Parse(WCode);
                label11.Text = MyIni.Read("Wind") + " Km/h winds";
                if (!File.Exists("cityName.txt"))
                {
                    label13.Text = "London";
                }
                else
                {
                    string CityName = File.ReadAllText("cityName.txt");
                    label13.Text = CityName;
                }
                int now = Int32.Parse(MyIni.Read("time"));
                int start = 19;
                int end = 6;

                if (WeatherCode == 0)
                {
                    label10.Text = "Clear sky";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._0n;
                        }
                        else
                        {

                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._0;

                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._0n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._0;
                        }
                    }
                }
                if (WeatherCode == 1)
                {
                    label10.Text = "Mainly clear";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._1n;
                        }
                        else
                        {

                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._1;

                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._1n;
                        }
                        else
                        {

                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._1;

                        }
                    }
                }
                if (WeatherCode == 2)
                {
                    label10.Text = "Partly cloudy";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._2n;
                        }
                        else
                        {
                            if (TempFloat > 40)
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources.hotvery;
                            }
                            else if (TempFloat < 40 && TempFloat > -4)
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._2;
                            }
                            else
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._75;
                            }
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._2n;
                        }
                        else
                        {
                            if (TempFloat > 40)
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources.hotvery;
                            }
                            else if (TempFloat < 40 && TempFloat > -4)
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._2;
                            }
                            else
                            {
                                panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._75;
                            }
                        }
                    }
                }
                if (WeatherCode == 3)
                {
                    label10.Text = "Overcast";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._3;
                }
                if (WeatherCode == 45)
                {
                    label10.Text = "Fog";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._45;
                }
                if (WeatherCode == 48)
                {
                    label10.Text = "Depositing rime fog";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._48;
                }
                if (WeatherCode == 51)
                {
                    label10.Text = "Light drizzle";

                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                }
                if (WeatherCode == 53)
                {
                    label10.Text = "Moderate drizzle";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._53;
                }
                if (WeatherCode == 55)
                {
                    label10.Text = "Heavy drizzle";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._55;
                }
                if (WeatherCode == 56)
                {
                    label10.Text = "SLight freezing drizzle";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._56;
                }
                if (WeatherCode == 57)
                {
                    label10.Text = "Heavy freezing drizzle";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._57;
                }
                if (WeatherCode == 61)
                {
                    label10.Text = "Slight rain";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                }
                if (WeatherCode == 63)
                {
                    label10.Text = "Moderate rain";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._63;
                }
                if (WeatherCode == 65)
                {
                    label10.Text = "Heavy rain";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._65;
                }
                if (WeatherCode == 66)
                {
                    label10.Text = "Slight freezing rain";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                }
                if (WeatherCode == 67)
                {
                    label10.Text = "Heavy freezing rain";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._63;
                }
                if (WeatherCode == 71)
                {
                    label10.Text = "Slight snow";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._71;
                }
                if (WeatherCode == 73)
                {
                    label10.Text = "Moderate snow";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._73;
                }
                if (WeatherCode == 75)
                {
                    label10.Text = "Heavy snow";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._75;
                }
                if (WeatherCode == 77)
                {
                    label10.Text = "Snow grains";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._77;
                }
                if (WeatherCode == 80)
                {
                    label10.Text = "Slight rain showers";
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51n;
                        }
                        else
                        {
                            panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._51;
                        }
                    }
                }
                if (WeatherCode == 81)
                {
                    label10.Text = "Moderate rain showers";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._81;
                }
                if (WeatherCode == 82)
                {
                    label10.Text = "Heavy rain showers";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._82;
                }
                if (WeatherCode == 85)
                {
                    label10.Text = "Slight snow showers";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._85;
                }
                if (WeatherCode == 86)
                {
                    label10.Text = "Heavy snow showers";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._86;
                }
                if (WeatherCode == 95)
                {
                    label10.Text = "Thunderstorm";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._95;
                }
                if (WeatherCode == 96)
                {
                    label10.Text = "Moderate hailstorm";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._96;
                }
                if (WeatherCode == 99)
                {
                    label10.Text = "Heavy hailstorm";
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._99;
                }
                if (WeatherCode == 404)
                {
                    label10.Visible = false;
                    label11.Visible = false;
                    label12.Text = "Invalid city name";
                    label13.Visible = false;
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources.na;
                }
                if (TempFloat > 40)
                {
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources.hotvery;
                }
                if (TempFloat <= -10)
                {
                    panel3.BackgroundImage = global::UserTaskerTile.Properties.Resources._75;
                }

            }
        }

        private void updateWeather()
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
            DisplayWeather();


        }
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
        private void Flyout_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            DisplayWeather();
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
            UserTaskerTile.UserTile.Location location = new UserTaskerTile.UserTile.Location();
            location.ShowInTaskbar = false;
            location.Show();
            location.FormClosed += Location_FormClosed;
        }

        private void Location_FormClosed(object sender, FormClosedEventArgs e)
        {
            updateWeather();
            DisplayWeather();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
