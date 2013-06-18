namespace Uwitter
{
    partial class FormPopup
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
            this.webPopup = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webPopup
            // 
            this.webPopup.AllowNavigation = false;
            this.webPopup.AllowWebBrowserDrop = false;
            this.webPopup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webPopup.IsWebBrowserContextMenuEnabled = false;
            this.webPopup.Location = new System.Drawing.Point(0, 0);
            this.webPopup.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPopup.Name = "webPopup";
            this.webPopup.ScrollBarsEnabled = false;
            this.webPopup.Size = new System.Drawing.Size(464, 114);
            this.webPopup.TabIndex = 0;
            this.webPopup.WebBrowserShortcutsEnabled = false;
            this.webPopup.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webPopup_DocumentCompleted);
            // 
            // FormPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 114);
            this.ControlBox = false;
            this.Controls.Add(this.webPopup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Name = "FormPopup";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FormPopup";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webPopup;
    }
}