namespace Uwitter
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnSetting = new System.Windows.Forms.Button();
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.editTweet = new System.Windows.Forms.TextBox();
            this.webMain = new System.Windows.Forms.WebBrowser();
            this.btnTweet = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Uwitter";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // btnSetting
            // 
            this.btnSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetting.Location = new System.Drawing.Point(12, 343);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(81, 23);
            this.btnSetting.TabIndex = 0;
            this.btnSetting.Text = "設定(仮配置)";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // timerCheck
            // 
            this.timerCheck.Tick += new System.EventHandler(this.timerCheck_Tick);
            // 
            // editTweet
            // 
            this.editTweet.AcceptsReturn = true;
            this.editTweet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editTweet.Location = new System.Drawing.Point(-1, 270);
            this.editTweet.MaxLength = 140;
            this.editTweet.Multiline = true;
            this.editTweet.Name = "editTweet";
            this.editTweet.Size = new System.Drawing.Size(282, 67);
            this.editTweet.TabIndex = 2;
            this.editTweet.TextChanged += new System.EventHandler(this.editTweet_TextChanged);
            // 
            // webMain
            // 
            this.webMain.AllowNavigation = false;
            this.webMain.AllowWebBrowserDrop = false;
            this.webMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webMain.IsWebBrowserContextMenuEnabled = false;
            this.webMain.Location = new System.Drawing.Point(-1, 1);
            this.webMain.MinimumSize = new System.Drawing.Size(20, 20);
            this.webMain.Name = "webMain";
            this.webMain.Size = new System.Drawing.Size(282, 263);
            this.webMain.TabIndex = 1;
            this.webMain.WebBrowserShortcutsEnabled = false;
            this.webMain.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webMain_DocumentCompleted);
            // 
            // btnTweet
            // 
            this.btnTweet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTweet.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnTweet.ForeColor = System.Drawing.Color.White;
            this.btnTweet.Location = new System.Drawing.Point(191, 343);
            this.btnTweet.Name = "btnTweet";
            this.btnTweet.Size = new System.Drawing.Size(81, 23);
            this.btnTweet.TabIndex = 3;
            this.btnTweet.Text = "ツイート";
            this.btnTweet.UseVisualStyleBackColor = false;
            this.btnTweet.Click += new System.EventHandler(this.btnTweet_Click);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.Location = new System.Drawing.Point(153, 346);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(32, 20);
            this.lblCount.TabIndex = 4;
            this.lblCount.Text = "140";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 369);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnTweet);
            this.Controls.Add(this.webMain);
            this.Controls.Add(this.editTweet);
            this.Controls.Add(this.btnSetting);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ClientSizeChanged += new System.EventHandler(this.FormMain_ClientSizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormMain_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Timer timerCheck;
        private System.Windows.Forms.TextBox editTweet;
        private System.Windows.Forms.WebBrowser webMain;
        private System.Windows.Forms.Button btnTweet;
        private System.Windows.Forms.Label lblCount;
    }
}

