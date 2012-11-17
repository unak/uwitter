using System;
using System.Text;
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
            since_id = null;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AccessToken) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.AccessTokenSecret) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.UserId) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.ScreenName))
            {
                this.Text = Properties.Settings.Default.ScreenName + " - " + Application.ProductName;
                auth = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
            }
            else
            {
                this.Text = "(未認証) - " + Application.ProductName;
                auth = null;
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (auth != null)
            {
                if (Properties.Settings.Default.Interval > 0)
                {
                    timerCheck.Interval = Properties.Settings.Default.Interval;
                }
                else
                {
                    timerCheck.Interval = 60 * 1000;    // デフォルトは1分
                }
                timerCheck_Tick(null, null);
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
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    this.Visible = true;
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingForm setting = new SettingForm();
            if (setting.ShowDialog() == DialogResult.OK)
            {
                timerCheck.Interval = Properties.Settings.Default.Interval;
            }
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
            if (timelines != null)
            {
                for (int i = 0; i < timelines.Length; ++i)
                {
                    var timeline = timelines[timelines.Length - i - 1];
                    var item = new ListViewItem(new string[] { timeline.text, timeline.id_str });
                    listTimeline.Items.Insert(0, item);
                    if (Convert.ToDecimal(timeline.id_str) > Convert.ToDecimal(since_id))
                    {
                        since_id = timeline.id_str;
                    }
                }

                if (timelines.Length > 0 && !this.Visible)
                {
                    var buf = new StringBuilder();
                    notifyIcon.ShowBalloonTip(15 * 1000, timelines[0].user.name + " @" + timelines[0].user.screen_name, timelines[0].text, ToolTipIcon.None);
                }
            }

            timerCheck.Start();
        }

        private void listTimeline_ClientSizeChanged(object sender, EventArgs e)
        {
            colText.Width = listTimeline.ClientSize.Width;
        }
    }
}
