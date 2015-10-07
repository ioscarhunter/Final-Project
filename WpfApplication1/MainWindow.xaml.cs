using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emotiv;
using System.Threading;
using System.Windows.Threading;


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        double data_o1 = 0;
        double data_o2 = 0;
        EEG_Logger p;
        private bool connect;
        private Thread loggerThread;
        private Thread LEDThread;

        private bool LEDrunning;
        Label[] light;

        SerialCom s;

        public MainWindow()
        {
            InitializeComponent();
            status.Content = "EEG Data Reader Example";

            p = new EEG_Logger();
            p.DataUpdate += HandleDataUpdate;
            p.StatusUpdate += HandleStatusUpdate;

            s = new SerialCom();
            s.LEDUpdate += HandleLEDUpdate;

            connect = false;
            LEDrunning = false;
            status.Content = "not connect";

        }

        private void HandleDataUpdate(object sender, EEG_LoggerEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                O1.Content = "O1 " + e.Data_O1;
                O2.Content = "O2 " + e.Data_O2;
            });

        }

        private void HandleStatusUpdate(object sender, EEG_StatusEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                time.Content = "Time on " + e.timePass;
                headseton.Content = "Headset status " + e.headsetOn;
                if (!connect && e.headsetOn == 1)
                {
                    connect = true;
                    status.Content = "Connected";
                }
                if (connect && e.headsetOn == 0)
                {
                    connect = false;
                    status.Content = "not connect";
                }

                signal.Content = "Signal " + e.signalStrength;
                batt.Content = "Battery " + e.chargeLevel + "/" + e.maxChargeLevel;

                baseL.Content = "Base L" + e.eSignal[0];
                baseR.Content = "Base R" + e.eSignal[1];

                OL.Content = "O L" + e.eSignal[9];
                OR.Content = "O R" + e.eSignal[10];

            });

        }

        private void HandleLEDUpdate(object sender, LED_StatusEventArgs e)
        {
            if (light == null)
                light = new Label[] { light1, light2, light3, light4, light5, light6, light7, light8 };
            Dispatcher.Invoke(() =>
            {
                if (e.cycle == 1)
                {
                    p.getEEG();
                }
                else
                {
                    for (int i = 0;i < light.Length;i++)
                    {

                        switch (e.status[i])
                        {
                            case 1:
                                light[i].Foreground = Brushes.ForestGreen;
                                break;
                            case 0:

                                light[i].Foreground = Brushes.WhiteSmoke;
                                break;
                            default:
                                break;
                        }
                    }
                }
            });

        }

        private void connect_but_Click(object sender, RoutedEventArgs e)
        {
            if (!connect)
            {
                status.Content = "not connect";
                connect_but.Content = "Connecting";
                for (int i = 0;i < 2;i++)
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
                        status.Content = "trying to connect";
                        continue;
                    }
                    catch (TaskCanceledException error)
                    {
                        return;
                    }
                }
            }
            //else
            //{
            //    status.Content = "Connected";

            //}
            //connect_but.Content = "Connect";
        }

        private void saveEEGButt_Click(object sender, RoutedEventArgs e)
        {
            p.getEEG();
        }

        private void startlight_Click(object sender, RoutedEventArgs e)
        {
            if (!LEDrunning)
            {
                LEDThread = new Thread(s.blinking);
                LEDThread.Start();
                LEDrunning = true;
            }
            else
            {
                LEDThread.Abort();
                LEDrunning = false;
            }
        }
    }
}

