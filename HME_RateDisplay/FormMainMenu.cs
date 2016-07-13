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
    public partial class FormMainMenu : FixedSizeForm
    {
        LargeButton rateDisplayButton;
        LargeButton settingButton;
        Button exitButton;
        FormFadeView fadeForm;
        FormLargeMessageBox confirmExitMessageBox;
        RateDisplayHeaderPanel headerPanel;

        public FormMainMenu()
        {
            InitializeComponent();
            LocalizedTextManager.InitInstance();
            ExchangeRateDataManager.InitInstance();
            FormsManager.InitInstance();
            FormsManager.SetFormMainMenu(this);

            RenderUI();

            confirmExitMessageBox = new FormLargeMessageBox(1);
            confirmExitMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.Message");
            confirmExitMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.RightButton");
            confirmExitMessageBox.leftButton.Text = LocalizedTextManager.GetLocalizedTextForKey("ConfirmExitMessageBox.LeftButton");
            confirmExitMessageBox.Visible = false;
            confirmExitMessageBox.rightButton.Click += new EventHandler(ConfirmExitMessageBoxRightButtonClicked);
            confirmExitMessageBox.leftButton.Click += new EventHandler(ConfirmExitMessageBoxLeftButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();

            FormFadeView baseBG = FormsManager.GetFormBaseBackgroundView();
            baseBG.Visible = true;
            this.BringToFront();
        }

        public void RenderUI()
        {
            int buttonGap = 100;

            rateDisplayButton = new LargeButton();
            rateDisplayButton.Location = new Point((SCREEN_WIDTH / 2 - rateDisplayButton.Width - buttonGap / 2),
                                             (SCREEN_HEIGHT - rateDisplayButton.Height) / 2);
            rateDisplayButton.Text = "เข้าสู่หน้าหลัก";
            rateDisplayButton.Click += new EventHandler(ButtonClickedRateDisplay);


            settingButton = new LargeButton();
            settingButton.Location = new Point((SCREEN_WIDTH / 2 + buttonGap / 2),
                                             (SCREEN_HEIGHT - settingButton.Height) / 2);
            settingButton.Text = "ตั้งค่า";
            settingButton.Click += new EventHandler(ButtonClickedSetting);

            exitButton = new Button();
            exitButton.Width = 200;
            exitButton.Height = 90;
            exitButton.Font = new Font(this.Font.FontFamily, 18);
            exitButton.Location = new Point(SCREEN_WIDTH - exitButton.Width - 50,
                              SCREEN_HEIGHT - exitButton.Height - 50);
            exitButton.Text = "ออกจากโปรแกรม";
            exitButton.Click += new EventHandler(ExitButtonClicked);

            headerPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH , 260);
            headerPanel.Location = new Point(0 , 0);

            this.Controls.Add(headerPanel);
            this.Controls.Add(exitButton);
            this.Controls.Add(rateDisplayButton);
            this.Controls.Add(settingButton);
        }


        void ExitButtonClicked(object sender, EventArgs e)
        {
            fadeForm.Visible = true;
            fadeForm.BringToFront();

            confirmExitMessageBox.Visible = true;
            confirmExitMessageBox.BringToFront();
            confirmExitMessageBox.Location = new Point((SCREEN_WIDTH - confirmExitMessageBox.Width) / 2,
                                                    (SCREEN_HEIGHT - confirmExitMessageBox.Height) / 2);
        }

        void ButtonClickedRateDisplay(object sender, EventArgs e)
        {
            FormRateDisplay instanceFormRateDisplay = FormsManager.GetFormRateDisplay();
            instanceFormRateDisplay.Visible = true;
            instanceFormRateDisplay.Enabled = true;
            instanceFormRateDisplay.RefreshUI();
            instanceFormRateDisplay.BringToFront();
            this.Visible = false;
        }

        void ButtonClickedSetting(object sender, EventArgs e)
        {
            FormExchangeRateSetting instanceFormExchangeRateSetting = FormsManager.GetFormExchangeRateSetting();
            instanceFormExchangeRateSetting.Visible = true;
            instanceFormExchangeRateSetting.Enabled = true;
            instanceFormExchangeRateSetting.RefreshUI();
            instanceFormExchangeRateSetting.BringToFront();
            this.Visible = false;
        }

        void ExitProgram()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console app
                System.Environment.Exit(1);
            }
        }

        void ConfirmExitMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            ExitProgram();
        }

        void ConfirmExitMessageBoxLeftButtonClicked(object sender, EventArgs e)
        {
            confirmExitMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }
    }
}
