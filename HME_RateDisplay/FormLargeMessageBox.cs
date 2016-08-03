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
    public partial class FormLargeMessageBox : Form
    {
        public Button leftButton;
        public Button rightButton;
        public Label messageLabel;
        int mode;

        public FormLargeMessageBox(int mode)
        {
            InitializeComponent();

            this.mode = mode;
            this.Width = 600;
            this.Height = 300;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            RenderUI();
        }

        void RenderUI()
        {
            int buttonOffsetX = 30;
            int buttonOffsetY = 30;

            leftButton = new Button();
            leftButton.Width = 150;
            leftButton.Height = 80;
            leftButton.Font = new Font(this.Font.FontFamily, 14);

            rightButton = new Button();
            rightButton.Width = 150;
            rightButton.Height = 80;
            rightButton.Font = new Font(this.Font.FontFamily, 14);

            switch (this.mode)
            {
                case 0:
                    leftButton.Visible = false;
                    rightButton.Visible = true;
                    rightButton.Location = new Point(this.Width - rightButton.Width - buttonOffsetX,
                                this.Height - rightButton.Height - buttonOffsetY);
                    break;
                case 1:
                    leftButton.Visible = true;
                    rightButton.Visible = true;

                    rightButton.Location = new Point(this.Width / 2 + buttonOffsetX,
                                                    this.Height - rightButton.Height - buttonOffsetY);
                    leftButton.Location = new Point(this.Width / 2 - buttonOffsetX - leftButton.Width,
                                                    rightButton.Location.Y);
                    break;
                case -1:
                    leftButton.Visible = false;
                    rightButton.Visible = false;
                    break;
            }

            int messageLabelOffsetX = 50;
            int messageLabelOffsetY = 50;

            messageLabel = new Label();
            messageLabel.Width = this.Width - (messageLabelOffsetX * 2);
            messageLabel.Height = this.Height - rightButton.Height - messageLabelOffsetY * 2;
            messageLabel.Location = new Point(messageLabelOffsetX, messageLabelOffsetY);
            messageLabel.Font = new Font(this.Font.FontFamily, 14);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(messageLabel);
            this.Controls.Add(leftButton);
            this.Controls.Add(rightButton);
        }

        public void ShowMessageBoxAtLocation(Point location)
        {
            this.BringToFront();
            this.Visible = true;
            this.Location = location;
        }
    }
}
