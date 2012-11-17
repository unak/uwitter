using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        const int ITEM_HEIGHT = 80;

        Twitter auth;
        string since_id;

        public FormMain()
        {
            InitializeComponent();

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

            if (Properties.Settings.Default.Width != 0 && Properties.Settings.Default.Height != 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Left = Properties.Settings.Default.X;
                this.Top = Properties.Settings.Default.Y;
                this.Width = Properties.Settings.Default.Width;
                this.Height = Properties.Settings.Default.Height;
            }

            // タイムライン表示の1行(1ツイート)の高さを調整するためのギミック
            var dummyList = new ImageList();
            dummyList.ImageSize = new Size(1, ITEM_HEIGHT);
            listTimeline.SmallImageList = dummyList;

            SetNotifyIcon();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (auth != null)
            {
                timerCheck_Tick(null, null);
            }
        }

        private void FormMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                // XXX:FIXME!!! 位置保存だが、ここでやるのは頻度が高すぎてよくないか?
                Properties.Settings.Default.X = this.Left;
                Properties.Settings.Default.Y = this.Top;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Height = this.Width;
                Properties.Settings.Default.Save();
            }
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0D && this.ActiveControl == editTweet)
            {
                if (!string.IsNullOrEmpty(editTweet.Text) && auth != null && auth.IsActive)
                {
                    if (auth.SendTweet(editTweet.Text))
                    {
                        editTweet.Clear();
                        timerCheck.Interval = 5 * 1000; // 数秒待たないとツイートが反映されない
                        timerCheck.Start();
                    }
                }
                e.Handled = true;
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm setting = new SettingForm();
            if (setting.ShowDialog() == DialogResult.OK)
            {
                timerCheck.Interval = Properties.Settings.Default.Interval;

                if (string.IsNullOrEmpty(Properties.Settings.Default.AccessTokenSecret))
                {
                    timerCheck.Stop();
                    this.Text = "(未認証) - " + Application.ProductName;
                    auth = null;
                    SetNotifyIcon();
                }
                else
                {
                    this.Text = Properties.Settings.Default.ScreenName + " - " + Application.ProductName;
                    auth = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
                    SetNotifyIcon();
                    timerCheck_Tick(null, null);
                }
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
                SetNotifyIcon();
                for (int i = 0; i < timelines.Length; ++i)
                {
                    var timeline = timelines[timelines.Length - i - 1];
                    var item = new ListViewItem(new string[] { timeline.text, timeline.user.name, timeline.user.screen_name, null, timeline.created_at, timeline.source, timeline.id_str });
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
            else
            {
                SetNotifyIcon(true);
            }

            // Intervalは毎回再設定する(へんなタイミングで呼ぶことがよくあるので)
            timerCheck.Interval = Properties.Settings.Default.Interval > 0 ? Properties.Settings.Default.Interval : 60 * 1000;  // デフォルト1分
            timerCheck.Start();
        }

        private void listTimeline_ClientSizeChanged(object sender, EventArgs e)
        {
            colText.Width = listTimeline.ClientSize.Width;
        }

        private void SetNotifyIcon(bool error = false)
        {
            if (error)
            {
                // XXX:FIXME!!! エラーっぽいアイコン
                notifyIcon.Icon = Properties.Resources.notify;
            }
            else if (auth == null || !auth.IsActive)
            {
                // XXX:FIXME!!! アクティブじゃないっぽいアイコン
                notifyIcon.Icon = Properties.Resources.notify;
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.notify;
            }
        }

        private void listTimeline_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((e.State & (ListViewItemStates.Selected | ListViewItemStates.Hot)) != 0)
            {
                e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }
            var pen = new Pen(Brushes.DarkGray, 2);
            e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);
            pen.Dispose();

            var bounds = new Rectangle(e.Item.Position.X, e.Item.Position.Y, listTimeline.ClientSize.Width, ITEM_HEIGHT);
            var normalFont = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular);
            var nameFont = new Font(normalFont, FontStyle.Bold);
            var smallFont = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Regular);

            // name
            e.Graphics.DrawString(e.Item.SubItems[1].Text, nameFont, Brushes.Black, bounds);

            // screen_name
            var size = e.Graphics.MeasureString(e.Item.SubItems[1].Text, nameFont);
            e.Graphics.DrawString('@' + e.Item.SubItems[2].Text, normalFont, Brushes.DarkSlateGray, bounds.Left + size.Width + 4, bounds.Top);

            // text
            bounds.Y += nameFont.Height + 4;
            bounds.Height = ITEM_HEIGHT - (nameFont.Height + 4) - 2;
            e.Graphics.DrawString(e.Item.SubItems[0].Text, normalFont, Brushes.Black, bounds);

            // created_at
            bounds.Height = smallFont.Height + 2;
            bounds.Y = e.Item.Position.Y + ITEM_HEIGHT - bounds.Height - 2;
            e.Graphics.DrawString(e.Item.SubItems[4].Text, smallFont, Brushes.DarkSlateGray, bounds);

            // source
            size = e.Graphics.MeasureString(e.Item.SubItems[4].Text, nameFont);
            var source = Regex.Replace(e.Item.SubItems[5].Text, @"</?a\b[^>]*>", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            e.Graphics.DrawString(source + "で", smallFont, Brushes.DarkSlateGray, bounds.Left + size.Width, bounds.Top);

            smallFont.Dispose();
            nameFont.Dispose();
            normalFont.Dispose();
        }

        private void listTimeline_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // DrawItemでやっちゃってるので何もしないよ
        }
    }
}
