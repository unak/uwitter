using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        const int ITEM_HEIGHT = 80;
        const int ICON_SIZE = 24;

        Twitter twitter;
        string since_id;
        IDictionary<string, Image> icons;

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
                twitter = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
            }
            else
            {
                this.Text = "(未認証) - " + Application.ProductName;
                twitter = null;
            }

            if (Properties.Settings.Default.Width != 0 && Properties.Settings.Default.Height != 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopBounds = new Rectangle(Properties.Settings.Default.X, Properties.Settings.Default.Y, Properties.Settings.Default.Width, Properties.Settings.Default.Height);
            }

            icons = new Dictionary<string, Image>();

            // タイムライン表示の1行(1ツイート)の高さを調整するためのギミック
            var dummyList = new ImageList();
            dummyList.ImageSize = new Size(1, ITEM_HEIGHT);
            listTimeline.SmallImageList = dummyList;

            SetNotifyIcon();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (twitter != null)
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
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Save();
            }
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0D && this.ActiveControl == editTweet)
            {
                if (!string.IsNullOrEmpty(editTweet.Text) && twitter != null && twitter.IsActive)
                {
                    if (twitter.SendTweet(editTweet.Text))
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
                    twitter = null;
                    SetNotifyIcon();
                }
                else
                {
                    this.Text = Properties.Settings.Default.ScreenName + " - " + Application.ProductName;
                    twitter = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET, Properties.Settings.Default.AccessToken, Properties.Settings.Default.AccessTokenSecret);
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
            if (twitter == null)
            {
                return;
            }

            // タイムライン取得
            var timelines = twitter.GetTimeline(since_id);
            if (timelines != null)
            {
                SetNotifyIcon();
                for (int i = 0; i < timelines.Length; ++i)
                {
                    var timeline = timelines[timelines.Length - i - 1];
                    var item = new ListViewItem(new string[] { timeline.text, timeline.user.name, timeline.user.screen_name, timeline.created_at, timeline.source, timeline.id_str });
                    listTimeline.Items.Insert(0, item);
                    if (Convert.ToDecimal(timeline.id_str) > Convert.ToDecimal(since_id))
                    {
                        since_id = timeline.id_str;
                    }

                    LoadUserIcon(timeline.user.screen_name, timeline.user.profile_image_url);
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
            using (var pen = new Pen(Brushes.DarkGray, 2))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);
            }

            var bounds = new Rectangle(e.Item.Position.X + ICON_SIZE + 4, e.Item.Position.Y, listTimeline.ClientSize.Width - (ICON_SIZE + 4), ITEM_HEIGHT);

            // icon
            lock (icons)
            {
                Image icon;
                if (icons.TryGetValue(e.Item.SubItems[1].Text, out icon) && icon != null)
                {
                    e.Graphics.DrawImage(icon, e.Item.Position.X, e.Item.Position.Y, ICON_SIZE, ICON_SIZE);
                }
            }

            using (var normalFont = new Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular))
            {
                using (var nameFont = new Font(normalFont, FontStyle.Bold))
                {
                    // name
                    e.Graphics.DrawString(e.Item.SubItems[1].Text, nameFont, Brushes.Black, bounds);

                    // screen_name
                    var size = e.Graphics.MeasureString(e.Item.SubItems[1].Text, nameFont);
                    e.Graphics.DrawString('@' + e.Item.SubItems[2].Text, normalFont, Brushes.DarkSlateGray, bounds.Left + size.Width + 4, bounds.Top);

                    // text
                    bounds.Y += nameFont.Height + 4;
                    bounds.Height = ITEM_HEIGHT - (nameFont.Height + 4) - 2;
                    e.Graphics.DrawString(e.Item.SubItems[0].Text, normalFont, Brushes.Black, bounds);

                    using (var smallFont = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Regular))
                    {
                        // created_at
                        bounds.Height = smallFont.Height + 2;
                        bounds.Y = e.Item.Position.Y + ITEM_HEIGHT - bounds.Height - 2;
                        e.Graphics.DrawString(e.Item.SubItems[3].Text, smallFont, Brushes.DarkSlateGray, bounds);

                        // source
                        size = e.Graphics.MeasureString(e.Item.SubItems[3].Text, nameFont);
                        var source = Regex.Replace(e.Item.SubItems[4].Text, @"</?a\b[^>]*>", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        e.Graphics.DrawString(source + "で", smallFont, Brushes.DarkSlateGray, bounds.Left + size.Width, bounds.Top);
                    }
                }
            }
        }

        private void listTimeline_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // DrawItemでやっちゃってるので何もしないよ
        }

        private void SetNotifyIcon(bool error = false)
        {
            if (error)
            {
                // XXX:FIXME!!! エラーっぽいアイコン
                notifyIcon.Icon = Properties.Resources.notify;
            }
            else if (twitter == null || !twitter.IsActive)
            {
                // XXX:FIXME!!! アクティブじゃないっぽいアイコン
                notifyIcon.Icon = Properties.Resources.notify;
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.notify;
            }
        }

        private void LoadUserIcon(string screen_name, string url)
        {
            lock (icons)
            {
                // まだテーブルにscreen_nameがないなら、誰も取得しに行っていない
                if (!icons.ContainsKey(screen_name))
                {
                    // まず予約
                    icons.Add(screen_name, null);

                    // 別スレッドでアイコンを取りに行く
                    Task.Factory.StartNew(() =>
                    {
                        var data = Twitter.HttpGetBinary(url, null);
                        if (data != null)
                        {
                            Image icon;
                            using (var stream = new MemoryStream(data))
                            {
                                icon = Image.FromStream(stream);
                            }
                            if (icon != null)
                            {
                                lock (icons)
                                {
                                    icons[screen_name] = icon;
                                    listTimeline.Invalidate();
                                }
                            }
                        }
                    });
                }
            }
        }
    }
}
