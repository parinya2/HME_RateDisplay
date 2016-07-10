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
        public FormRateDisplay()
        {
            InitializeComponent();

            RenderUI();
        }

        public void RenderUI()
        {
            RateDisplayHeaderPanel rateDisplayHeaderPanel = new RateDisplayHeaderPanel(SCREEN_WIDTH, (int)(SCREEN_HEIGHT * 0.3));
            rateDisplayHeaderPanel.Location = new Point(0 , 0);
            
            RateDisplayContentPanel rateDisplayContentPanel = new RateDisplayContentPanel(SCREEN_WIDTH, SCREEN_HEIGHT - rateDisplayHeaderPanel.Height);
            rateDisplayContentPanel.Location = new Point(0, rateDisplayHeaderPanel.Height);

            this.Controls.Add(rateDisplayHeaderPanel);
            this.Controls.Add(rateDisplayContentPanel);
        }

        public void RefreshUI()
        {
            ExchangeRateDataManager.LoadData();
        }
    }

    public class RateDisplayContentPanel : Panel
    {
        RateDisplaySingleDataRowPanel[] singleDataRowPanelList;

        public RateDisplayContentPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Gray;

            int gapY = 7;
            int gapX = 7;
            int rateDisplaySingleDataRowHeight = 100;
            int rowCountPerPage = (int)(this.Height / (rateDisplaySingleDataRowHeight + gapY));
            singleDataRowPanelList = new RateDisplaySingleDataRowPanel[rowCountPerPage - 1];
           
            for (int i = 0; i < rowCountPerPage; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = new RateDisplaySingleDataRowPanel(this.Width - gapX * 2, rateDisplaySingleDataRowHeight);
                rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY + (rateDisplaySingleDataRowPanel.Height + gapY) * i);

                if (i == 0)
                {
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(true);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyName("Currency");
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy("Buy");
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell("Sell");
                }
                else
                {
                    rateDisplaySingleDataRowPanel.SetIsHeaderRow(false);
                    rateDisplaySingleDataRowPanel.SetTextCurrencyName("USD");
                    rateDisplaySingleDataRowPanel.SetTextCurrencyBuy("35.12");
                    rateDisplaySingleDataRowPanel.SetTextCurrencySell("36.32");

                    singleDataRowPanelList[i - 1] = rateDisplaySingleDataRowPanel;
                }


                this.Controls.Add(rateDisplaySingleDataRowPanel);
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
            headerTextLabel.Location = new Point((this.Width - headerTextLabel.Width) / 2 , 100);

            this.Controls.Add(headerTextLabel);
        }
    }
}
