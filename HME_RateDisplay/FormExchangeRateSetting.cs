﻿using System;
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
        FormLargeMessageBox saveErrorMessageBox;
        RateDisplayHeaderPanel rateSettingHeaderPanel;
        RateSettingContentPanel rateSettingContentPanel;

        Label displayRefreshIntervalLabel;
        TextBox displayRefreshIntervalTextbox;

        String SAVE_SUCCESS = "SAVE_SUCCESS";
        String SAVE_ERROR_INVALID_CHAR = "SAVE_ERROR_INVALID_CHAR";

        public FormExchangeRateSetting()
        {
            InitializeComponent();

            this.Load += new EventHandler(OnFormLoaded);

            saveCompletedMessageBox = new FormLargeMessageBox(0);
            saveCompletedMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveCompletedMessageBox.Message");
            saveCompletedMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveCompletedMessageBox.RightButton");
            saveCompletedMessageBox.Visible = false;
            saveCompletedMessageBox.rightButton.Click += new EventHandler(SaveCompletedMessageBoxRightButtonClicked);

            saveErrorMessageBox = new FormLargeMessageBox(0);
            saveErrorMessageBox.messageLabel.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveErrorMessageBox.InvalidChar.Message");
            saveErrorMessageBox.rightButton.Text = LocalizedTextManager.GetLocalizedTextForKey("SaveErrorMessageBox.RightButton");
            saveErrorMessageBox.Visible = false;
            saveErrorMessageBox.rightButton.Click += new EventHandler(SaveErrorMessageBoxRightButtonClicked);

            fadeForm = FormsManager.GetFormFadeView();       
        }

        private void OnFormLoaded(object sender, System.EventArgs e)
        {
            RenderUI();
        }

        public void RenderUI()
        {
            rateSettingHeaderPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH, 150);
            rateSettingHeaderPanel.Location = new Point(0, 0);

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

            displayRefreshIntervalLabel = new Label();
            displayRefreshIntervalLabel.Width = 280;
            displayRefreshIntervalLabel.Height = 50;
            displayRefreshIntervalLabel.Font = new Font(this.Font.FontFamily, 13);
            displayRefreshIntervalLabel.Location = new Point(50, goBackButton.Location.Y);
            displayRefreshIntervalLabel.Text = "ระยะเวลาสลับหน้าจอ (วินาที)";

            displayRefreshIntervalTextbox = new TextBox();
            displayRefreshIntervalTextbox.Width = 50;
            displayRefreshIntervalTextbox.Height = 50;
            displayRefreshIntervalTextbox.Font = new Font(this.Font.FontFamily, 13);
            displayRefreshIntervalTextbox.Location = new Point(displayRefreshIntervalLabel.Location.X + displayRefreshIntervalLabel.Width, 
                                                                goBackButton.Location.Y);
            displayRefreshIntervalTextbox.MaxLength = 2;
            displayRefreshIntervalTextbox.KeyPress += new KeyPressEventHandler(displayRefreshIntervalTextbox_KeyPress);

            rateSettingContentPanel = new RateSettingContentPanel(SCREEN_WIDTH, goBackButton.Location.Y - 50 - rateSettingHeaderPanel.Height);
            rateSettingContentPanel.Location = new Point(0, rateSettingHeaderPanel.Height);

            Label horizontalLine = new Label();
            horizontalLine.Width = SCREEN_WIDTH;
            horizontalLine.Height = 5;
            horizontalLine.BackColor = rateSettingHeaderPanel.BackColor;
            horizontalLine.Location = new Point(0, rateSettingContentPanel.Location.Y + rateSettingContentPanel.Height + 20);

            this.Controls.Add(displayRefreshIntervalLabel);
            this.Controls.Add(displayRefreshIntervalTextbox);
            this.Controls.Add(rateSettingHeaderPanel);
            this.Controls.Add(rateSettingContentPanel);
            this.Controls.Add(goBackButton);
            this.Controls.Add(saveButton);
            this.Controls.Add(horizontalLine);
        }

        void displayRefreshIntervalTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        public void RefreshUI()
        {
            ExchangeRateDataManager.LoadData();
            rateSettingContentPanel.FillDataIntoPanel();
            rateSettingHeaderPanel.RefreshHeadertext();
            displayRefreshIntervalTextbox.Text = ExchangeRateDataManager.GetDisplayRefreshInterval() + "";
        }

        void SaveCompletedMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            RefreshUI();
            saveCompletedMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void SaveErrorMessageBoxRightButtonClicked(object sender, EventArgs e)
        {
            saveErrorMessageBox.Visible = false;
            fadeForm.Visible = false;
            this.Visible = true;
            this.Enabled = true;
            this.BringToFront();
        }

        void SaveButtonClicked(object sender, EventArgs e)
        {
            String result = SaveEverything();

            fadeForm.Visible = true;
            fadeForm.BringToFront();

            if (result.Equals(SAVE_SUCCESS))
            {
                saveCompletedMessageBox.Visible = true;
                saveCompletedMessageBox.BringToFront();
                saveCompletedMessageBox.Location = new Point(SCREEN_OFFSET_X + (SCREEN_WIDTH - saveCompletedMessageBox.Width) / 2,
                                                            (SCREEN_HEIGHT - saveCompletedMessageBox.Height) / 2);
   
            }
            else if (result.Equals(SAVE_ERROR_INVALID_CHAR))
            {
                saveErrorMessageBox.Visible = true;
                saveErrorMessageBox.BringToFront();
                saveErrorMessageBox.Location = new Point(SCREEN_OFFSET_X + (SCREEN_WIDTH - saveErrorMessageBox.Width) / 2,
                                                         (SCREEN_HEIGHT - saveErrorMessageBox.Height) / 2);
            
            }

        }
        
        String SaveEverything()
        {
            StringBuilder sb = new StringBuilder("");
            RateSettingSingleDataRowPanel[] singleDataRowPanelList = rateSettingContentPanel.singleDataRowPanelList;
            for (int i = 0; i < singleDataRowPanelList.Length; i++)
            { 
                RateSettingSingleDataRowPanel panelObj = singleDataRowPanelList[i];
                if (!panelObj.IsDataFormatCorrect())
                {
                    return SAVE_ERROR_INVALID_CHAR;
                }
                String tmpLine = panelObj.currencyKey + "," + 
                                 (panelObj.shouldDisplayFlag ? "T" : "F") + "," +
                                 (panelObj.shouldDrawBottomLine ? "T" : "F") + ","+
                                 (panelObj.currencyNameLabel.Text.Equals("") ? GlobalConfig.NULL_CURRENCY_NAME : panelObj.currencyNameLabel.Text) + "," +
                                 (panelObj.denomLabel.Text.Equals("") ? GlobalConfig.NULL_DENOM_NAME : panelObj.denomLabel.Text) + "," + 
                                 panelObj.currencyBuyTextBox.Text + "," +
                                 panelObj.currencySellTextBox.Text + "," + 
                                 panelObj.countryName;
                if (i < singleDataRowPanelList.Length - 1) tmpLine += "#";
                sb.AppendLine(tmpLine);
            }

            ExchangeRateDataManager.SaveData(sb.ToString());

            string intervalStr = displayRefreshIntervalTextbox.Text;
            try
            {
                int interval = Int32.Parse(intervalStr);
                ExchangeRateDataManager.SaveDisplayRefreshInterval(interval);
            }
            catch (Exception e)
            {
                ExchangeRateDataManager.SaveDisplayRefreshInterval(GlobalConfig.DEFAULT_DISPLAY_REFRESH_INTERVAL);
            }
            return SAVE_SUCCESS;            
        }

        void GoBackButtonClicked(object sender, EventArgs e)
        {
            FormMainMenu instanceFormMainMenu = FormsManager.GetFormMainMenu();
            instanceFormMainMenu.Visible = true;
            instanceFormMainMenu.Enabled = true;
            instanceFormMainMenu.RefreshUI();
            instanceFormMainMenu.BringToFront();
            this.Visible = false;
        }
    }

    public class RateSettingContentPanel : Panel
    {
        public RateSettingSingleDataRowPanel[] singleDataRowPanelList;
        public int ROW_COUNT_PER_COLUMN = -1;

        public RateSettingContentPanel(int width, int height)
        {
            int rateSettingSingleDataRowWidth = 550;
            int rateSettingSingleDataRowHeight = 35;
            int requiredHeight = ((ExchangeRateDataManager.currencyKeyArr.Count / 2) + 1) * rateSettingSingleDataRowHeight;

            this.AutoScroll = true;
            this.Width = width;
            this.Height = height;
            this.SetAutoScrollMargin(0, requiredHeight - this.Height - 20);
            int gapY = 5;

            singleDataRowPanelList = new RateSettingSingleDataRowPanel[ExchangeRateDataManager.currencyKeyArr.Count];
            ROW_COUNT_PER_COLUMN = (ExchangeRateDataManager.currencyKeyArr.Count / 2) + 1;

            for (int i = 0; i < singleDataRowPanelList.Length; i++)
            {
                RateSettingSingleDataRowPanel rateSettingSingleDataRowPanel = new RateSettingSingleDataRowPanel(rateSettingSingleDataRowWidth, rateSettingSingleDataRowHeight);

                int columnNo = i / ROW_COUNT_PER_COLUMN;
                int rowNo = i % ROW_COUNT_PER_COLUMN;

                int locationX = 0;
                if (columnNo == 0) locationX = 30;
                if (columnNo == 1) locationX = this.Width / 2 + 30;

                rateSettingSingleDataRowPanel.Location = new Point(locationX, 
                                                                   gapY + rateSettingSingleDataRowPanel.Height * rowNo);

                String targetKey = (String)(ExchangeRateDataManager.currencyKeyArr[i]);
                rateSettingSingleDataRowPanel.currencyKey = targetKey;

                singleDataRowPanelList[i] = rateSettingSingleDataRowPanel;

                this.Controls.Add(rateSettingSingleDataRowPanel);
            }
        }

        public void FillDataIntoPanel()
        { 
            for (int i = 0; i < singleDataRowPanelList.Length; i++)
            {
                RateSettingSingleDataRowPanel panelObj = singleDataRowPanelList[i];
                ExchangeRateDataObject dataObj = ExchangeRateDataManager.GetExchangeRateObjectForKey(panelObj.currencyKey);

                panelObj.countryFlagPictureBox.Image = dataObj.shoudlDisplayFlag ? dataObj.countryFlagImage : null;
                panelObj.currencyNameLabel.Text = dataObj.currencyText.Equals(GlobalConfig.NULL_CURRENCY_NAME) ? "" : dataObj.currencyText;
                panelObj.denomLabel.Text = dataObj.denomText.Equals(GlobalConfig.NULL_DENOM_NAME) ? "" : dataObj.denomText;
                panelObj.currencyBuyTextBox.Text = dataObj.buyText;
                panelObj.currencySellTextBox.Text = dataObj.sellText;
                panelObj.shouldDisplayFlag = dataObj.shoudlDisplayFlag;
                panelObj.shouldDrawBottomLine = dataObj.shoudlDrawBottomLine;
                panelObj.countryName = dataObj.countryName;
            }
        }
    }

    public class RateSettingSingleDataRowPanel : Panel
    {
        public PictureBox countryFlagPictureBox;
        public String currencyKey;
        public bool shouldDisplayFlag;
        public bool shouldDrawBottomLine;
        public String countryName;
        public TextBox currencyBuyTextBox;
        public TextBox currencySellTextBox;
        public Label currencyNameLabel;
        public Label denomLabel;
        Label currencyBuyLabel;
        Label currencySellLabel;
        Label verticalLineLabel1;
        Label verticalLineLabel2;

        int gapY = 2;
        int gapX = 7;

        public RateSettingSingleDataRowPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            int fontSize = 10;

            verticalLineLabel1 = new Label();
            verticalLineLabel1.BackColor = Color.Transparent;
            verticalLineLabel1.Width = gapX;
            verticalLineLabel1.Height = this.Height;

            verticalLineLabel2 = new Label();
            verticalLineLabel2.BackColor = verticalLineLabel1.BackColor;
            verticalLineLabel2.Width = verticalLineLabel1.Width;
            verticalLineLabel2.Height = verticalLineLabel1.Height;

            verticalLineLabel1.Location = new Point((int)(this.Width * 0.42), 0);
            verticalLineLabel2.Location = new Point((int)(this.Width * 0.7), 0);

            countryFlagPictureBox = new PictureBox();
            countryFlagPictureBox.Height = this.Height - gapY * 2;
            countryFlagPictureBox.Width = (int)(countryFlagPictureBox.Height * 1.5);
            countryFlagPictureBox.Location = new Point(gapX, gapY);
            countryFlagPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            currencyNameLabel = new Label();
            currencyNameLabel.Width = 50;
            currencyNameLabel.Height = this.Height - gapY * 2;
            currencyNameLabel.Location = new Point(countryFlagPictureBox.Location.X + countryFlagPictureBox.Width + gapX,
                                                    gapY);
            currencyNameLabel.Font = new Font(this.Font.FontFamily, fontSize);
            currencyNameLabel.TextAlign = ContentAlignment.MiddleLeft;

            denomLabel = new Label();
            denomLabel.Width = verticalLineLabel1.Location.X - (currencyNameLabel.Location.X + currencyNameLabel.Width);
            denomLabel.Height = this.Height - gapY * 2;
            denomLabel.Location = new Point(currencyNameLabel.Location.X + currencyNameLabel.Width + gapX,
                                                    gapY);
            denomLabel.Font = new Font(this.Font.FontFamily, fontSize);
            denomLabel.TextAlign = ContentAlignment.MiddleCenter;

            currencyBuyLabel = new Label();
            currencyBuyLabel.ForeColor = Color.Black;
            currencyBuyLabel.Width = (int)((verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX) * 0.4);
            currencyBuyLabel.Height = this.Height - gapY * 2;
            currencyBuyLabel.Location = new Point(verticalLineLabel1.Location.X + gapX * 2, gapY);
            currencyBuyLabel.Font = new Font(this.Font.FontFamily, fontSize);
            currencyBuyLabel.TextAlign = ContentAlignment.MiddleLeft;
            currencyBuyLabel.Text = "BUY";

            currencyBuyTextBox = new TextBox();
            currencyBuyTextBox.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX * 2 - currencyBuyLabel.Width;
            currencyBuyTextBox.Height = currencyBuyLabel.Height;
            currencyBuyTextBox.Location = new Point(currencyBuyLabel.Location.X + currencyBuyLabel.Width , currencyBuyLabel.Location.Y);
            currencyBuyTextBox.TextAlign = HorizontalAlignment.Right;
            currencyBuyTextBox.Font = currencyBuyLabel.Font;

            currencySellLabel = new Label();
            currencySellLabel.ForeColor = Color.Black;
            currencySellLabel.Width = currencyBuyLabel.Width;
            currencySellLabel.Height = currencyBuyLabel.Height;
            currencySellLabel.Location = new Point(verticalLineLabel2.Location.X + gapX, gapY);
            currencySellLabel.Font = currencyBuyLabel.Font;
            currencySellLabel.TextAlign = ContentAlignment.MiddleLeft;
            currencySellLabel.Text = "SELL";

            currencySellTextBox = new TextBox();
            currencySellTextBox.Width = currencyBuyTextBox.Width;
            currencySellTextBox.Height = currencySellLabel.Height;
            currencySellTextBox.Location = new Point(currencySellLabel.Location.X + currencySellLabel.Width, currencySellLabel.Location.Y);
            currencySellTextBox.TextAlign = HorizontalAlignment.Right;
            currencySellTextBox.Font = currencySellLabel.Font;

            this.Controls.Add(verticalLineLabel1);
            this.Controls.Add(verticalLineLabel2);
            this.Controls.Add(countryFlagPictureBox);
            this.Controls.Add(currencyNameLabel);
            this.Controls.Add(denomLabel);
            this.Controls.Add(currencyBuyLabel);
            this.Controls.Add(currencySellLabel);
            this.Controls.Add(currencyBuyTextBox);
            this.Controls.Add(currencySellTextBox);
        }

        public bool IsDataFormatCorrect()
        {
            String buyString = currencyBuyTextBox.Text.Trim();
            String sellString = currencySellTextBox.Text.Trim();
            double d1, d2;

            bool isBuyStringValid = true;
            if (buyString.Contains("#") || buyString.Contains(","))
                isBuyStringValid = false;
            else if (buyString == null || buyString.Equals("") || buyString.Equals("-"))
                isBuyStringValid = true;
            else          
                isBuyStringValid = Double.TryParse(buyString, out d1);
            
            bool isSellStringValid = true;
            if (sellString.Contains("#") || sellString.Contains(","))
                isSellStringValid = false;
            else if (sellString == null || sellString.Equals("") || sellString.Equals("-"))
                isSellStringValid = true;
            else          
                isSellStringValid = Double.TryParse(sellString, out d2);
            
            return isBuyStringValid && isSellStringValid;
        }
    }
}
