using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HME_RateDisplay
{
    public partial class FixedSizeForm : Form
    {
        public int SCREEN_WIDTH;
        public int SCREEN_HEIGHT;

        public FixedSizeForm()
        {
            InitializeComponent();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;


            SCREEN_WIDTH = SystemInformation.VirtualScreen.Width;
            SCREEN_HEIGHT = SystemInformation.VirtualScreen.Height;
        }
    }
}
