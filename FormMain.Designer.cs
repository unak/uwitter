﻿namespace Uwitter
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
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnSetting = new System.Windows.Forms.Button();
            this.timerCheck = new System.Windows.Forms.Timer(this.components);
            this.listTimeline = new System.Windows.Forms.ListView();
            this.colText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.editTweet = new System.Windows.Forms.TextBox();
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
            this.btnSetting.Location = new System.Drawing.Point(202, -1);
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
            // listTimeline
            // 
            this.listTimeline.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listTimeline.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colText});
            this.listTimeline.FullRowSelect = true;
            this.listTimeline.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listTimeline.Location = new System.Drawing.Point(0, 110);
            this.listTimeline.MultiSelect = false;
            this.listTimeline.Name = "listTimeline";
            this.listTimeline.OwnerDraw = true;
            this.listTimeline.Size = new System.Drawing.Size(283, 259);
            this.listTimeline.TabIndex = 1;
            this.listTimeline.UseCompatibleStateImageBehavior = false;
            this.listTimeline.View = System.Windows.Forms.View.Details;
            this.listTimeline.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listTimeline_DrawItem);
            this.listTimeline.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listTimeline_DrawSubItem);
            this.listTimeline.ClientSizeChanged += new System.EventHandler(this.listTimeline_ClientSizeChanged);
            // 
            // colText
            // 
            this.colText.Text = "";
            // 
            // editTweet
            // 
            this.editTweet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editTweet.Location = new System.Drawing.Point(0, 31);
            this.editTweet.MaxLength = 140;
            this.editTweet.Multiline = true;
            this.editTweet.Name = "editTweet";
            this.editTweet.Size = new System.Drawing.Size(282, 79);
            this.editTweet.TabIndex = 2;
            this.editTweet.WordWrap = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 369);
            this.Controls.Add(this.editTweet);
            this.Controls.Add(this.listTimeline);
            this.Controls.Add(this.btnSetting);
            this.KeyPreview = true;
            this.Name = "FormMain";
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
        private System.Windows.Forms.ListView listTimeline;
        private System.Windows.Forms.ColumnHeader colText;
        private System.Windows.Forms.TextBox editTweet;
    }
}

