using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;
using System.Threading;

namespace SerialCStest
{

    class Program
    {
        static SerialPort port1;
        static int bRate = 460800;
        //static long[] timearr;
        static void Main(string[] args)
        {
            //timearr = new long[1000];
            Console.WriteLine(AutodetectArduinoPort());
            port1 = new SerialPort();
            port1.PortName = AutodetectArduinoPort();
            port1.BaudRate = bRate;
            port1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            port1.Open();
            Thread.Sleep(10);
            LED[] leds = new LED[8];
            for (int i = 0;i < leds.Length;i++)
            {
                leds[i] = new LED(i, 0x228B22, ref port1);
                leds[i].setupLED();
                Thread.Sleep(5);
            }

            while (true)
            {

                for (int i = 0;i < 8;i++)
                {
                    Console.WriteLine("on");
                    leds[i].turnon();
                    Thread.Sleep(110);
                    Console.WriteLine("off");
                    leds[i].turnoff();
                    Thread.Sleep(5);


                }
            }
            
            //for (int j = 0; j < 10; j++)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        Thread.Sleep(64);
            //        port1.Write(((char)(i + 48)).ToString());
            //        timearr[i] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //    }
            //}
            Console.ReadKey();
        }

        static string AutodetectArduinoPort()
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
        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Write the serial port data to the console.
            Console.WriteLine(port1.ReadExisting());

        }
    }
}
