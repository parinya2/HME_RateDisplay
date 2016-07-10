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
    public partial class FixedSizeFormWithHeader : FixedSizeForm
    {
        public Label headerLineLabel;
        private Label headerTextLabel;
        private PictureBox dltLogoPictureBox;
        int headerLineGap = 20;

        public FixedSizeFormWithHeader()
        {
            InitializeComponent();

            dltLogoPictureBox = new PictureBox();
            dltLogoPictureBox.Width = 150;
            dltLogoPictureBox.Height = 150;
            dltLogoPictureBox.Location = new Point(70, 30);
            dltLogoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
           // dltLogoPictureBox.Image = Util.GetImageFromImageResources("DLT_Logo.png");

            headerLineLabel = new Label();
            headerLineLabel.BackColor = Color.Black;
            headerLineLabel.Width = SCREEN_WIDTH - headerLineGap * 2;
            headerLineLabel.Height = 3;
            headerLineLabel.Location = new Point(headerLineGap, dltLogoPictureBox.Location.Y + dltLogoPictureBox.Height + 10);

            headerTextLabel = new Label();
            headerTextLabel.ForeColor = Color.Black;
            headerTextLabel.Width = SCREEN_WIDTH - dltLogoPictureBox.Width - dltLogoPictureBox.Location.X - headerLineGap * 3;
            headerTextLabel.Height = dltLogoPictureBox.Height;
            headerTextLabel.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel.Font = new Font(this.Font.FontFamily, 22);
            headerTextLabel.Text = "Hatyai Money Exchange";
            headerTextLabel.Location = new Point(dltLogoPictureBox.Location.X + dltLogoPictureBox.Width + headerLineGap,
                                                 dltLogoPictureBox.Location.Y);

            this.Controls.Add(headerTextLabel);
            this.Controls.Add(dltLogoPictureBox);
            this.Controls.Add(headerLineLabel);
        }


    }
}
