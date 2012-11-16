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
            this.btnAuthorize = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.editPin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.linkUser = new System.Windows.Forms.LinkLabel();
            this.tabSettings.SuspendLayout();
            this.tabAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabAccount);
            this.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSettings.Location = new System.Drawing.Point(0, 0);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(445, 279);
            this.tabSettings.TabIndex = 0;
            // 
            // tabAccount
            // 
            this.tabAccount.Controls.Add(this.linkUser);
            this.tabAccount.Controls.Add(this.label2);
            this.tabAccount.Controls.Add(this.editPin);
            this.tabAccount.Controls.Add(this.label1);
            this.tabAccount.Controls.Add(this.btnAuthorize);
            this.tabAccount.Location = new System.Drawing.Point(4, 22);
            this.tabAccount.Name = "tabAccount";
            this.tabAccount.Padding = new System.Windows.Forms.Padding(3);
            this.tabAccount.Size = new System.Drawing.Size(437, 253);
            this.tabAccount.TabIndex = 0;
            this.tabAccount.Text = "アカウント";
            this.tabAccount.UseVisualStyleBackColor = true;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "PIN";
            // 
            // editPin
            // 
            this.editPin.Location = new System.Drawing.Point(40, 51);
            this.editPin.Name = "editPin";
            this.editPin.Size = new System.Drawing.Size(389, 19);
            this.editPin.TabIndex = 2;
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
            // linkUser
            // 
            this.linkUser.AutoSize = true;
            this.linkUser.Location = new System.Drawing.Point(100, 17);
            this.linkUser.Name = "linkUser";
            this.linkUser.Size = new System.Drawing.Size(0, 12);
            this.linkUser.TabIndex = 4;
            this.linkUser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkUser_LinkClicked);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 279);
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
    }
}