using System;
using System.Threading;
using Frequencytest.Logger;
using Frequencytest.Serial;
using System.Threading.Tasks;
using System.IO;

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
            int time =20;
			int freq = 10;

            int starttime = 10;
            String Prefix = "OCR";
			String folder = ".\\" + "data" + "\\";

			//strip s = new strip( + "OCR-20,10-030416-110942");

			string[] fileEntries = Directory.GetFiles(folder);
			foreach (string fileName in fileEntries)
			{
				String filename = Path.GetFileName(fileName);
				strip s = new strip(folder, filename.TrimEnd(".csv".ToCharArray()));
			}

			//Console.WriteLine("EEG Data Reader Example");
			//try
			//{
			//	p = new EEG_Logger(11 + time, freq, Prefix);
			//	s = new SerialCom();
			//	for (int i = 0; i < 2; i++)
			//	{
			//		p.connect();
			//		Thread.Sleep(1000);
			//	}

			//	p.setMarker(1);
			//	Thread.Sleep(starttime * 1000);
			//	p.setMarker(1);


			//Dual colour
			//s.changeColour(0, colourset.GREEN);
			//s.blinking(0, freq);
			//p.setMarker(freq);
			//Thread.Sleep((time / 2) * 1000);


			//s.changeColour(0, colourset.RED);
			//s.blinking(0, freq);
			//p.setMarker(freq);
			//Thread.Sleep((time / 2) * 1000);
			//p.setMarker(freq);
			//s.all_off();

			//Single colour

			//	s.blinking(0, freq);
			//		p.setMarker(freq);
			//		Thread.Sleep(time * 1000);
			//		p.setMarker(freq);
			//		s.all_off();

			//		p.Run();


			//	}
			//	catch (NotConnectException e)
			//	{
			//		Console.WriteLine("not connect");
			//	}

			//	Console.WriteLine("Press any key to continue...");
			//	         Console.ReadKey();
		}
	}
}
