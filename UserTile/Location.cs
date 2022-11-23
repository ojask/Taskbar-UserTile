using System;
using System.IO;
using System.Windows.Forms;

namespace UserTaskerTile.UserTile
{
    public partial class Location : Form
    {
        public Location()
        {
            InitializeComponent();

            if (textBox1.Text != "")
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }

        }

        private void Location_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.Bounds.Height - 500;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 800;
            if (!File.Exists("cityName.txt"))
            {
                File.CreateText("cityName.txt");
            }
            textBox1.Text = File.ReadAllText("cityName.txt");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                button1.Enabled = true;
            }
            else if (textBox1.Text == "")
            {
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Cityname = textBox1.Text;
            File.WriteAllText("cityName.txt", Cityname[0].ToString().ToUpper() + Cityname.Substring(1));
            this.Close();
        }
    }
}
