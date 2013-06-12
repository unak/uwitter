namespace Uwitter
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabAccount = new System.Windows.Forms.TabPage();
            this.linkUser = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.editPin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAuthorize = new System.Windows.Forms.Button();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.editProxyPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.editProxyHost = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkUseProxy = new System.Windows.Forms.CheckBox();
            this.cmbInterval = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.tabSettings.SuspendLayout();
            this.tabAccount.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabAccount);
            this.tabSettings.Controls.Add(this.tabNetwork);
            this.tabSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabSettings.Location = new System.Drawing.Point(0, 0);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(445, 241);
            this.tabSettings.TabIndex = 0;
            // 
            // tabAccount
            // 
            this.tabAccount.Controls.Add(this.btnLogout);
            this.tabAccount.Controls.Add(this.linkUser);
            this.tabAccount.Controls.Add(this.label2);
            this.tabAccount.Controls.Add(this.editPin);
            this.tabAccount.Controls.Add(this.label1);
            this.tabAccount.Controls.Add(this.btnAuthorize);
            this.tabAccount.Location = new System.Drawing.Point(4, 22);
            this.tabAccount.Name = "tabAccount";
            this.tabAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tabAccount.Size = new System.Drawing.Size(437, 215);
            this.tabAccount.TabIndex = 0;
            this.tabAccount.Text = "アカウント";
            this.tabAccount.UseVisualStyleBackColor = true;
            // 
            // linkUser
            // 
            this.linkUser.AutoSize = true;
            this.linkUser.Location = new System.Drawing.Point(100, 17);
            this.linkUser.Name = "linkUser";
            this.linkUser.Size = new System.Drawing.Size(0, 12);
            this.linkUser.TabIndex = 4;
            this.linkUser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkUser_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "現在のアカウント";
            // 
            // editPin
            // 
            this.editPin.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.editPin.Location = new System.Drawing.Point(40, 51);
            this.editPin.MaxLength = 7;
            this.editPin.Name = "editPin";
            this.editPin.Size = new System.Drawing.Size(389, 19);
            this.editPin.TabIndex = 2;
            this.editPin.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "PIN";
            // 
            // btnAuthorize
            // 
            this.btnAuthorize.Location = new System.Drawing.Point(13, 76);
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Size = new System.Drawing.Size(416, 36);
            this.btnAuthorize.TabIndex = 0;
            this.btnAuthorize.UseVisualStyleBackColor = true;
            this.btnAuthorize.Click += new System.EventHandler(this.btnAuthorize_Click);
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.groupBox1);
            this.tabNetwork.Controls.Add(this.cmbInterval);
            this.tabNetwork.Controls.Add(this.label3);
            this.tabNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Size = new System.Drawing.Size(437, 215);
            this.tabNetwork.TabIndex = 1;
            this.tabNetwork.Text = "ネットワーク";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.editProxyPort);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.editProxyHost);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkUseProxy);
            this.groupBox1.Location = new System.Drawing.Point(10, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 108);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "プロクシ";
            // 
            // editProxyPort
            // 
            this.editProxyPort.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.editProxyPort.Location = new System.Drawing.Point(85, 69);
            this.editProxyPort.MaxLength = 5;
            this.editProxyPort.Name = "editProxyPort";
            this.editProxyPort.Size = new System.Drawing.Size(45, 19);
            this.editProxyPort.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "ポート番号";
            // 
            // editProxyHost
            // 
            this.editProxyHost.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.editProxyHost.Location = new System.Drawing.Point(85, 44);
            this.editProxyHost.MaxLength = 128;
            this.editProxyHost.Name = "editProxyHost";
            this.editProxyHost.Size = new System.Drawing.Size(328, 19);
            this.editProxyHost.TabIndex = 2;
            this.editProxyHost.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "ホスト名";
            // 
            // chkUseProxy
            // 
            this.chkUseProxy.AutoSize = true;
            this.chkUseProxy.Location = new System.Drawing.Point(6, 18);
            this.chkUseProxy.Name = "chkUseProxy";
            this.chkUseProxy.Size = new System.Drawing.Size(112, 16);
            this.chkUseProxy.TabIndex = 0;
            this.chkUseProxy.Text = "プロクシを使用する";
            this.chkUseProxy.UseVisualStyleBackColor = true;
            this.chkUseProxy.CheckedChanged += new System.EventHandler(this.chkUseProxy_CheckedChanged);
            // 
            // cmbInterval
            // 
            this.cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterval.FormattingEnabled = true;
            this.cmbInterval.Items.AddRange(new object[] {
            "15秒(Twitter側に制限されます)",
            "30秒(Twitter側に制限されます)",
            "1分",
            "2分",
            "5分",
            "10分",
            "30分"});
            this.cmbInterval.Location = new System.Drawing.Point(67, 14);
            this.cmbInterval.Name = "cmbInterval";
            this.cmbInterval.Size = new System.Drawing.Size(198, 20);
            this.cmbInterval.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "更新頻度";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(275, 247);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 20);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "適用";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(361, 247);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 20);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(372, 12);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(57, 23);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "ログアウト";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 279);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "設定";
            this.tabSettings.ResumeLayout(false);
            this.tabAccount.ResumeLayout(false);
            this.tabAccount.PerformLayout();
            this.tabNetwork.ResumeLayout(false);
            this.tabNetwork.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabAccount;
        private System.Windows.Forms.Button btnAuthorize;
        private System.Windows.Forms.TextBox editPin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabNetwork;
        private System.Windows.Forms.ComboBox cmbInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox editProxyPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox editProxyHost;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkUseProxy;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLogout;
    }
}