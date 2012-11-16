using System;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        OAuth1 auth;

        public FormMain()
        {
            InitializeComponent();

            string screenName = Properties.Settings.Default.ScreenName;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AccessToken) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.AccessTokenSecret) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.UserId) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.ScreenName))
            {
                this.Text = Properties.Settings.Default.ScreenName + " - " + Application.ProductName;
                auth = new OAuth1(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
            }
            else
            {
                this.Text = "(未認証) - " + Application.ProductName;
                auth = null;
            }
        }

        private void FormMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.Visible)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // テスト
                notifyIcon.ShowBalloonTip(6000, "バルーンテスト", "これはバルーンのテストなのであります！", ToolTipIcon.None);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingForm setting = new SettingForm();
            setting.ShowDialog();
        }
    }
}
