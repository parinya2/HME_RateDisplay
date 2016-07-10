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
        
        }
    }

    public class RateDisplayContentPanel : Panel
    {
        public RateDisplayContentPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Gray;

            int gapY = 7;
            int gapX = 7;
            for (int i = 0; i < 5; i++)
            {
                RateDisplaySingleDataRowPanel rateDisplaySingleDataRowPanel = new RateDisplaySingleDataRowPanel(this.Width - gapX * 2 , 100);
                rateDisplaySingleDataRowPanel.Location = new Point(gapX, gapY + (rateDisplaySingleDataRowPanel.Height + gapY) * i);
                rateDisplaySingleDataRowPanel.SetTextCurrencyName("USD");
                rateDisplaySingleDataRowPanel.SetTextCurrencyBuy("35");
                rateDisplaySingleDataRowPanel.SetTextCurrencySell("36");

                this.Controls.Add(rateDisplaySingleDataRowPanel);
            }
        }
    }

    public class RateDisplaySingleDataRowPanel : Panel
    {
        PictureBox countryFlagImage;
        Label currencyNameLabel;
        Label currencyBuyLabel;
        Label currencySellLabel;
        Label verticalLineLabel1;
        Label verticalLineLabel2;

        public RateDisplaySingleDataRowPanel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.Black;

            int gapY = 7;
            int gapX = 7;

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

            countryFlagImage = new PictureBox();
            countryFlagImage.Height = this.Height - gapY * 2;
            countryFlagImage.Width = (int)(countryFlagImage.Height * 1.5);
            countryFlagImage.Location = new Point(gapX, gapY);
            countryFlagImage.SizeMode = PictureBoxSizeMode.Zoom;

            currencyNameLabel = new Label();
            currencyNameLabel.ForeColor = Color.White;
            currencyNameLabel.Width = verticalLineLabel1.Location.X - countryFlagImage.Width;
            currencyNameLabel.Height = this.Height - gapY * 2;
            currencyNameLabel.Location = new Point(countryFlagImage.Location.X + countryFlagImage.Width + gapX ,
                                                    gapY);
            currencyNameLabel.Font = new Font(this.Font.FontFamily, 40);

            currencyBuyLabel = new Label();
            currencyBuyLabel.ForeColor = Color.LawnGreen;
            currencyBuyLabel.Width = verticalLineLabel2.Location.X - verticalLineLabel1.Location.X - gapX;
            currencyBuyLabel.Height = this.Height - gapY * 2;
            currencyBuyLabel.Location = new Point(verticalLineLabel1.Location.X + gapX , gapY);
            currencyBuyLabel.Font = new Font(this.Font.FontFamily, 50);
            currencyBuyLabel.TextAlign = ContentAlignment.MiddleRight;

            currencySellLabel = new Label();
            currencySellLabel.ForeColor = Color.Yellow;
            currencySellLabel.Width = this.Width - verticalLineLabel2.Location.X - gapX;
            currencySellLabel.Height = this.Height - gapY * 2;
            currencySellLabel.Location = new Point(verticalLineLabel2.Location.X + gapX , gapY);
            currencySellLabel.Font = new Font(this.Font.FontFamily, 50);
            currencySellLabel.TextAlign = ContentAlignment.MiddleRight;

            this.Controls.Add(verticalLineLabel1);
            this.Controls.Add(verticalLineLabel2);
            this.Controls.Add(countryFlagImage);
            this.Controls.Add(currencyNameLabel);
            this.Controls.Add(currencyBuyLabel);
            this.Controls.Add(currencySellLabel);
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
