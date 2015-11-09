using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class controller
    {
        EEG_Logger p;
        private bool connect;

        private bool LEDrunning;

        int battery_level = 0;

        SerialCom s;
        Random rnd = new Random();

        LineTrend lineTrend;

        SgraphControl ctrl;

        int cycle_count = 0;

        public System.Timers.Timer _timerEEGREC;
        public System.Timers.Timer _timerMARK;

        public controller()
        {
            p = new EEG_Logger();
            p.DataUpdate += HandleDataUpdate;
            p.StatusUpdate += HandleStatusUpdate;
            p.whichsUpdate += HandleLedUpdate;

            s = new SerialCom();
            s.LEDUpdate += HandleLEDUpdate;

            connect = false;
            LEDrunning = false;
        }

        public void startlight()
        {
            if (!LEDrunning)
            {
                s.readystate();
                _timerEEGREC = new System.Timers.Timer();
                _timerEEGREC.Interval = 10000;
                _timerEEGREC.Elapsed += new System.Timers.ElapsedEventHandler(saveEEGdata);
                _timerEEGREC.Enabled = true;

                _timerMARK = new System.Timers.Timer();
                _timerMARK.Interval = 10000;
                _timerMARK.Elapsed += new System.Timers.ElapsedEventHandler(setmarker);
                _timerMARK.Enabled = true;

                LEDrunning = true;
            }
            else
            {
                LEDrunning = false;
            }
        }

        private void saveEEGdata(object sender, System.Timers.ElapsedEventArgs e)
        {
            p.getEEG();

        }

        private void setmarker(object sender, System.Timers.ElapsedEventArgs e)
        {
            s.blinking(ref p);
        }

    }
}
