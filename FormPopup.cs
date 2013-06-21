using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormPopup : Form
    {
        private FormPopup child;
        private FormMain main;

        public FormPopup(FormMain main)
        {
            InitializeComponent();

            this.main = main;

            webPopup.Visible = false;    // 音を消すため
            webPopup.DocumentText = string.Format("<html><head><style type=\"text/css\">{0}</style><script type=\"text/javascript\">{1}</script></head><body class=\"popup\"><table id=\"tweets\"></table></body></html>", Properties.Resources.css, Properties.Resources.js);
        }

        private void FormPopup_Activated(object sender, EventArgs e)
        {
            if (child != null)
            {
                child.Hide();
            }
        }

        private void FormPopup_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false && child != null)
            {
                child.Hide();
            }
        }

        private void webPopup_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webPopup.Visible = true; // 音を消すために非表示にしてあったので戻す
            webPopup.Document.Click += new HtmlElementEventHandler(webPopup_DocumentClick);
        }

        private void webPopup_DocumentClick(object sender, HtmlElementEventArgs e)
        {
            HtmlElement clicked = FormMain.GetClickedLink(webPopup, e.MousePosition);
            if (clicked != null)
            {
                var className = clicked.GetAttribute("className");
                var href = clicked.GetAttribute("href");
                if (className.Equals("reply"))
                {
                    main.EditReply(href);
                }
                else if (className.Equals("retweet"))
                {
                    main.DoRetweet(href);
                }
                else if (className.Equals("in-reply-to"))
                {
                    main.ShowChildPopup(child, href);
                }
                else if (!string.IsNullOrEmpty(href))
                {
                    Process.Start(href);
                }
            }
        }

        public void SetHTML(string html)
        {
            while (webPopup.Document.Body == null)
            {
                System.Threading.Thread.Sleep(100);
            }
            webPopup.Document.Body.InnerHtml = html;
            this.Size = webPopup.Document.Body.ScrollRectangle.Size;

            // 子供を作っておく
            if (child == null)
            {
                child = new FormPopup(main);
            }
        }
    }
}
