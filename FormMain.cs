using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Uwitter
{
    public partial class FormMain : Form
    {
        public delegate bool EnumWindowsDelegate(IntPtr hwnd, UIntPtr lparam);
        [DllImport("user32")]
        public static extern IntPtr EnumWindows(EnumWindowsDelegate callback, UIntPtr lparam);
        [DllImport("user32")]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder buf, int size);
        [DllImport("user32")]
        public static extern long SendMessage(IntPtr hwnd, uint msg, UIntPtr wparam, UIntPtr lparam);

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.Visible)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // テスト
                notifyIcon.ShowBalloonTip(6000, "バルーンテスト", "これはバルーンのテストなのであります！", ToolTipIcon.None);
            }
        }
    }
}
