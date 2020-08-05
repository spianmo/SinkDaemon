using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SinkDaemon
{
    public partial class frmProtect : Form
    {
        public frmProtect()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void Form1_Show(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            Application.Exit();
            Application.ExitThread();
            Environment.Exit(0);
            Process.GetCurrentProcess().Kill();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_APPWINDOW = 0x40000;
                const int WS_EX_TOOLWINDOW = 0x80;
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= (~WS_EX_APPWINDOW);    // 不显示在TaskBar
                cp.ExStyle |= WS_EX_TOOLWINDOW;      // 不显示在Alt+Tab
                return cp;
            }
        }
    }
}
