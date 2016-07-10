﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HME_RateDisplay
{
    public class LocalizedTextManager
    {
        public enum LanguageMode { TH, EN };
        private static LocalizedTextManager instance;
        private Dictionary<string, string> dataDictTH;
        private Dictionary<string, string> dataDictEN;
        LanguageMode language;

        private LocalizedTextManager()
        {
            dataDictTH = new Dictionary<string, string>();
            dataDictEN = new Dictionary<string, string>();

            SetDictValueForKey("ConfirmExitMessageBox.Message", "คุณต้องการออกจากโปรแกรมใช่หรือไม่", "Do you want to exit the program ?");
            SetDictValueForKey("ConfirmExitMessageBox.RightButton", "ตกลง", "OK");
            SetDictValueForKey("ConfirmExitMessageBox.LeftButton", "ย้อนกลับ", "Go Back");
        }

        public static LocalizedTextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizedTextManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new LocalizedTextManager();
            }
        }

        private void SetDictValueForKey(string key, string valueTH, string valueEN)
        {
            dataDictTH.Add(key, valueTH);
            dataDictEN.Add(key, valueEN);
        }

        public static void SetLanguage(int languageMode)
        {
            if (languageMode == 0)
            {
                instance.language = LanguageMode.TH;
            }
            if (languageMode == 1)
            {
                instance.language = LanguageMode.EN;
            }
        }

        public static string GetLocalizedTextForKeyTH(string key)
        {
            string returnValue;
            Dictionary<string, string> targetDict = Instance.dataDictTH;

            if (targetDict.ContainsKey(key))
            {
                returnValue = targetDict[key];
            }
            else
            {
                returnValue = null;
            }

            return returnValue;
        }

        public static string GetLocalizedTextForKey(string key)
        {
            string returnValue;
            Dictionary<string, string> targetDict = Instance.dataDictTH;

            if (Instance.language == LanguageMode.EN)
            {
                targetDict = Instance.dataDictEN;
            }

            if (targetDict.ContainsKey(key))
            {
                returnValue = targetDict[key];
            }
            else
            {
                returnValue = null;
            }

            return returnValue;
        }
    }
}