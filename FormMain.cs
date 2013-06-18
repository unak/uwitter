using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        const int ITEM_HEIGHT = 80;
        const int ICON_SIZE = 24;

        const int REFRESH_INTERVAL = 5 * 1000;  // 5 sec

        Twitter twitter;
        decimal? since_id;
        decimal? in_reply_to_id;
        string in_reply_to_name;
        List<Timeline> timelines;
        bool hasRead;
        FormPopup popup;

        public FormMain()
        {
            InitializeComponent();

            this.MouseMove += new MouseEventHandler(FormMain_MouseMove);

            btnTweet.Enabled = false;
            btnTweet.BackColor = SystemColors.ControlDark;

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

            since_id = null;
            in_reply_to_id = null;
            in_reply_to_name = null;
            timelines = new List<Timeline>();
            hasRead = false;

            webMain.Visible = false;    // 音を消すため
            webMain.DocumentText = string.Format("<html><head><style type=\"text/css\">{0}</style><script type=\"text/javascript\">{1}</script></head><body><table id=\"tweets\"></table></body></html>", Properties.Resources.css, Properties.Resources.js);

            popup = new FormPopup(this);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (twitter != null)
            {
                timerCheck_Tick(null, null);
            }

            // バルーンを消すため。このタイミングじゃないと残ることもある
            SetNotifyIcon();
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            if (popup != null)
            {
                popup.Hide();
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (twitter != null)
            {
                twitter.Dispose();
                twitter = null;
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

        void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            hasRead = true;
        }

        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            hasRead = true;

            if (this.ActiveControl != editTweet)
            {
                switch (e.KeyChar)
                {
                    case 'j':
                        ScrollTimeline(1);
                        e.Handled = true;
                        break;
                    case 'k':
                        ScrollTimeline(-1);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void editTweet_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editTweet.Text))
            {
                btnTweet.Enabled = false;
                btnTweet.BackColor = SystemColors.ControlDark;
                in_reply_to_id = null;
                in_reply_to_name = null;
            }
            else
            {
                btnTweet.Enabled = true;
                btnTweet.BackColor = Color.DeepSkyBlue;
            }
            int count = 140 - editTweet.Text.Length;
            lblCount.Text = count.ToString();
            if (count > 20)
            {
                lblCount.ForeColor = Color.Black;
            }
            else if (count > 10)
            {
                lblCount.ForeColor = Color.DarkRed;
            }
            else
            {
                lblCount.ForeColor = Color.Red;
            }
        }

        private void btnTweet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(editTweet.Text) && twitter != null && twitter.IsActive)
            {
                if (in_reply_to_id != null)
                {
                    if (!Regex.IsMatch(editTweet.Text, string.Format(@"^@{0}\b", in_reply_to_name)))
                    {
                        in_reply_to_id = null;
                        in_reply_to_name = null;
                    }
                }
                if (twitter.SendTweet(editTweet.Text, in_reply_to_id))
                {
                    in_reply_to_id = null;
                    in_reply_to_name = null;
                    editTweet.Clear();
                    timerCheck.Interval = 3 * 1000; // 数秒待たないとツイートが反映されない
                    timerCheck.Start();
                }
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            var setting = new SettingForm();
            if (setting.ShowDialog() == DialogResult.OK)
            {
                timerCheck.Interval = REFRESH_INTERVAL;

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
            if (webMain.Document.Body == null)
            {
                // 出直し
                timerCheck.Interval = 500;
                timerCheck.Start();
                return;
            }

            timerCheck.Stop();  // 再入を避けるため、いったん止める
            if (twitter == null)
            {
                return;
            }

            UpdateTimeline();

            // Intervalは毎回再設定する(へんなタイミングで呼ぶことがよくあるので)
            timerCheck.Interval = REFRESH_INTERVAL;
            timerCheck.Start();
        }

        private void webMain_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webMain.Visible = true; // 音を消すために非表示にしてあったので戻す
            webMain.Document.Click += new HtmlElementEventHandler(webMain_DocumentClick);
            webMain.Document.Body.KeyDown += new HtmlElementEventHandler(webMain_KeyDown);
            webMain.Document.MouseMove += new HtmlElementEventHandler(webMain_MouseMove);
        }

        private void webMain_DocumentClick(object sender, HtmlElementEventArgs e)
        {
            hasRead = true;

            var clicked = GetClickedLink(webMain, e.MousePosition);
            if (clicked != null)
            {
                var className = clicked.GetAttribute("className");
                var href = clicked.GetAttribute("href");
                if (className.Equals("reply"))
                {
                    EditReply(href);
                }
                else if (className.Equals("retweet"))
                {
                    DoRetweet(href);
                }
                else if (className.Equals("delete"))
                {
                    if (twitter != null && twitter.IsActive &&
                        MessageBox.Show("削除しますか?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        decimal id = Convert.ToDecimal(href.Replace("about:", ""));
                        if (twitter.Destroy(id))
                        {
                            lock (timelines)
                            {
                                timelines.Remove(GetTimelineById(id));
                                UpdateTimeline(false);
                            }
                        }
                    }
                }
                else if (className.Equals("in-reply-to"))
                {
                    ShowChildPopup(popup, href);
                }
                else if (className.Equals("more"))
                {
                    if (twitter != null && twitter.IsActive)
                    {
                        decimal? since_id = null;
                        if (timelines != null && timelines.Count > 0)
                        {
                            since_id = timelines[timelines.Count - 1].id;
                        }
                        var oldTweets = twitter.GetTimeline(since_id, true);
                        if (oldTweets != null)
                        {
                            oldTweets.RemoveAt(0);  // 先頭は重複なので捨てる
                            lock (timelines)
                            {
                                timelines.AddRange(oldTweets);
                            }
                            UpdateTimeline(false);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(href))
                {
                    Process.Start(href);
                }
            }
            e.ReturnValue = false;
        }

        private void webMain_KeyDown(object sender, HtmlElementEventArgs e)
        {
            hasRead = true;

            switch (e.KeyPressedCode)
            {
                case 9:     // TAB
                    this.SelectNextControl(webMain, true, true, true, true);
                    e.ReturnValue = false;
                    break;
                case 74:    // 'j'
                    ScrollTimeline(1);
                    break;
                case 75:    // 'k'
                    ScrollTimeline(-1);
                    break;
            }
        }

        private void webMain_MouseMove(object sender, HtmlElementEventArgs e)
        {
            hasRead = true;
        }

        public static HtmlElement GetClickedLink(WebBrowser browser, Point pos)
        {
            var clicked = browser.Document.GetElementFromPoint(pos);
            while (clicked != null)
            {
                if (clicked.TagName == "a" || clicked.TagName == "A")
                {
                    break;
                }
                clicked = clicked.Parent;
            }
            return clicked;
        }

        public void EditReply(string href)
        {
            in_reply_to_id = Convert.ToDecimal(Regex.Replace(href, "^[^0-9]*([0-9]+)(/.*)$", "$1"));
            in_reply_to_name = Regex.Replace(href, ".*/", "");
            var mentions = new List<string>();
            var timeline = GetTimelineById(in_reply_to_id.Value);
            mentions.Add("@" + in_reply_to_name);
            if (timeline != null)
            {
                var matches = Regex.Matches(WebUtility.HtmlEncode(timeline.text), @"@([0-9A-Za-z_]+)");
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i].Value;
                    if (mentions.IndexOf(match) < 0 && !match.Equals("@" + Properties.Settings.Default.ScreenName))
                    {
                        mentions.Add(match);
                    }
                }
            }
            editTweet.Text = string.Join(" ", mentions) + " ";
            this.ActiveControl = editTweet;
            editTweet.Select(editTweet.Text.Length, 0);
        }

        public void DoRetweet(string href)
        {
            if (twitter != null && twitter.IsActive &&
                MessageBox.Show("リツイートしますか?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (twitter.Retweet(Convert.ToDecimal(href.Replace("about:", ""))))
                {
                    timerCheck.Interval = 3 * 1000; // 数秒待たないとツイートが反映されない
                    timerCheck.Start();
                }
            }
        }

        public void ShowChildPopup(FormPopup child, string href)
        {
            var id = Convert.ToDecimal(Regex.Replace(href, "^[^0-9]*([0-9]+)/[^/]*$", "$1"));
            var user = Regex.Replace(href, ".*/", "");
            var timeline = GetTimelineById(id);
            if (timeline == null && twitter != null && twitter.IsActive)
            {
                timeline = twitter.GetUserTimeline(user, id);
            }
            if (timeline != null)
            {
                child.SetHTML("<table id=\"tweets\">" + FormMain.TimelineToHtml(timeline) + "</table>");
                child.StartPosition = FormStartPosition.Manual;
                child.Location = Cursor.Position;
                if (child.Right >= Screen.PrimaryScreen.Bounds.Right)
                {
                    child.Left -= child.Right - Screen.PrimaryScreen.Bounds.Right;
                }
                if (child.Bottom >= Screen.PrimaryScreen.Bounds.Bottom)
                {
                    child.Top -= child.Bottom - Screen.PrimaryScreen.Bounds.Bottom;
                }
                child.Show();
            }
        }

        private void ScrollTimeline(int count)
        {
            if (webMain.Document.Body != null)
            {
                webMain.Document.Body.ScrollTop += count * 40;  // XXX:FIXME!!!
            }
        }

        private void SetNotifyIcon(bool error = false)
        {
            lock (timelines)
            {
                int num = 0;
                foreach (var timeline in timelines)
                {
                    if (timeline.Unread)
                    {
                        ++num;
                    }
                }
                if (num > 0)
                {
                    notifyIcon.Text = string.Format("{0} (未読:{1})", Application.ProductName, num);
                }
                else
                {
                    notifyIcon.Text = Application.ProductName;
                }
            }

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

        private void UpdateTimeline(bool shouldRead = true)
        {
            // 未読フラグを落とす
            if (hasRead)
            {
                hasRead = false;
                lock (timelines)
                {
                    foreach (var timeline in timelines)
                    {
                        timeline.Unread = false;
                    }
                }
            }

            List<Timeline> curTLs = null;
            if (shouldRead)
            {
                // タイムライン取得
                curTLs = twitter.GetTimeline(since_id);
                if (curTLs != null)
                {
                    lock (timelines)
                    {
                        for (int i = 0; i < curTLs.Count; ++i)
                        {
                            var timeline = curTLs[curTLs.Count - i - 1];
                            timeline.Unread = true;
                            timelines.Insert(0, timeline);
                            if (since_id == null || timeline.id > since_id)
                            {
                                since_id = timeline.id;
                            }
                        }
                    }
                }
            }

            // 表示更新
            var html = new StringBuilder();
            html.Append(@"<table id=""tweets"">");
            lock (timelines)
            {
                // foreachでいいような気がするが、RT時に置き換えをやるので敢えてforで回す
                for (int i = 0; i < timelines.Count; ++i)
                {
                    html.Append(TimelineToHtml(timelines[i]));
                }
            }
            html.Append(@"<tr><td class=""more"" colspan=""3""><a class=""more"" href="""">もっと見る</a></td></tr>");
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

            // タスクトレイアイコン更新
            if (curTLs != null)
            {
                SetNotifyIcon();
                if (curTLs.Count > 0 && !this.Visible)
                {
                    var buf = new StringBuilder();
                    notifyIcon.ShowBalloonTip(15 * 1000, curTLs[0].user.name + " @" + curTLs[0].user.screen_name, curTLs[0].text, ToolTipIcon.None);
                }
            }
            else
            {
                SetNotifyIcon(true);
            }
        }

        private static string TimelineToHtml(Timeline timeline)
        {
            var html = new StringBuilder();
            TwitterUser rt_user = null;
            if (timeline.retweeted_status != null)
            {
                rt_user = timeline.user;
                timeline.retweeted_status.Unread = timeline.Unread;
                timeline = timeline.retweeted_status;
            }
            string className = "tweet";
            if (timeline.Unread)
            {
                className += " unread";
            }
            if (timeline.in_reply_to_user_id != null && Convert.ToDecimal(Properties.Settings.Default.UserId) == timeline.in_reply_to_user_id)
            {
                className += " replied";
            }
            html.Append(string.Format(@"<tr class=""{0}"" onmouseover=""this.className=this.className.replace('tweet', 'hover');"" onmouseout=""this.className=this.className.replace('hover', 'tweet');""><td><a href=""https://twitter.com/", className));
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
            var text = WebUtility.HtmlEncode(timeline.text);
            if (timeline.entities != null && timeline.entities.urls != null)
            {
                foreach (var url in timeline.entities.urls)
                {
                    text = Regex.Replace(text, @"\b" + Regex.Escape(url.url) + @"\b", string.Format(@"<a href=""{0}"">{1}</a>", url.expanded_url, url.display_url));
                }
            }
            if (timeline.entities != null && timeline.entities.media != null)
            {
                foreach (var media in timeline.entities.media)
                {
                    text = Regex.Replace(text, @"\b" + Regex.Escape(media.url) + @"\b", string.Format(@"<a href=""{0}"">{1}</a>", media.expanded_url, media.display_url));
                }
            }
            if (timeline.entities != null && timeline.entities.hashtags != null)
            {
                foreach (var hashtag in timeline.entities.hashtags)
                {
                    text = Regex.Replace(text, @"#" + Regex.Escape(hashtag.text) + @"\b", string.Format(@"<a href=""https://twitter.com/search?q=%23{0}&src=hash"">#{1}</a>", Uri.EscapeDataString(hashtag.text), hashtag.text));
                }
            }
            // 本来はtimeline.user_mentionsを見るべきかとも思うが、あてにならないので無条件にメンションぽいものは全部リンクにしちゃう
            text = Regex.Replace(text, @"@([0-9A-Za-z_]+)", @"<a href=""https://twitter.com/$1"">@$1</a>");
            text = Regex.Replace(text, "\n", "<br/>");  // 改行...
            html.Append(text);
            html.Append(@"<br/><a class=""created_at"" href=""https://twitter.com/");
            html.Append(WebUtility.HtmlEncode(timeline.user.screen_name));
            html.Append(@"/statuses/");
            html.Append(WebUtility.HtmlEncode(timeline.id.ToString()));
            html.Append(@""">");
            html.Append(WebUtility.HtmlEncode(timeline.RelativeCreatedAt));
            html.Append(@"</a>");
            if (timeline.in_reply_to_status_id != null)
            {
                html.Append(string.Format(@" <a class=""in-reply-to"" href=""{0}/{1}"">{2} への返信</a>", timeline.in_reply_to_status_id.ToString(), timeline.in_reply_to_user_id.ToString(), timeline.in_reply_to_screen_name));
            }
            html.Append(@" <span class=""source"">");
            html.Append(timeline.source);
            html.Append(@"で</span>");
            if (rt_user != null)
            {
                html.Append(string.Format(@"<br/><span class=""retweeted""><a href=""https://twitter.com/{0}"">{0}</a>がリツイート</span>", rt_user.screen_name));
            }
            html.Append(string.Format(@"</td><td class=""re""><a class=""reply"" href=""{0}/{1}"">RE</a><br/><a class=""retweet"" href=""{0}"">RT</a>", timeline.id.ToString(), timeline.user.screen_name));
            if (timeline.user.id_str.Equals(Properties.Settings.Default.UserId))
            {
                html.Append(string.Format(@"<br/><a class=""delete"" href=""{0}"">DEL</a>", timeline.id.ToString()));
            }
            html.Append(@"</td></tr>");

            return html.ToString();
        }

        private Timeline GetTimelineById(decimal id)
        {
            Timeline timeline = null;
            lock (timelines)
            {
                timeline = timelines.Find(delegate(Timeline elem)
                {
                    if (elem.retweeted_status != null)
                    {
                        elem = elem.retweeted_status;
                    }
                    return elem.id.Equals(id);
                });
            }
            return timeline;
        }
    }
}
