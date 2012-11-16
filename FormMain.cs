using System;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        Twitter auth;
        string since_id;

        public FormMain()
        {
            InitializeComponent();

            listTimeline_ClientSizeChanged(null, null); // サイズ調整
            timerCheck.Interval = 30 * 1000;    // XXX:FIXME!!!
            since_id = null;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AccessToken) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.AccessTokenSecret) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.UserId) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.ScreenName))
            {
                this.Text = Properties.Settings.Default.ScreenName + " - " + Application.ProductName;
                auth = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
                timerCheck_Tick(null, null);
            }
            else
            {
                this.Text = "(未認証) - " + Application.ProductName;
                auth = null;
                timerCheck.Stop();  // 念のため
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

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            timerCheck.Stop();  // 再入を避けるため、いったん止める
            if (auth == null)
            {
                return;
            }

            // タイムライン取得
            var timelines = auth.GetTimeline(since_id);
            for (int i = 0; i < timelines.Length; ++i)
            {
                var timeline = timelines[timelines.Length - i - 1];
                var item = new ListViewItem(new string[]{timeline.text, timeline.id_str});
                listTimeline.Items.Insert(0, item);
                if (Convert.ToDecimal(timeline.id_str) > Convert.ToDecimal(since_id))
                {
                    since_id = timeline.id_str;
                }
            }

            if (!this.Visible)
            {
            }

            timerCheck.Start();
        }

        private void listTimeline_ClientSizeChanged(object sender, EventArgs e)
        {
            colText.Width = listTimeline.ClientSize.Width;
        }
    }
}
