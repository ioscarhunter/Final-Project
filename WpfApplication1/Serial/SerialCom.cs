using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.Threading;
using System.Windows.Media;

namespace WpfApplication1
{
    class SerialCom
    {
        private SerialPort port1;
        private int bRate = 460800;
        private LED[] leds;
        private int lednum = 8;
        private int[] ledstatus;
        private int cycle_count = 0;
        private int[] ledsequence = new int[] { 1, 3, 5, 7 };

        public event EventHandler<LED_StatusEventArgs> LEDUpdate;

        public SerialCom()
        {
            leds = new LED[lednum];
            ledstatus = new int[lednum];

            Console.WriteLine(AutodetectArduinoPort());
            port1 = new SerialPort();
            port1.PortName = AutodetectArduinoPort();
            port1.BaudRate = bRate;
            port1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            port1.Open();
            Thread.Sleep(5);

            setupColour(colourset.LIMEGREEN);
            all_off();
        }

        public void readystate()
        {
            changeColour(colourset.RED);
            all_dim();
        }

        public void setupColour(int setcolour)
        {
            for (int i = 0;i < lednum;i++)
            {
                leds[i] = new LED(i, setcolour, ref port1);
                leds[i].setupLED();
                Thread.Sleep(8);
            }
        }

        public void changeColour(int setcolour)
        {
            for (int i = 0;i < lednum;i++)
            {
                leds[i].changecolour(setcolour);
                Thread.Sleep(5);
            }
        }

        public void all_on()
        {
            for (int i = 0;i < lednum;i++)
            {
                leds[i].turnon();
                Thread.Sleep(5);
            }
        }

        public void all_off()
        {
            for (int i = 0;i < lednum;i++)
            {
                leds[i].blackout();
                Thread.Sleep(5);
            }
        }

        public void all_dim()
        {
            for (int i = 0;i < lednum;i++)
            {
                leds[i].turnoff();
                Thread.Sleep(5);
            }
        }
        public void blinking(ref EEG_Logger eeg)
        {
            //eeg.getEEG();
            changeColour(colourset.LIMEGREEN);
            all_off();
            for (int t = 0;t < 11;t++)
            {
                Thread.Sleep(40);
                for (int i = 1;i < lednum;i += 2)
                {
                    Console.WriteLine(i);
                    ledstatus[i] = 1;
                    eeg.setmarker(i);
                    strobe(leds[i], 500, 3);

                    Console.WriteLine("l: " + i);
                    //Thread.Sleep(500);
                    //Console.WriteLine("off");

                    ledstatus[i] = 0;
                    OnLEDStatusUpdate(0, i);
                    leds[i].blackout();
                    //eeg.getEEG();
                    //OnLEDStatusUpdate();
                    //eeg.setmarker(i);
                    Thread.Sleep(40);

                }
                //    OnLEDStatusUpdate(1, 0);

            }
            //changeColour(colourset.VERYDARKGRAY);
            //all_dim();
            //eeg.writedata();
            //eeg.compute();
        }

        public void strobe(LED led, int totaltime, int times)
        {
            for (int i = 0;i < times;i++)
            {
                led.turnon();
                Thread.Sleep(totaltime / (times * 2));
                led.blackout();
                Thread.Sleep(totaltime / (times * 2));
            }
        }

        public void OnLEDStatusUpdate(int i, int led)
        {
            if (LEDUpdate != null)
                LEDUpdate(this, new LED_StatusEventArgs(led, i));
        }

        private string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                throw new NotConnectException();
            }

            return null;
        }
        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Write the serial port data to the console.
            Console.WriteLine(port1.ReadExisting());

        }
    }

    public class LED_StatusEventArgs:EventArgs
    {
        public int status;
        public int cycle;

        public LED_StatusEventArgs(int ledstatus, int cycle)
        {
            this.cycle = cycle;
            status = ledstatus;
        }
    }
}
