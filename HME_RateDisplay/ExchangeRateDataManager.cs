﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections;

namespace HME_RateDisplay
{
    public class ExchangeRateDataManager
    {
        private static ExchangeRateDataManager instance;
        private Dictionary<string, ExchangeRateDataObject> exchangeRateDataObjectDict;
        public static ArrayList currencyKeyArr;
        private string updatedTimeString;
        private string updatedDateString;
        private int rowCountPerPageMagicNumber = Int32.MaxValue;
        private int displayRefreshInterval = Int32.MaxValue;

        public ExchangeRateDataManager()
        {
            exchangeRateDataObjectDict = new Dictionary<string, ExchangeRateDataObject>();
        }

        public static ExchangeRateDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExchangeRateDataManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new ExchangeRateDataManager();
            }
        }

        public static ExchangeRateDataObject GetExchangeRateObjectForKey(String key)
        { 
            return instance.exchangeRateDataObjectDict[key];
        }

        public static void SetExchangeRateObject(String key, ExchangeRateDataObject obj)
        {
            if (instance.exchangeRateDataObjectDict.ContainsKey(key))
            {
                instance.exchangeRateDataObjectDict.Remove(key);
            }
            instance.exchangeRateDataObjectDict.Add(key, obj);
        }

        public static string GetUpdatedDateString()
        {
            return instance.updatedDateString;
        }

        public static void SetUpdatedDateString(string text)
        {
            instance.updatedDateString = text;
        }

        public static int GetRowCountPerPageMagicNumber()
        {
            if (instance.rowCountPerPageMagicNumber == Int32.MaxValue)
            { 
                string tmp = File.ReadAllText(Util.GetRowCounPerPageMagicNumberFilePath());
                try 
                {
                    instance.rowCountPerPageMagicNumber = Int32.Parse(tmp);
                }
                catch(Exception e)
                {
                    instance.rowCountPerPageMagicNumber = 0;
                }            
            }
            return instance.rowCountPerPageMagicNumber;
        }

        public static int GetDisplayRefreshInterval()
        {
            string tmp = File.ReadAllText(Util.GetDisplayRefreshIntervalFilePath());
            try
            {
                instance.displayRefreshInterval = Int32.Parse(tmp);
            }
            catch (Exception e)
            {
                instance.displayRefreshInterval = GlobalConfig.DEFAULT_DISPLAY_REFRESH_INTERVAL;
            }
            
            return instance.displayRefreshInterval;
        }

        public static void SetDisplayRefreshInterval(int interval)
        {
            instance.displayRefreshInterval = interval;
        }

        public static string GetUpdatedTimeString()
        {
            return instance.updatedTimeString;
        }

        public static void SetUpdatedTimeString(string text)
        {
            instance.updatedTimeString = text;
        }

        public static void SaveDisplayRefreshInterval(int interval)
        {
            File.WriteAllText(Util.GetDisplayRefreshIntervalFilePath(), interval + "");
        }

        public static void SaveData(String data)
        {
            File.WriteAllText(Util.GetCurrencyFilePath(), data);

            DateTime date = DateTime.Now;
            string fullDateStr = date.ToString("dd/MM/yyyy HH:mm:ss");
            File.WriteAllText(Util.GetLastUpdatedFilePath(), fullDateStr);
        }

        public static void CreateDailyReport()
        {
            LoadData();

            StringBuilder sb = new StringBuilder("");
            foreach (string key in currencyKeyArr)
            {
                ExchangeRateDataObject dataObj = GetExchangeRateObjectForKey(key);
                string currencyText = dataObj.currencyText.Equals(GlobalConfig.NULL_CURRENCY_NAME) ? "" : dataObj.currencyText;
                string denomText = dataObj.denomText.Equals(GlobalConfig.NULL_DENOM_NAME) ? "" : dataObj.denomText;
                string countryText = dataObj.countryName.Equals(GlobalConfig.NULL_COUNTRY_NAME) ? "" : dataObj.countryName;
                string tabString = denomText.Length > 7 ? "\t" : "\t\t";              
                
                string tmp = currencyText + "\t" + 
                             denomText + tabString + 
                             "Buy = " + dataObj.buyText + "\t" +
                             "Sell = " + dataObj.sellText + "\t" +
                             countryText;
                sb.Append(tmp);
                sb.Append(Environment.NewLine);
            }

            File.WriteAllText(Util.GetDailyReportPath(), sb.ToString());
        }

        public static void LoadData()
        {
            string dateTimeText = "" + File.ReadAllText(Util.GetLastUpdatedFilePath(), Encoding.UTF8);
            string[] dateTimeArr = dateTimeText.Split(' ');
            if (dateTimeArr.Length == 2)
            {
                SetUpdatedDateString(dateTimeArr[0]);
                SetUpdatedTimeString(dateTimeArr[1]);
            }

            currencyKeyArr = new ArrayList();
            string allText = "" + File.ReadAllText(Util.GetCurrencyFilePath(), Encoding.UTF8);
            allText = allText.Trim();
            char separatorLv1 = '#';
            char separatorLv2 = ',';

            String[] resultRowArr;
            if (allText.Contains(separatorLv1))
            {
                resultRowArr = allText.Split(separatorLv1);
            }
            else
            {
                resultRowArr = new string[1];
                resultRowArr[0] = allText;
            }

            for (int i = 0; i < resultRowArr.Length; i++)
            {
                String text = resultRowArr[i].Trim();
                String[] tmpArr = text.Split(separatorLv2);
                if (tmpArr.Length == 8)
                {
                    String currencyKey = tmpArr[0];
                    bool shouldDisplayFlag = tmpArr[1].Equals("T");
                    bool shouldDrawBottomLine = tmpArr[2].Equals("T");
                    String currencyText = tmpArr[3];
                    String denomText = tmpArr[4];
                    String buyRate = tmpArr[5];
                    String sellRate = tmpArr[6];
                    String countryName = tmpArr[7];

                    currencyKeyArr.Add(currencyKey);

                    ExchangeRateDataObject obj = new ExchangeRateDataObject();
                    obj.currencyKey = currencyKey;
                    obj.currencyText = currencyText;
                    obj.denomText = denomText;
                    obj.shoudlDisplayFlag = shouldDisplayFlag;
                    obj.shoudlDrawBottomLine = shouldDrawBottomLine;
                    obj.buyText = buyRate;
                    obj.sellText = sellRate;
                    obj.countryName = countryName;
                    String imageName = "Flag" + currencyKey.Substring(0,3) + ".jpg";
                    obj.countryFlagImage = Util.GetImageFromImageResources(imageName);

                    SetExchangeRateObject(currencyKey, obj);
                }
            }
        }
    }

    public class ExchangeRateDataObject
    {
        public Bitmap countryFlagImage;
        public String currencyText;
        public String denomText;
        public String currencyKey;
        public String buyText;
        public String sellText;
        public String countryName;
        public bool shoudlDisplayFlag;
        public bool shoudlDrawBottomLine;
    }
}
