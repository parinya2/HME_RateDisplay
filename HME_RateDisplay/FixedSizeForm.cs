using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace HME_RateDisplay
{
    public partial class FixedSizeForm : Form
    {
        public static int SCREEN_WIDTH;
        public static int SCREEN_HEIGHT;
        public int SCREEN_OFFSET_X;

        public FixedSizeForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(FixedSizeFormLoaded);
        }

        private void FixedSizeFormLoaded(object sender, System.EventArgs e)
        {
            Screen[] screens = Screen.AllScreens;
            Screen targetScreen = screens[0];
            Screen primaryScreen = screens[0];
            bool isExtendedScreen = screens.Length > 1;
            for (int i = 0; i < screens.Length; i++)
            {
                Screen x = screens[i];
                if (!x.Primary)
                {
                    targetScreen = x;
                }
            }

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            if (isExtendedScreen)
            {
                SCREEN_WIDTH = targetScreen.WorkingArea.Width - 150;
                SCREEN_HEIGHT = targetScreen.WorkingArea.Height - 120;
                SCREEN_OFFSET_X = primaryScreen.Bounds.Width;

                this.Size = new System.Drawing.Size(SCREEN_WIDTH, SCREEN_HEIGHT);
                this.Location = new Point(targetScreen.WorkingArea.X + 60, targetScreen.WorkingArea.Y + 40);
            }
            else
            {
                SCREEN_WIDTH = SystemInformation.VirtualScreen.Width;
                SCREEN_HEIGHT = SystemInformation.VirtualScreen.Height;
                SCREEN_OFFSET_X =  0;

                this.Size = new System.Drawing.Size(SCREEN_WIDTH, SCREEN_HEIGHT);
                this.Location = new Point(0, 0);       
            }

        }
    }
}
