using System;
using System.Threading;
using Frequencytest.Logger;
using Frequencytest.Serial; 
using System.Threading.Tasks;

namespace Frequencytest
{
    class Program
    {
        private static EEG_Logger p;
        private static SerialCom s;

        private bool connect = false;
        private Thread loggerThread;
        private Thread LEDThread;

        void connectEngin()
        {
            if (!connect)
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine(i);
                    try
                    {
                        if (loggerThread == null)
                        {
                            loggerThread = new Thread(p.Run);
                            loggerThread.Start();
                        }
                        Thread.Sleep(100);
                    }
                    catch (NotConnectException error)
                    {
                        continue;
                    }
                    catch (TaskCanceledException error)
                    {
                        return;
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("EEG Data Reader Example");
            try
            {
                p = new EEG_Logger(11+10);
                s = new SerialCom();
                for (int i = 0; i < 2; i++)
                {
                    p.connect();
                    Thread.Sleep(1000);
                }

                p.setMarker(1);
                Thread.Sleep(10*1000);
                p.setMarker(1);
                
                s.blinking();
                p.setMarker(10);
                Thread.Sleep(10*1000);
                p.setMarker(10);
                s.all_off();

                p.Run();


            }
            catch (NotConnectException e)
            {
                Console.WriteLine("not connect");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
