using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormPopup : Form
    {
        public FormPopup()
        {
            InitializeComponent();

            webPopup.Visible = false;    // 音を消すため
            webPopup.DocumentText = string.Format("<html><head><style type=\"text/css\">{0}</style><script type=\"text/javascript\">{1}</script></head><body><table id=\"tweets\"></table></body></html>", Properties.Resources.css, Properties.Resources.js);
        }

        private void webPopup_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webPopup.Visible = true; // 音を消すために非表示にしてあったので戻す
            webPopup.Document.Click += new HtmlElementEventHandler(webPopup_DocumentClick);
        }

        private void webPopup_DocumentClick(object sender, HtmlElementEventArgs e)
        {
            // XXX: FIX ME!!!
        }

        public void SetHTML(string html)
        {
            while (webPopup.Document.Body == null)
            {
                System.Threading.Thread.Sleep(100);
            }
            webPopup.Document.Body.InnerHtml = html;
            this.Size = webPopup.Document.Body.ScrollRectangle.Size;
        }
    }
}
