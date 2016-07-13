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
            ExchangeRateDataManager.InitInstance();

            RenderUI();

            rateDisplaySignalClock = new SignalClock(200);
            rateDisplaySignalClock.TheTimeChanged += new SignalClock.SignalClockTickHandler(RateDisplaySignalClockHasChanged);
        }

        public void RenderUI()
        {
            rateDisplayHeaderPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH, (int)(SCREEN_HEIGHT * 0.3));
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
            rateDisplayContentPanel.FillDataIntoPanel(4,5);
        }

        protected void RateDisplaySignalClockHasChanged(int state)
        {
            int totalCurrencyCount = rateDisplayContentPanel.currencyKeyArr.Length;
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
        public String[] currencyKeyArr = { "USD1", "USD2", "USD3", "USD4", "USD5", 
                                           "EUR1", "EUR2", "EUR3", "GBP1", "GBP2", 
                                           "AUD", "CNY", "JPY1", "JPY2", "SGD1", "SGD2", 
                                           "MYR1", "MYR2", "MYR3", "TWD1", "TWD2",
                                           "KRW1", "KRW2", "HKD" };
        int ROW_COUNT_PER_PAGE = -1;

        public RateDisplayContentPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Gray;

            int gapY = 7;
            int gapX = 7;
            int rateDisplaySingleDataRowHeight = 100;
            ROW_COUNT_PER_PAGE = (int)(this.Height / (rateDisplaySingleDataRowHeight + gapY));
            singleDataRowPanelList = new RateDisplaySingleDataRowPanel[ROW_COUNT_PER_PAGE - 1];

            for (int i = 0; i < ROW_COUNT_PER_PAGE; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = new RateDisplaySingleDataRowPanel(this.Width - gapX * 2, rateDisplaySingleDataRowHeight);
                rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY + (rateDisplaySingleDataRowPanel.Height + gapY) * i);

                if (i == 0)
                {
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(true);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyName("Currency");
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy("Buy");
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell("Sell");
                    rateDisplaySingleDataRowPanel.SetCurrencyImage(null);
                }
                else
                {
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
                    String targetKey = currencyKeyArr[targetKeyIndex];
                    ExchangeRateDataObject dataObj = ExchangeRateDataManager.GetExchangeRateObjectForKey(targetKey);

                    rateDisplaySingleDataRowPanel.SetTextCurrencyName(dataObj.currencyText);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy(dataObj.buyText);
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell(dataObj.sellText);
                    rateDisplaySingleDataRowPanel.SetCurrencyImage(dataObj.shoudlDisplayFlag ? dataObj.countryFlagImage : null);
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
        Label currencyNameLabel;
        Label currencyBuyLabel;
        Label currencySellLabel;
        Label verticalLineLabel1;
        Label verticalLineLabel2;

        int gapY = 7;
        int gapX = 7;

        public RateDisplaySingleDataRowPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Black;

            verticalLineLabel1 = new Label();
            verticalLineLabel1.BackColor = Color.Gray;
            verticalLineLabel1.Width = gapX;
            verticalLineLabel1.Height = this.Height;

            verticalLineLabel2 = new Label();
            verticalLineLabel2.BackColor = verticalLineLabel1.BackColor;
            verticalLineLabel2.Width = verticalLineLabel1.Width;
            verticalLineLabel2.Height = verticalLineLabel1.Height;

            verticalLineLabel1.Location = new Point((int)(this.Width * 0.6) , 0);
            verticalLineLabel2.Location = new Point((int)(this.Width * 0.8) , 0);

            countryFlagPictureBox = new PictureBox();
            countryFlagPictureBox.Height = this.Height - gapY * 2;
            countryFlagPictureBox.Width = (int)(countryFlagPictureBox.Height * 1.5);
            countryFlagPictureBox.Location = new Point(gapX, gapY);
            countryFlagPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            currencyNameLabel = new Label();
            currencyNameLabel.ForeColor = Color.White;
            currencyNameLabel.Width = verticalLineLabel1.Location.X - countryFlagPictureBox.Width;
            currencyNameLabel.Height = this.Height - gapY * 2;
            currencyNameLabel.Location = new Point(countryFlagPictureBox.Location.X + countryFlagPictureBox.Width + gapX ,
                                                    gapY);
            currencyNameLabel.Font = new Font(this.Font.FontFamily, 40);

            currencyBuyLabel = new Label();
            currencyBuyLabel.ForeColor = Color.LawnGreen;
            currencyBuyLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX;
            currencyBuyLabel.Height = this.Height - gapY * 2;
            currencyBuyLabel.Location = new Point(verticalLineLabel1.Location.X + gapX , gapY);
            currencyBuyLabel.Font = new Font(this.Font.FontFamily, 40);
            currencyBuyLabel.TextAlign = ContentAlignment.MiddleRight;

            currencySellLabel = new Label();
            currencySellLabel.ForeColor = Color.Yellow;
            currencySellLabel.Width = this.Width - verticalLineLabel2.Location.X - gapX;
            currencySellLabel.Height = this.Height - gapY * 2;
            currencySellLabel.Location = new Point(verticalLineLabel2.Location.X + gapX , gapY);
            currencySellLabel.Font = new Font(this.Font.FontFamily, 40);
            currencySellLabel.TextAlign = ContentAlignment.MiddleRight;

            this.Controls.Add(verticalLineLabel1);
            this.Controls.Add(verticalLineLabel2);
            this.Controls.Add(countryFlagPictureBox);
            this.Controls.Add(currencyNameLabel);
            this.Controls.Add(currencyBuyLabel);
            this.Controls.Add(currencySellLabel);
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
                currencyNameLabel.Location = new Point(countryFlagPictureBox.Location.X + countryFlagPictureBox.Width + gapX,
                                            gapY);
                currencyNameLabel.TextAlign = ContentAlignment.MiddleLeft;
                currencyNameLabel.ForeColor = Color.White;
                currencyBuyLabel.ForeColor = Color.LawnGreen;
                currencySellLabel.ForeColor = Color.Yellow;
            }
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
        }
    }

    public class RateDisplayHeaderPanel : Panel
    {
        public RateDisplayHeaderPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.DarkBlue;

            Label headerTextLabel = new Label();
            headerTextLabel.ForeColor = Color.White;
            headerTextLabel.Width = (int)(width * 0.85);
            headerTextLabel.Height = 100;
            headerTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            headerTextLabel.Font = new Font(this.Font.FontFamily, 50);
            headerTextLabel.Text = "Hatyai  Money  Exchange";
            headerTextLabel.Location = new Point((this.Width - headerTextLabel.Width) / 2 , (this.Height - headerTextLabel.Height) / 2);

            this.Controls.Add(headerTextLabel);
        }
    }
}
