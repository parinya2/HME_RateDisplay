using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HME_RateDisplay
{
    class FormsManager
    {
        private FormMainMenu instanceFormMainMenu;
        private FormFadeView instanceFormFadeView;
        private FormFadeView instanceFormBaseBackgroundView;
        private FormRateDisplay instanceFormRateDisplay;

        private static FormsManager instance;

        private FormsManager()
        {

        }

        public static FormsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FormsManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new FormsManager();
            }
        }

        public static FormRateDisplay GetFormRateDisplay()
        {
            if (Instance.instanceFormRateDisplay == null)
            {
                Instance.instanceFormRateDisplay = new FormRateDisplay();
            }
            return Instance.instanceFormRateDisplay;
        }

        public static FormFadeView GetFormFadeView()
        {
            if (Instance.instanceFormFadeView == null)
            {
                Instance.instanceFormFadeView = new FormFadeView();
            }
            return Instance.instanceFormFadeView;
        }

        public static FormFadeView GetFormBaseBackgroundView()
        {
            if (Instance.instanceFormBaseBackgroundView == null)
            {
                Instance.instanceFormBaseBackgroundView = new FormFadeView();
            }
            return Instance.instanceFormBaseBackgroundView;
        }

        public static FormMainMenu GetFormMainMenu()
        {
            if (Instance.instanceFormMainMenu == null)
            {
                Instance.instanceFormMainMenu = new FormMainMenu();
            }
            return Instance.instanceFormMainMenu;
        }

        public static void SetFormMainMenu(FormMainMenu formMainMenu)
        {
            Instance.instanceFormMainMenu = formMainMenu;
        }
    }
}
