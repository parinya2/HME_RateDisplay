using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HME_RateDisplay
{
    public class LargeButton : Button
    {
        public LargeButton()
        {
            this.Width = 300;
            this.Height = 150;
            this.Font = new Font(this.Font.FontFamily, 26);
        }
    }
}
