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
    public partial class FormRateDisplay : FixedSizeForm
    {
        RateDisplayHeaderPanel rateDisplayHeaderPanel;
        RateDisplayContentPanel rateDisplayContentPanel;
        SignalClock rateDisplaySignalClock;
        int currentStartIndex = -1;
        int currentStopIndex = -1;
        Button goBackButton;

        public FormRateDisplay()
        {
            InitializeComponent();

            this.Load += new EventHandler(OnFormLoaded);

            rateDisplaySignalClock = new SignalClock(200);
            rateDisplaySignalClock.TheTimeChanged += new SignalClock.SignalClockTickHandler(RateDisplaySignalClockHasChanged);
        }

        private void OnFormLoaded(object sender, System.EventArgs e)
        {
            RenderUI();
        }

        public void RenderUI()
        {
            rateDisplayHeaderPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH, 150);
            rateDisplayHeaderPanel.Location = new Point(0 , 0);
            
            rateDisplayContentPanel = new RateDisplayContentPanel(SCREEN_WIDTH, SCREEN_HEIGHT - rateDisplayHeaderPanel.Height);
            rateDisplayContentPanel.Location = new Point(0, rateDisplayHeaderPanel.Height);

            goBackButton = new Button();
            goBackButton.BackColor = Color.White;
            goBackButton.Width = 20;
            goBackButton.Height = 20;
            goBackButton.Location = new Point(0,0);
            goBackButton.Click += new EventHandler(GoBackButtonClicked);

            rateDisplayHeaderPanel.Controls.Add(goBackButton);

            this.Controls.Add(rateDisplayHeaderPanel);
            this.Controls.Add(rateDisplayContentPanel);
        }

        void GoBackButtonClicked(object sender, EventArgs e)
        {
            FormMainMenu instanceFormMainMenu = FormsManager.GetFormMainMenu();
            instanceFormMainMenu.Visible = true;
            instanceFormMainMenu.Enabled = true;
           
            instanceFormMainMenu.BringToFront();
            this.Visible = false;
        }

        public void RefreshUI()
        {
            ExchangeRateDataManager.LoadData();

            rateDisplayContentPanel.FillDataIntoPanel(0, 3);
            rateDisplayHeaderPanel.RefreshHeadertext();
        }

        protected void RateDisplaySignalClockHasChanged(int state)
        {
            int totalCurrencyCount = ExchangeRateDataManager.currencyKeyArr.Count;
            int totalContentRowCount = rateDisplayContentPanel.singleDataRowPanelList.Length;
            if (state == 1)
            {
                if (currentStartIndex == -1 && currentStopIndex == -1)
                {
                    currentStartIndex = 0;
                    currentStopIndex = (totalCurrencyCount < totalContentRowCount) ? (totalCurrencyCount - 1) : (totalContentRowCount - 1);
                }
                else
                {
                    if (currentStopIndex == totalCurrencyCount - 1)
                    {
                        currentStartIndex = 0;
                        currentStopIndex = (totalCurrencyCount < totalContentRowCount) ? (totalCurrencyCount - 1) : (totalContentRowCount - 1);
                    }
                    else
                    {
                        currentStartIndex = currentStopIndex + 1;
                        currentStopIndex = (currentStartIndex + totalContentRowCount - 1) <= (totalCurrencyCount - 1) ?
                                            currentStartIndex + totalContentRowCount - 1 :
                                            totalCurrencyCount - 1;
                    }
                }
                this.rateDisplayContentPanel.FillDataIntoPanel(currentStartIndex, currentStopIndex);
            }           
        }
    }

    public class RateDisplayContentPanel : Panel
    {
        public RateDisplaySingleDataRowPanel[] singleDataRowPanelList;        
        int ROW_COUNT_PER_PAGE = -1;

        public RateDisplayContentPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Gray;

            int gapY = GlobalConfig.RATE_DISPLAY_GAP_Y;
            int gapX = GlobalConfig.RATE_DISPLAY_GAP_X;
            int rateDisplaySingleDataRowHeight = 80;
            ROW_COUNT_PER_PAGE = (int)(this.Height / (rateDisplaySingleDataRowHeight + gapY));
            singleDataRowPanelList = new RateDisplaySingleDataRowPanel[ROW_COUNT_PER_PAGE - 1];

            for (int i = 0; i < ROW_COUNT_PER_PAGE; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = new RateDisplaySingleDataRowPanel(this.Width - gapX * 2, rateDisplaySingleDataRowHeight);
                rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY + (rateDisplaySingleDataRowPanel.Height + gapY) * i);

                if (i == 0)
                {
                    rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY);
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(true);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyName("Currency");
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy("Buy");
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell("Sell");
                    rateDisplaySingleDataRowPanel.SetCurrencyImage(null);
                    rateDisplaySingleDataRowPanel.SetShouldDrawBottomLine(false);
                    rateDisplaySingleDataRowPanel.SetCountryName(GlobalConfig.NULL_COUNTRY_NAME);
                }
                else
                {
                    rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY * 2 +  rateDisplaySingleDataRowPanel.Height * i);
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(false);
                    singleDataRowPanelList[i - 1] = rateDisplaySingleDataRowPanel;
                }

                this.Controls.Add(rateDisplaySingleDataRowPanel);
            }
        }

        public void FillDataIntoPanel(int startIndex, int stopIndex)
        {
            int count = stopIndex - startIndex + 1;
            for (int i = 0; i < singleDataRowPanelList.Length; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = singleDataRowPanelList[i];

                int targetKeyIndex = startIndex + i;
                if (targetKeyIndex <= stopIndex)
                {
                    String targetKey = (String)(ExchangeRateDataManager.currencyKeyArr[targetKeyIndex]);
                    ExchangeRateDataObject dataObj = ExchangeRateDataManager.GetExchangeRateObjectForKey(targetKey);

                    rateDisplaySingleDataRowPanel.SetTextCurrencyName(dataObj.currencyText);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy(dataObj.buyText);
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell(dataObj.sellText);
                    rateDisplaySingleDataRowPanel.SetCurrencyImage(dataObj.shoudlDisplayFlag ? dataObj.countryFlagImage : null);
                    rateDisplaySingleDataRowPanel.SetShouldDrawBottomLine(dataObj.shoudlDrawBottomLine);
                    rateDisplaySingleDataRowPanel.SetCountryName(dataObj.countryName);
                }
                else
                {
                    rateDisplaySingleDataRowPanel.ClearAllData();
                }
            }
        }
    }

    public class RateDisplaySingleDataRowPanel : Panel
    {
        PictureBox countryFlagPictureBox;
        Label countryNameLabel;
        Label currencyNameLabel;
        Label currencyBuyLabel;
        Label currencySellLabel;
        Label verticalLineLabel1;
        Label verticalLineLabel2;
        Label bottomLineLabel;

        int gapY = GlobalConfig.RATE_DISPLAY_GAP_Y;
        int gapX = GlobalConfig.RATE_DISPLAY_GAP_X;

        int rateDisplayFontSize = 27;

        public RateDisplaySingleDataRowPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Black;

            verticalLineLabel1 = new Label();
            verticalLineLabel1.BackColor = GlobalConfig.RATE_DISPLAY_LINE_COLOR;
            verticalLineLabel1.Width = gapX;
            verticalLineLabel1.Height = this.Height;

            verticalLineLabel2 = new Label();
            verticalLineLabel2.BackColor = verticalLineLabel1.BackColor;
            verticalLineLabel2.Width = verticalLineLabel1.Width;
            verticalLineLabel2.Height = verticalLineLabel1.Height;

            bottomLineLabel = new Label();
            bottomLineLabel.BackColor = GlobalConfig.RATE_DISPLAY_LINE_COLOR;
            bottomLineLabel.Width = this.Width;
            bottomLineLabel.Height = gapY;
            bottomLineLabel.Location = new Point(0, this.Height - gapY);

            verticalLineLabel1.Location = new Point((int)(this.Width * 0.6) , 0);
            verticalLineLabel2.Location = new Point((int)(this.Width * 0.8) , 0);

            int countryNameLabelHeight = 30;

            countryFlagPictureBox = new PictureBox();
            countryFlagPictureBox.Height = this.Height - gapY * 3 - countryNameLabelHeight;
            countryFlagPictureBox.Width = (int)(countryFlagPictureBox.Height * 1.5);
            countryFlagPictureBox.Location = new Point(gapX, gapY);
            countryFlagPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            countryNameLabel = new Label();
            countryNameLabel.ForeColor = Color.White;
            countryNameLabel.Height = countryNameLabelHeight;
            countryNameLabel.Width = countryFlagPictureBox.Width * 4;
            countryNameLabel.Location = new Point(gapX, countryFlagPictureBox.Location.Y + countryFlagPictureBox.Height + 2);
            countryNameLabel.Font = new Font(this.Font.FontFamily, 12);
            countryNameLabel.TextAlign = ContentAlignment.MiddleLeft;

            currencyNameLabel = new Label();
            currencyNameLabel.Width = verticalLineLabel1.Location.X - countryNameLabel.Width;
            currencyNameLabel.Height = this.Height - gapY * 2;
            currencyNameLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);

            currencyBuyLabel = new Label();
            currencyBuyLabel.ForeColor = Color.LawnGreen;
            currencyBuyLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX;
            currencyBuyLabel.Height = this.Height - gapY * 2;
            currencyBuyLabel.Location = new Point(verticalLineLabel1.Location.X + gapX , gapY);
            currencyBuyLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);
            currencyBuyLabel.TextAlign = ContentAlignment.MiddleRight;

            currencySellLabel = new Label();
            currencySellLabel.ForeColor = Color.Yellow;
            currencySellLabel.Width = this.Width - verticalLineLabel2.Location.X - gapX;
            currencySellLabel.Height = this.Height - gapY * 2;
            currencySellLabel.Location = new Point(verticalLineLabel2.Location.X + gapX , gapY);
            currencySellLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);
            currencySellLabel.TextAlign = ContentAlignment.MiddleRight;

            
            this.Controls.Add(verticalLineLabel1);
            this.Controls.Add(verticalLineLabel2);
            this.Controls.Add(countryFlagPictureBox);
            this.Controls.Add(countryNameLabel);
            this.Controls.Add(currencyNameLabel);           
            this.Controls.Add(currencyBuyLabel);
            this.Controls.Add(currencySellLabel);
            this.Controls.Add(bottomLineLabel);
        }

        public void SetIsHeaderRow(bool flag)
        {
            currencyBuyLabel.TextAlign = flag ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
            currencySellLabel.TextAlign = flag ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
            
            if (flag)
            {
                currencyNameLabel.Location = new Point((verticalLineLabel1.Location.X - currencyNameLabel.Width) / 2 , gapY);
                currencyNameLabel.TextAlign = ContentAlignment.MiddleCenter;
                currencyNameLabel.ForeColor = Color.PowderBlue;
                currencyBuyLabel.ForeColor = Color.PowderBlue;
                currencySellLabel.ForeColor = Color.PowderBlue;
            }
            else 
            {
                currencyNameLabel.Location = new Point(countryNameLabel.Location.X + countryNameLabel.Width + gapX,
                                            gapY);
                currencyNameLabel.TextAlign = ContentAlignment.MiddleLeft;
                currencyNameLabel.ForeColor = Color.White;
                currencyBuyLabel.ForeColor = Color.LawnGreen;
                currencySellLabel.ForeColor = Color.Yellow;
            }
        }

        public void SetCountryName(string text)
        {
            if (text == null)
            {
                this.countryNameLabel.Text = "";
            }
            else
            {
                this.countryNameLabel.Text = text.Equals(GlobalConfig.NULL_COUNTRY_NAME) ? null : text;
            }
            
        }

        public void SetShouldDrawBottomLine(bool flag)
        {
            this.bottomLineLabel.BackColor = flag ? GlobalConfig.RATE_DISPLAY_LINE_COLOR : Color.Black;
        }

        public void SetCurrencyImage(Bitmap image)
        {
            this.countryFlagPictureBox.Image = image;
        }

        public void SetTextCurrencyBuy(String text)
        {
            this.currencyBuyLabel.Text = text;
        }

        public void SetTextCurrencySell(String text)
        {
            this.currencySellLabel.Text = text;
        }

        public void SetTextCurrencyName(String text)
        {
            this.currencyNameLabel.Text = text;
        }

        public void ClearAllData()
        {
            this.countryFlagPictureBox.Image = null;
            this.currencyNameLabel.Text = null;
            this.currencyBuyLabel.Text = null;
            this.currencySellLabel.Text = null;
            this.countryNameLabel.Text = null;
        }
    }

    public class RateDisplayHeaderPanel : Panel
    {
        Label headerTextLabel1;
        Label headerTextLabel2;
        Label headerTextLabel3;
        int gapY = 3;
        public RateDisplayHeaderPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.DarkBlue;

            headerTextLabel1 = new Label();
            headerTextLabel1.ForeColor = Color.White;
            headerTextLabel1.Width = (int)(width * 0.85);
            headerTextLabel1.Height = 30;
            headerTextLabel1.TextAlign = ContentAlignment.MiddleCenter;
            headerTextLabel1.Font = new Font(this.Font.FontFamily, 16);
            headerTextLabel1.Location = new Point((this.Width - headerTextLabel1.Width) / 2 , 20);

            headerTextLabel2 = new Label();
            headerTextLabel2.ForeColor = headerTextLabel1.ForeColor;
            headerTextLabel2.Width = headerTextLabel1.Width;
            headerTextLabel2.Height = headerTextLabel1.Height;
            headerTextLabel2.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel2.Font = headerTextLabel1.Font;
            headerTextLabel2.Location = new Point(headerTextLabel1.Location.X, headerTextLabel1.Location.Y + headerTextLabel1.Height + gapY);//new Point((this.Width - headerTextLabel1.Width) / 2, (this.Height - headerTextLabel1.Height) / 2);

            headerTextLabel3 = new Label();
            headerTextLabel3.ForeColor = headerTextLabel1.ForeColor;
            headerTextLabel3.Width = headerTextLabel1.Width;
            headerTextLabel3.Height = headerTextLabel1.Height;
            headerTextLabel3.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel3.Font = headerTextLabel1.Font;
            headerTextLabel3.Location = new Point(headerTextLabel1.Location.X, headerTextLabel2.Location.Y + headerTextLabel2.Height + gapY);//new Point((this.Width - headerTextLabel1.Width) / 2, (this.Height - headerTextLabel1.Height) / 2);
           

            RefreshHeadertext();

            this.Controls.Add(headerTextLabel1);
            this.Controls.Add(headerTextLabel2);
            this.Controls.Add(headerTextLabel3);
        }

        public void RefreshHeadertext()
        {
            
            DateTime date = DateTime.Now;
            string dateString = date.ToString("dd / MM / yyyy");

            if (GlobalConfig.IS_RATE_SETTER_MODE)
            {
                headerTextLabel1.Text = "โปรแกรมนี้ใช้สำหรับตั้งค่า Rate Exchange เท่านั้น";
                headerTextLabel2.Text = "Date : " + dateString + "    Last Updated : " + ExchangeRateDataManager.GetUpdatedTimeString();      
            }
            else
            {
                headerTextLabel1.Text = "HATYAI  EXCHANGE Co., Ltd.";
                headerTextLabel2.Text = "CURRENCY EXCHANGE RATE";
                headerTextLabel3.Text = "Date : " + dateString + 
                                        "                                Last Updated : " + ExchangeRateDataManager.GetUpdatedTimeString();      
            }
        }
    }
}
