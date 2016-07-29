using System;
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
        /*public static String[] currencyKeyArr = { "USD1", "USD2", "USD3", "USD4", "USD5", 
                                           "EUR1", "EUR2", "EUR3", "GBP1", "GBP2", 
                                           "AUD", "CNY", "JPY1", "JPY2", "SGD1", "SGD2", 
                                           "MYR1", "MYR2", "MYR3", "TWD1", "TWD2",
                                           "KRW1", "KRW2", "HKD" };*/
        public static ArrayList currencyKeyArr;

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

        public static void SaveData(String data)
        {
            File.WriteAllText(Util.GetTokenPath(), data);
        }

        public static void CreateDailyReport()
        {
            LoadData();

            StringBuilder sb = new StringBuilder("");
            foreach (string key in currencyKeyArr)
            {
                ExchangeRateDataObject dataObj = GetExchangeRateObjectForKey(key);
                string tabString = dataObj.currencyText.Length > 15 ? "\t" : "\t\t";              
                string tmp = dataObj.currencyText + tabString + "Buy = " + dataObj.buyText + "\tSell = " + dataObj.sellText;
                sb.Append(tmp);
                sb.Append(Environment.NewLine);
            }

            File.WriteAllText(Util.GetDailyReportPath(), sb.ToString());
        }

        public static void LoadData()
        {
            currencyKeyArr = new ArrayList();
            string allText = "" + File.ReadAllText(Util.GetTokenPath(), Encoding.UTF8);
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
                if (tmpArr.Length == 5)
                {
                    String currencyKey = tmpArr[0];
                    bool shouldDisplayFlag = tmpArr[1].Equals("T");
                    String currencyText = tmpArr[2];
                    String buyRate = tmpArr[3];
                    String sellRate = tmpArr[4];

                    currencyKeyArr.Add(currencyKey);

                    ExchangeRateDataObject obj = new ExchangeRateDataObject();
                    obj.currencyKey = currencyKey;
                    obj.currencyText = currencyText;
                    obj.shoudlDisplayFlag = shouldDisplayFlag;
                    obj.buyText = buyRate;
                    obj.sellText = sellRate;
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
        public String currencyKey;
        public String buyText;
        public String sellText;
        public bool shoudlDisplayFlag;
    }
}
