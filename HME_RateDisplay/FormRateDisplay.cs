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
        int DISPLAY_INTERVAL = 7;

        public FormRateDisplay()
        {
            InitializeComponent();

            this.Load += new EventHandler(OnFormLoaded);

            rateDisplaySignalClock = new SignalClock(DISPLAY_INTERVAL * 100);
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
            ROW_COUNT_PER_PAGE = (int)(this.Height / (rateDisplaySingleDataRowHeight));
            singleDataRowPanelList = new RateDisplaySingleDataRowPanel[ROW_COUNT_PER_PAGE - 1];

            for (int i = 0; i < ROW_COUNT_PER_PAGE; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = new RateDisplaySingleDataRowPanel(this.Width - gapX * 2, rateDisplaySingleDataRowHeight);
              
                if (i == 0)
                {
                    rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY);
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(true);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyName("Currency");
                    rateDisplaySingleDataRowPanel.SetTextDenom("Denom");
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
                    rateDisplaySingleDataRowPanel.SetTextDenom(dataObj.denomText);
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
        Label denomLabel;
        Label currencyBuyLabel;
        Label currencySellLabel;
        Label verticalLineLabel0;
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

            verticalLineLabel0 = new Label();
            verticalLineLabel0.BackColor = GlobalConfig.RATE_DISPLAY_LINE_COLOR;
            verticalLineLabel0.Width = gapX;
            verticalLineLabel0.Height = this.Height;

            verticalLineLabel1 = new Label();
            verticalLineLabel1.BackColor = verticalLineLabel0.BackColor;
            verticalLineLabel1.Width = verticalLineLabel0.Width;
            verticalLineLabel1.Height = verticalLineLabel0.Height;

            verticalLineLabel2 = new Label();
            verticalLineLabel2.BackColor = verticalLineLabel0.BackColor;
            verticalLineLabel2.Width = verticalLineLabel0.Width;
            verticalLineLabel2.Height = verticalLineLabel0.Height;

            bottomLineLabel = new Label();
            bottomLineLabel.BackColor = GlobalConfig.RATE_DISPLAY_LINE_COLOR;
            bottomLineLabel.Width = this.Width;
            bottomLineLabel.Height = gapY;
            bottomLineLabel.Location = new Point(0, this.Height - gapY);

            verticalLineLabel0.Location = new Point((int)(this.Width * 0.3), 0);
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
            countryNameLabel.Width = (int)(countryFlagPictureBox.Width * 3);
            countryNameLabel.Location = new Point(gapX, countryFlagPictureBox.Location.Y + countryFlagPictureBox.Height + 2);
            countryNameLabel.Font = new Font(this.Font.FontFamily, 12);
            countryNameLabel.TextAlign = ContentAlignment.MiddleLeft;

            currencyNameLabel = new Label();
            currencyNameLabel.Height = this.Height - gapY * 2;
            currencyNameLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);

            denomLabel = new Label();
            denomLabel.Width = verticalLineLabel1.Location.X - verticalLineLabel0.Location.X - verticalLineLabel0.Width - gapX * 3;
            denomLabel.Height = this.Height - gapY * 2;
            denomLabel.Location = new Point(verticalLineLabel0.Location.X + verticalLineLabel0.Width, gapY);
            denomLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);
            denomLabel.TextAlign = ContentAlignment.MiddleCenter;

            currencyBuyLabel = new Label();
            currencyBuyLabel.ForeColor = Color.LawnGreen;
            currencyBuyLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX * 9;
            currencyBuyLabel.Height = this.Height - gapY * 2;
            currencyBuyLabel.Location = new Point(verticalLineLabel1.Location.X + gapX , gapY);
            currencyBuyLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);
            currencyBuyLabel.TextAlign = ContentAlignment.MiddleRight;

            currencySellLabel = new Label();
            currencySellLabel.ForeColor = Color.Yellow;
            currencySellLabel.Width = this.Width - verticalLineLabel2.Location.X - gapX * 9;
            currencySellLabel.Height = this.Height - gapY * 2;
            currencySellLabel.Location = new Point(verticalLineLabel2.Location.X + gapX , gapY);
            currencySellLabel.Font = new Font(this.Font.FontFamily, rateDisplayFontSize);
            currencySellLabel.TextAlign = ContentAlignment.MiddleRight;

            this.Controls.Add(verticalLineLabel0);
            this.Controls.Add(verticalLineLabel1);
            this.Controls.Add(verticalLineLabel2);
            this.Controls.Add(countryFlagPictureBox);
            this.Controls.Add(countryNameLabel);
            this.Controls.Add(denomLabel);
            this.Controls.Add(currencyNameLabel);           
            this.Controls.Add(currencyBuyLabel);
            this.Controls.Add(currencySellLabel);
            this.Controls.Add(bottomLineLabel);
        }

        public void SetIsHeaderRow(bool isHeader)
        {
            currencyBuyLabel.TextAlign = isHeader ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
            currencySellLabel.TextAlign = isHeader ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleRight;
            
            if (isHeader)
            {
                countryNameLabel.Width = 1;
                currencyNameLabel.Location = new Point(0 , gapY);
                currencyNameLabel.Width = verticalLineLabel0.Location.X - gapX * 2;
                currencyNameLabel.TextAlign = ContentAlignment.MiddleCenter;

                Color headerColor = Color.FromArgb(128, 255, 255);
                
                currencyNameLabel.ForeColor = headerColor;
                denomLabel.ForeColor = headerColor;
                currencyBuyLabel.ForeColor = headerColor;
                currencySellLabel.ForeColor = headerColor;

                currencyBuyLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX * 2;
                currencySellLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX * 2;
            }
            else 
            {
                currencyNameLabel.Location = new Point(countryNameLabel.Location.X + countryNameLabel.Width + gapX,
                                            gapY);
                currencyNameLabel.Width = verticalLineLabel0.Location.X - (countryNameLabel.Location.X + countryNameLabel.Width + gapX);
                currencyNameLabel.TextAlign = ContentAlignment.MiddleRight;
                currencyNameLabel.ForeColor = Color.White;        
                denomLabel.ForeColor = Color.White;
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
            if (text == null)
            {
                this.currencyNameLabel.Text = "";
            }
            else
            {
                this.currencyNameLabel.Text = text.Equals(GlobalConfig.NULL_CURRENCY_NAME) ? null : text;
            }
        }

        public void SetTextDenom(String text)
        {
            if (text == null)
            {
                this.denomLabel.Text = "";
            }
            else
            {
                this.denomLabel.Text = text.Equals(GlobalConfig.NULL_DENOM_NAME) ? "-" : text;
            }
        }
          

        public void ClearAllData()
        {
            this.countryFlagPictureBox.Image = null;
            this.currencyNameLabel.Text = null;
            this.denomLabel.Text = null;
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
            headerTextLabel1.ForeColor = Color.Yellow;
            headerTextLabel1.Width = (int)(width * 0.85);
            headerTextLabel1.Height = 50;
            headerTextLabel1.TextAlign = ContentAlignment.MiddleCenter;
            headerTextLabel1.Font = new Font(this.Font.FontFamily, 22);
            headerTextLabel1.Location = new Point((this.Width - headerTextLabel1.Width) / 2 , 20);

            headerTextLabel2 = new Label();
            headerTextLabel2.ForeColor = Color.White;
            headerTextLabel2.Width = headerTextLabel1.Width;
            headerTextLabel2.Height = 30;
            headerTextLabel2.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel2.Font = new Font(this.Font.FontFamily, 16);
            headerTextLabel2.Location = new Point(headerTextLabel1.Location.X, headerTextLabel1.Location.Y + headerTextLabel1.Height + gapY);//new Point((this.Width - headerTextLabel1.Width) / 2, (this.Height - headerTextLabel1.Height) / 2);

            headerTextLabel3 = new Label();
            headerTextLabel3.ForeColor = Color.White;
            headerTextLabel3.Width = headerTextLabel1.Width;
            headerTextLabel3.Height = 30;
            headerTextLabel3.TextAlign = ContentAlignment.MiddleLeft;
            headerTextLabel3.Font = new Font(this.Font.FontFamily, 16);
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
                headerTextLabel2.Text = "EXCHANGE RATE";
                headerTextLabel3.Text = "Date : " + dateString + 
                                        "                                Last Updated : " + ExchangeRateDataManager.GetUpdatedTimeString();      
            }
        }
    }
}
