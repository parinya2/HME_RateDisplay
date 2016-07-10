using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HME_RateDisplay
{
    public class SignalClock
    {
        private Timer timer;
        private int theTime;
        public int TheTime
        {
            get
            {
                return theTime;
            }
            set
            {
                theTime = value;
                OnTheTimeChanged(this.theTime);
            }
        }

        public SignalClock(int interval)
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = interval;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TheTime < 10)
            {
                TheTime++;
            }
            else
            {
                TheTime = 1;
            }
        }

        public delegate void SignalClockTickHandler(int newTime);
        public event SignalClockTickHandler TheTimeChanged;

        protected void OnTheTimeChanged(int newTime)
        {
            if (TheTimeChanged != null)
            {
                TheTimeChanged(newTime);
            }
        }
    }
}
