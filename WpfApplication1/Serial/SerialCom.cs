using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.Threading;

namespace WpfApplication1
{
    class SerialCom
    {
        private SerialPort port1;
        private int bRate = 460800;
        private LED[] leds;
        private int lednum = 8;
        private int[] ledstatus;

        public event EventHandler<LED_StatusEventArgs> LEDUpdate;

        public SerialCom()
        {
            leds = new LED[lednum];
            ledstatus = new int[8];

            Console.WriteLine(AutodetectArduinoPort());
            port1 = new SerialPort();
            port1.PortName = AutodetectArduinoPort();
            port1.BaudRate = bRate;
            port1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            port1.Open();
            Thread.Sleep(5);
            setupColour();
        }

        public void setupColour()
        {
            for (int i = 0;i < leds.Length;i++)
            {
                leds[i] = new LED(i, 0x228B22, ref port1);
                leds[i].setupLED();
                Thread.Sleep(3);
            }
        }

        public void blinking()
        {
            while (true)
            {
                Thread.Sleep(50);
                for (int i = 0;i < lednum;i++)
                {
                    Thread.Sleep(1);
                    //Console.WriteLine("on");
                    ledstatus[i] = 1;
                    OnLEDStatusUpdate(0);
                    strobe(leds[i], 500, 3);
                    
                    Console.WriteLine("l: " + i);
                    //Thread.Sleep(500);
                    //Console.WriteLine("off");
                    leds[i].blackout();
                    ledstatus[i] = 0;
                    //OnLEDStatusUpdate();
                    Thread.Sleep(1);

                }
                OnLEDStatusUpdate(1);
            }
        }

        public void strobe(LED led,int totaltime,int times)
        {
            for (int i = 0;i < times; i++){
                led.turnon();
                Thread.Sleep(totaltime / (times * 2));
                led.turnoff();
                Thread.Sleep(totaltime / (times * 2));
            }
        }

        public void OnLEDStatusUpdate(int i)
        {
            if (LEDUpdate != null)
                LEDUpdate(this, new LED_StatusEventArgs(ledstatus,i));
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
        public int[] status;
        public int cycle;

        public LED_StatusEventArgs(int[] ledstatus,int cycle)
        {
            this.cycle = cycle;
            status = ledstatus;
        }
    }
}
