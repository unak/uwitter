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

            // アカウントタブ関連
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

            // ネットワークタブ関連
            if (Properties.Settings.Default.Interval <= 0)
            {
                cmbInterval.SelectedIndex = 2;  // デフォルトは1分
            }
            else if (Properties.Settings.Default.Interval <= 15 * 1000)
            {
                cmbInterval.SelectedIndex = 0;  // 15秒
            }
            else if (Properties.Settings.Default.Interval <= 30 * 1000)
            {
                cmbInterval.SelectedIndex = 1;  // 30秒
            }
            else if (Properties.Settings.Default.Interval <= 1 * 60 * 1000)
            {
                cmbInterval.SelectedIndex = 2;  // 1分
            }
            else if (Properties.Settings.Default.Interval <= 2 * 60 * 1000)
            {
                cmbInterval.SelectedIndex = 3;  // 2分
            }
            else if (Properties.Settings.Default.Interval <= 5 * 60 * 1000)
            {
                cmbInterval.SelectedIndex = 4;  // 5分
            }
            else if (Properties.Settings.Default.Interval <= 10 * 60 * 1000)
            {
                cmbInterval.SelectedIndex = 5;  // 10分
            }
            else
            {
                cmbInterval.SelectedIndex = 6;  // 30分
            }
            if (Properties.Settings.Default.UseProxy)
            {
                chkUseProxy.Checked = true;
            }
            else
            {
                chkUseProxy.Checked = false;
            }
            chkUseProxy_CheckedChanged(null, null);
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ProxyHost))
            {
                editProxyHost.Text = Properties.Settings.Default.ProxyHost;
            }
            if (Properties.Settings.Default.ProxyPort > 0)
            {
                editProxyPort.Text = Properties.Settings.Default.ProxyPort.ToString();
            }
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

                MessageBox.Show("Twitterで「うぃったー」を認証し、表示されるPINを入力して認証を完了してください。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(auth.GetAuthorizeUrl());

                editPin.Enabled = true;
                btnAuthorize.Text = "認証を完了する";
            }
        }

        private void linkUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitter.com/" + Uri.EscapeDataString(linkUser.Text));
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            switch (cmbInterval.SelectedIndex)
            {
                case 0: // 15秒
                    Properties.Settings.Default.Interval = 15 * 1000;
                    break;
                case 1: // 30秒
                    Properties.Settings.Default.Interval = 30 * 1000;
                    break;
                case 2: // 1分
                default:
                    Properties.Settings.Default.Interval = 1 * 60 * 1000;
                    break;
                case 3: // 2分
                    Properties.Settings.Default.Interval = 2 * 60 * 1000;
                    break;
                case 4: // 5分
                    Properties.Settings.Default.Interval = 5 * 60 * 1000;
                    break;
                case 5: // 10分
                    Properties.Settings.Default.Interval = 10 * 60 * 1000;
                    break;
                case 6: // 30分
                    Properties.Settings.Default.Interval = 30 * 60 * 1000;
                    break;
            }

            Properties.Settings.Default.UseProxy = chkUseProxy.Checked;
            if (string.IsNullOrEmpty(editProxyHost.Text))
            {
                Properties.Settings.Default.UseProxy = false;
                Properties.Settings.Default.ProxyHost = "";
            }
            else
            {
                Properties.Settings.Default.ProxyHost = editProxyHost.Text;
            }
            ushort port;
            if (ushort.TryParse(editProxyPort.Text, out port) && port > 0)
            {
                Properties.Settings.Default.ProxyPort = port;
            }
            else
            {
                Properties.Settings.Default.UseProxy = false;
                Properties.Settings.Default.ProxyPort = 0;
            }

            Properties.Settings.Default.Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
        }

        private void chkUseProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseProxy.Checked)
            {
                editProxyHost.Enabled = true;
                editProxyPort.Enabled = true;
            }
            else
            {
                editProxyHost.Enabled = false;
                editProxyPort.Enabled = false;
            }
        }
    }
}
