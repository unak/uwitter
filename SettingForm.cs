using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class SettingForm : Form
    {
        const string FIRST_TEXT_OF_BUTTON = "Twitterでこのアプリを認証する";

        Twitter auth;

        public SettingForm()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.ScreenName))
            {
                linkUser.Text = Properties.Settings.Default.ScreenName;
                linkUser.Enabled = true;
            }
            else
            {
                linkUser.Text = "(未認証)";
                linkUser.Enabled = false;
            }
            auth = null;
            editPin.Text = "";
            editPin.Enabled = false;
            btnAuthorize.Text = FIRST_TEXT_OF_BUTTON;
        }

        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            if (auth != null && editPin.Enabled)
            {
                var userName = auth.GetAccessToken(editPin.Text);
                editPin.Text = "";
                editPin.Enabled = false;
                btnAuthorize.Text = FIRST_TEXT_OF_BUTTON;
                if (string.IsNullOrEmpty(userName))
                {
                    MessageBox.Show("認証に失敗したのでやり直せー", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                linkUser.Text = userName;
                linkUser.Enabled = true;
            }
            else
            {
                auth = new Twitter(OAuthKey.CONSUMER_KEY, OAuthKey.CONSUMER_SECRET);
                if (!auth.GetRequestToken())
                {
                    MessageBox.Show("なんかだめぽ", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Process.Start(auth.GetAuthorizeUrl());

                MessageBox.Show("Twitterで「うぃったー」を認証し、表示されるPINを入力して認証を完了してください。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                editPin.Enabled = true;
                btnAuthorize.Text = "認証を完了する";
            }
        }

        private void linkUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitter.com/" + Uri.EscapeDataString(linkUser.Text));
        }
    }
}
