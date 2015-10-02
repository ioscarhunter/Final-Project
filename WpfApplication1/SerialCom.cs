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
                Thread.Sleep(20);
                for (int i = 0;i < lednum;i++)
                {
                    //Console.WriteLine("on");
                    leds[i].turnon();
                    ledstatus[i] = 1;
                    OnLEDStatusUpdate();
                    Thread.Sleep(110);
                    //Console.WriteLine("off");
                    leds[i].turnoff();
                    ledstatus[i] = 0;
                    //OnLEDStatusUpdate();
                    Thread.Sleep(3);

                }
                
            }
        }

        public void OnLEDStatusUpdate()
        {
            if (LEDUpdate != null)
                LEDUpdate(this, new LED_StatusEventArgs(ledstatus));
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
                /* Do Nothing */
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

        public LED_StatusEventArgs(int[] ledstatus)
        {
            status = ledstatus;
        }
    }
}
