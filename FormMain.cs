using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        const int ITEM_HEIGHT = 80;
        const int ICON_SIZE = 24;

        Twitter twitter;
        string since_id;
        List<Timeline> timelines;

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

            timelines = new List<Timeline>();

            webMain.Visible = false;    // 音を消すため
            webMain.DocumentText = string.Format("<html><head><style type=\"text/css\">{0}</style><script type=\"text/javascript\">{1}</script></head><body><table id=\"tweets\"></table></body></html>", Properties.Resources.css, Properties.Resources.js);

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
            var curTLs = twitter.GetTimeline(since_id);
            if (curTLs != null)
            {
                SetNotifyIcon();
                for (int i = 0; i < curTLs.Length; ++i)
                {
                    var timeline = curTLs[curTLs.Length - i - 1];
                    timelines.Insert(0, timeline);
                    if (Convert.ToDecimal(timeline.id_str) > Convert.ToDecimal(since_id))
                    {
                        since_id = timeline.id_str;
                    }
                }

                var html = new StringBuilder();
                html.Append(@"<table id=""tweets"">");
                foreach (var timeline in timelines)
                {
                    html.Append(@"<tr class=""tweet"" onmouseover=""this.className='hover';"" onmouseout=""this.className='tweet';""><td><a href=""https://twitter.com/");
                    html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
                    html.Append(@"""><img src=""");
                    html.Append(timeline.user.profile_image_url);
                    html.Append(@"""/></a></td><td><a class=""name"" href=""https://twitter.com/");
                    html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
                    html.Append(@""">");
                    html.Append(WebUtility.HtmlEncode(timeline.user.name));
                    html.Append(@"</a> <a class=""screen_name"" href=""https://twitter.com/");
                    html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
                    html.Append(@""">@");
                    html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
                    html.Append(@"</a><br/>");
                    html.Append(WebUtility.HtmlEncode(timeline.text));
                    html.Append(@"<br/><a class=""created_at"" href=""https://twitter.com/");
                    html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
                    html.Append(@"/statuses/");
                    html.Append(WebUtility.HtmlEncode(timeline.id_str));
                    html.Append(@""">");
                    html.Append(WebUtility.HtmlEncode(timeline.created_at));
                    html.Append(@"</a> <span class=""source"">");
                    html.Append(timeline.source);
                    html.Append(@"で</span></td></tr>");
                }
                html.Append(@"</table>");
                webMain.Document.Body.InnerHtml = html.ToString();

                // 上の処理が走るたび、trに対するhoverが解除されるので、今のカーソル位置にhoverを設定しなおす
                var hover = webMain.Document.GetElementFromPoint(webMain.PointToClient(Cursor.Position));
                while (hover != null)
                {
                    if (hover.TagName == "tr" || hover.TagName == "TR")
                    {
                        if (hover.GetAttribute("className").Equals("tweet"))
                        {
                            hover.SetAttribute("className", "hover");
                        }
                        break;
                    }
                    hover = hover.Parent;
                }

                if (curTLs.Length > 0 && !this.Visible)
                {
                    var buf = new StringBuilder();
                    notifyIcon.ShowBalloonTip(15 * 1000, curTLs[0].user.name + " @" + curTLs[0].user.screen_name, curTLs[0].text, ToolTipIcon.None);
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

        private void webMain_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webMain.Visible = true; // 音を消すために非表示にしてあったので戻す
            webMain.Document.Click += new HtmlElementEventHandler(webMain_DocumentClick);
            webMain.Document.Body.KeyDown += new HtmlElementEventHandler(webMain_KeyDown);
        }

        private void webMain_DocumentClick(object sender, HtmlElementEventArgs e)
        {
            var clicked = webMain.Document.GetElementFromPoint(e.MousePosition);
            while (clicked != null)
            {
                if (clicked.TagName == "a" || clicked.TagName == "A")
                {
                    break;
                }
                clicked = clicked.Parent;
            }

            if (clicked != null)
            {
                var href = clicked.GetAttribute("href");
                if (!string.IsNullOrEmpty(href))
                {
                    Process.Start(href);
                }
            }
            e.ReturnValue = false;
        }

        private void webMain_KeyDown(object sender, HtmlElementEventArgs e)
        {
            if (e.KeyPressedCode == 0x09)
            {
                this.SelectNextControl(webMain, true, true, true, true);
                e.ReturnValue = false;
            }
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
    }
}
