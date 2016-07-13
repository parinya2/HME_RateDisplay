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
    public partial class FormExchangeRateSetting : FixedSizeForm
    {
        Button goBackButton;
        Button saveButton;
        FormFadeView fadeForm;
        FormLargeMessageBox saveCompletedMessageBox;
        RateDisplayHeaderPanel headerPanel;

        public FormExchangeRateSetting()
        {
            InitializeComponent();

            RenderUI();

            saveCompletedMessageBox = new FormLargeMessageBox(0);
            saveCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveCompletedMessageBox.Message");
            saveCompletedMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveCompletedMessageBox.RightButton");
            saveCompletedMessageBox.Visible = false;
            saveCompletedMessageBox.rightButton.Click += new EventHandler(SaveCompletedMessageBoxRightButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            
        }

        public void RenderUI()
        {

            goBackButton = new Button();
            goBackButton.Width = 200;
            goBackButton.Height = 90;
            goBackButton.Font = new Font(this.Font.FontFamily, 18);
            goBackButton.Location = new Point(SCREEN_WIDTH - goBackButton.Width - 50,
                              SCREEN_HEIGHT - goBackButton.Height - 50);
            goBackButton.Text = "ย้อนกลับ";
            goBackButton.Click += new EventHandler(GoBackButtonClicked);

            saveButton = new Button();
            saveButton.Width = 200;
            saveButton.Height = 90;
            saveButton.BackColor = Color.Orange;           
            saveButton.Font = new Font(this.Font.FontFamily, 18);
            saveButton.Location = new Point(SCREEN_WIDTH / 2 - saveButton.Width / 2,
                                            goBackButton.Location.Y);
            saveButton.Text = "บันทึกค่า";
            saveButton.Click += new EventHandler(SaveButtonClicked);

            headerPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH, 150);

            this.Controls.Add(headerPanel);
            this.Controls.Add(goBackButton);
            this.Controls.Add(saveButton);
        }

        public void RefreshUI()
        { 
        
        }

        void SaveCompletedMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            saveCompletedMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void SaveButtonClicked(object sender, EventArgs e)
        {
            // DO SOMETHING

            fadeForm.Visible = true;
            fadeForm.BringToFront();

            saveCompletedMessageBox.Visible = true;
            saveCompletedMessageBox.BringToFront();
            saveCompletedMessageBox.Location = new Point((SCREEN_WIDTH - saveCompletedMessageBox.Width) / 2,
                                                        (SCREEN_HEIGHT - saveCompletedMessageBox.Height) / 2);
   
        }

        void GoBackButtonClicked(object sender, EventArgs e)
        {
            FormMainMenu instanceFormMainMenu = FormsManager.GetFormMainMenu();
            instanceFormMainMenu.Visible = true;
            instanceFormMainMenu.Enabled = true;

            instanceFormMainMenu.BringToFront();
            this.Visible = false;
        }
    }
}
