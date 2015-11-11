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
        private int[] ledsequence = new int[] { 0, 1, 3, 5, 7 };
        private int counttimes;

        private int[,] randomseed = {{4 ,3 ,2 ,1},
                            {4, 3, 1, 2},
                            {4, 2, 1, 3},
                            {4, 2, 3, 1},
                            {4, 1, 3, 2},
                            {4, 1, 2, 3},
                            {3, 2, 1, 4},
                            {3, 2, 4, 1},
                            {3 ,1 ,4 ,2},
                            {3 ,1 ,2 ,4},
                            {3 ,4 ,2 ,1},
                            {3 ,4 ,1 ,2},
                            {2 ,1 ,4 ,3},
                            {2 ,1 ,3 ,4},
                            {2 ,3 ,4, 1},
                            {2 ,3 ,1 ,4},
                            {2 ,4, 1, 3},
                            {2 ,4 ,3 ,1},
                            {1 ,2 ,3 ,4},
                            {1 ,2 ,4 ,3},
                            {1 ,3 ,2, 4},
                            {1 ,3 ,4 ,2},
                            {1 ,4 ,3 ,2},
                            {1 ,4 ,2 ,3}};
        private int[,] randomseed5 = { { 2,4,3,3,4,2,4,4,2,3,1,1,3,4,4,1,3,2,1,2},
                                    { 1,4,2,3,1,3,2,3,3,3,2,1,1,4,1,4,3,4,1,2 },
                                    { 1,4,1,4,4,4,1,2,2,4,2,4,1,2,1,1,4,3,3,1},
                                    { 4,3,2,3,2,1,1,1,1,1,2,1,4,4,2,2,2,4,2,1 },
                                    { 4,2,1,2,1,1,4,4,3,1,1,2,4,1,1,1,3,3,3,2 } };

        private int[,] randomseed7 = { { 3, 2, 3, 1, 3, 1, 2, 3, 4, 1, 4, 4, 2, 2, 2, 2, 3, 3, 4, 4, 3, 2, 4, 3, 2, 4, 4, 3 },
                                        { 3, 3, 1, 2, 2, 1, 4, 1, 1, 1, 1, 2, 2, 4, 2, 1, 4, 4, 2, 1, 2, 2, 3, 2, 3, 3, 1, 1},
                                        {2, 2, 2, 3, 1, 2, 4, 1, 4, 3, 2, 3, 1, 2, 4, 3, 3, 1, 2, 3, 3, 2, 2, 4, 1, 4, 4, 4},
                                        {1, 2, 2, 3, 1, 3, 1, 3, 2, 4, 3, 4, 4, 2, 3, 1, 1, 3, 3, 2, 4, 3, 3, 4, 4, 3, 1, 1},
                                        {4, 1, 2, 1, 4, 3, 3, 2, 1, 3, 1, 1, 3, 1, 4, 4, 3, 1, 3, 3, 4, 3, 4, 2, 2, 4, 1, 1}};

        Random rnd = new Random();
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
            counttimes = 0;
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
            int set = rnd.Next(5);
            for (int t = 0;t < 7;t++)
            {
                //int set = rnd.Next(23);
                Thread.Sleep(10);
                for (int i = 0;i < 4;i++)
                {
                    int lednum = ledsequence[randomseed7[set, counttimes]];

                    OnLEDStatusUpdate(0, lednum);
                    eeg.setmarker(lednum);
                    strobe(leds[lednum], 500, 3);

                    Console.WriteLine("l: " + lednum);
                    //Thread.Sleep(500);
                    //Console.WriteLine("off");

                    leds[lednum].blackout();
                    //eeg.getEEG();
                    //OnLEDStatusUpdate();
                    //eeg.setmarker(i);
                    Thread.Sleep(40);
                    counttimes++;

                }
                //    OnLEDStatusUpdate(1, 0);
            }
            counttimes = 0;
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

        public void turnonEquipment()
        {
            port1.Write("E:" + 1 + "&" + "1" + "#");
        }

        public void turnoffEquipment()
        {
            port1.Write("E:" + 0 + "&" + "1" + "#");
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
