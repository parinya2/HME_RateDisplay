using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace HME_RateDisplay
{
    public class Util
    {
        private static Dictionary<int, Color> buttonBlinkColorDict;

        public static string GetExecutingPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static string GetTokenPath()
        {
            return GetExecutingPath() + "/Private/token";
        }

        public static string GetToken2Path()
        {
            return GetExecutingPath() + "/Private/token2";
        }

        public static string GetDailyReportPath()
        {
            string dateString = DateTime.Now.ToString("dd-MM-yyyy เวลา HH.mm.ss น.");
            return GetExecutingPath() + "/Reports/รายงานวันที่ " + dateString +".txt";
        }

        static string ExtractEmailPassword()
        {
            string text = "" + File.ReadAllText(GetTokenPath(), Encoding.UTF8);
            text = text.Trim();
            StringBuilder result = new StringBuilder("");
            for (int i = 0; i < text.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result.Append(text[i]);
                }
            }
            return result.ToString();
        }

        public static string[] readEmailAdminFromTextFile()
        {
            string[] result;
            string text = "" + File.ReadAllText(GetExecutingPath() + "/Config/Email_admin.txt", Encoding.UTF8);
            text = text.Trim();
            if (text.Contains(","))
            {
                result = text.Split(',');
            }
            else
            {
                result = new string[1];
                result[0] = text;
            }
            return result;
        }
        static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void printLine(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg + "\n");
        }

        public static Bitmap GetImageFromImageResources(string imageName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string path = "HME_RateDisplay.Images." + imageName;
            string pathNull = "HME_RateDisplay.Images.FlagNull.jpg";
            Stream myStream = myAssembly.GetManifestResourceStream(path);

            if (myStream == null)
            {
                myStream = myAssembly.GetManifestResourceStream(pathNull);
            }
        
            return new Bitmap(myStream);
        }

        public static void GenerateButtonBlinkColorDict()
        {
            buttonBlinkColorDict = new Dictionary<int, Color>();

            int stateCount = 40;
            int H = 30;
            int S = 240;
            int L = 240;
            for (int i = 1; i <= stateCount; i++)
            {
                L = 240 - i * 3;
                Color targetColor = ColorFromHSL(H, S, L);
                buttonBlinkColorDict[i] = targetColor;
            }

            for (int i = 1; i <= 3; i++)
            {
                buttonBlinkColorDict[stateCount + i] = buttonBlinkColorDict[stateCount];
            }
        }

        public static Color GetButtonBlinkColorAtSignalState(int state)
        {
            return buttonBlinkColorDict[state];
        }

        public static Color ColorFromHSL(int H, int S, int L)
        {
            double hue = (H / 240.0) * 360.0;
            double sat = S / 240.0;
            double lightness = L / 240.0;

            double C = (1 - Math.Abs(2 * lightness - 1)) * sat;
            double hueTmp = hue / 60.0;
            double X = C * (1 - Math.Abs((hueTmp % 2) - 1));
            double m = lightness - C / 2;

            double tmpR = 0, tmpG = 0, tmpB = 0;
            if (hue >= 0 && hue < 60)
            {
                tmpR = C;
                tmpG = X;
                tmpB = 0;
            }
            else if (hue >= 60 && hue < 120)
            {
                tmpR = X;
                tmpG = C;
                tmpB = 0;
            }
            else if (hue >= 120 && hue < 180)
            {
                tmpR = 0;
                tmpG = C;
                tmpB = X;
            }
            else if (hue >= 180 && hue < 240)
            {
                tmpR = 0;
                tmpG = X;
                tmpB = C;
            }
            else if (hue >= 240 && hue < 300)
            {
                tmpR = X;
                tmpG = 0;
                tmpB = C;
            }
            else if (hue >= 300 && hue < 360)
            {
                tmpR = C;
                tmpG = 0;
                tmpB = X;
            }

            int newR = (int)((tmpR + m) * 255);
            int newG = (int)((tmpG + m) * 255);
            int newB = (int)((tmpB + m) * 255);

            return Color.FromArgb(255, newR, newG, newB);
        }
    }
}
