namespace UserTile
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    //Deprecated

    public class PopupWindow : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal Image Avatar;
        internal TextBlock LockButton;
        internal TextBlock LogOffButton;
        internal TextBlock MyLookButton;
        internal TextBlock MySettingsButton;
        internal MediaElement Player;
        internal TextBlock SwitchUserButton;
        internal TextBlock Username;

        public PopupWindow()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/UserTile;component/popupwindow.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        private void LockButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
        }

        private void LogOffButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("shutdown.exe", "/l");
        }

        private void MyLookButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("control.exe", "userpasswords");
        }

        private void MySettingsButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("control.exe");
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Player.Position = TimeSpan.FromMilliseconds(0.0);
            this.Player.Play();
        }

        private void SwitchUserButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("tsdiscon.exe");
            }
            catch (Exception exception)
            {
                Program.Log(exception);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((PopupWindow) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
                    ((PopupWindow) target).Deactivated += new EventHandler(this.Window_Deactivated);
                    ((PopupWindow) target).SourceInitialized += new EventHandler(this.Window_SourceInitialized);
                    ((PopupWindow) target).Closed += new EventHandler(this.Window_Closed);
                    break;

                case 2:
                    this.Username = (TextBlock) target;
                    break;

                case 3:
                    this.SwitchUserButton = (TextBlock) target;
                    this.SwitchUserButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.SwitchUserButtonMouseLeftButtonUp);
                    break;

                case 4:
                    this.LogOffButton = (TextBlock) target;
                    this.LogOffButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.LogOffButtonMouseLeftButtonUp);
                    break;

                case 5:
                    this.LockButton = (TextBlock) target;
                    this.LockButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.LockButtonMouseLeftButtonUp);
                    break;

                case 6:
                    this.MySettingsButton = (TextBlock) target;
                    this.MySettingsButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.MySettingsButtonMouseLeftButtonUp);
                    break;

                case 7:
                    this.MyLookButton = (TextBlock) target;
                    this.MyLookButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.MyLookButtonMouseLeftButtonUp);
                    break;

                case 8:
                    this.Avatar = (Image) target;
                    break;

                case 9:
                    this.Player = (MediaElement) target;
                    this.Player.MediaEnded += new RoutedEventHandler(this.Player_MediaEnded);
                    break;

                default:
                    this._contentLoaded = true;
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Player.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            base.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            base.Top = (SystemInformation.WorkingArea.Bottom - base.Height) - 5.0;
            base.Left = (SystemInformation.WorkingArea.Right - base.Width) - 10.0;
            if (!string.IsNullOrEmpty(Program.AvatarPath) && File.Exists(Program.AvatarPath))
            {
                if (Program.AvatarPath.EndsWith(".wmv"))
                {
                }
                else
                {
                    this.Avatar.Source = new BitmapImage(new Uri(Program.AvatarPath, UriKind.RelativeOrAbsolute));
                }
            }
            else if (File.Exists(Path.GetTempPath() + @"\" + Environment.UserName + ".bmp"))
            {
                this.Avatar.Source = new BitmapImage(new Uri(Path.GetTempPath() + @"\" + Environment.UserName + ".bmp"));
            }
            else
            {
                this.Avatar.Source = new BitmapImage(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\userpic.png"));
            }
            this.Username.Text = Environment.UserName;
        }
    }
}

