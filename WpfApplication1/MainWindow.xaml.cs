using System;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        EEG_Logger p;
        private bool connect;
        private Thread loggerThread;
        private Thread LEDThread;

        private bool LEDrunning;
        Label[] light;
        Rectangle[] battery_level_rect;

        int battery_level = 0;

        SerialCom s;
        Random rnd = new Random();

        LineTrend lineTrend;

        SgraphControl ctrl;

        int cycle_count = 0;

        public System.Timers.Timer _timer;

        public MainWindow()
        {
            InitializeComponent();

            lineTrend = new LineTrend { Points = new ObservableCollection<TrendPoint>(), TrendColor = Brushes.Coral };

            status.Content = "EEG Data Reader Example";

            p = new EEG_Logger();
            p.DataUpdate += HandleDataUpdate;
            p.StatusUpdate += HandleStatusUpdate;
            p.whichsUpdate += HandleLedUpdate;

            s = new SerialCom();
            s.LEDUpdate += HandleLEDUpdate;

            connect = false;
            LEDrunning = false;
            status.Content = "not connect";

        }


        private void HandleLedUpdate(object sender, EEG_WhichEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                switch (e.lednum)
                {
                    case 1:
                        ledupdate.Content = "Down";
                        break;
                    case 3:
                        ledupdate.Content = "Right";
                        break;
                    case 5:
                        ledupdate.Content = "Up";
                        break;
                    case 7:
                        ledupdate.Content = "Left";
                        break;

                }
            });

        }

        private void HandleDataUpdate(object sender, EEG_LoggerEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                updateGraph(ref graph_o1, e.Data_O1);
                updateGraph(ref graph_o2, e.Data_O2);
            });

        }

        private void HandleStatusUpdate(object sender, EEG_StatusEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                //Console.WriteLine("ff");
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
                update_signal_quality(c_signal, e.signalStrength);
                batt.Content = e.chargeLevel + "/" + e.maxChargeLevel;
                if (e.chargeLevel != battery_level)
                {
                    battery_level = e.chargeLevel;
                    update_battery(battery_level);
                }

                //baseL.Content = "Base L" + e.eSignal[0];
                update_contact_quality(c_base1, e.eSignal[0]);
                //baseR.Content = "Base R" + e.eSignal[1];
                update_contact_quality(c_base2, e.eSignal[1]);
                //OL.Content = "O L" + e.eSignal[9];
                update_contact_quality(c_o1, e.eSignal[9]);
                //OR.Content = "O R" + e.eSignal[10];
                update_contact_quality(c_o2, e.eSignal[10]);

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

                }
                else
                {
                   
                    for (int i = 0;i < light.Length;i++)
                    {
                        //if (i == 0 || i == 2 || i == 4 || i == 6) { p.getEEG(128, 64, i); }
                        if (e.status == i)
                        {
                            light[i].Foreground = Brushes.ForestGreen;
                        }

                        else
                        {
                            light[i].Foreground = Brushes.WhiteSmoke;
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

        }

        private void saveEEGButt_Click(object sender, RoutedEventArgs e)
        {
            //p.getEEG();
        }

        private void startlight_Click(object sender, RoutedEventArgs e)
        {
            if (!LEDrunning)
            {
                s.readystate();
                _timer = new System.Timers.Timer();
                _timer.Interval = 10000;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(set_light);
                _timer.Enabled = true;



                LEDrunning = true;
            }
            else
            {
                LEDThread.Abort();
                LEDrunning = false;
            }
        }

        private void set_light(object sender, System.Timers.ElapsedEventArgs e)
        {
            s.blinking(ref p);

        }

        private void updateGraph(ref Grid graph, double[] value)
        {
            lineTrend.Points.Clear();
            int count = 0;
            for (int i = 0;i < 384;i += 6)
            {
                int y;
                if (value[count] >= 50) y = 199;
                else if (value[count] <= -50) y = 1;
                else y = (int) (value[count] * 2) + 100;
                lineTrend.Points.Add(new TrendPoint { X = i + 1, Y = y });
                count++;
            }
            ctrl = new SgraphControl();
            ctrl.Trends.Add(lineTrend);
            graph.Children.Add(ctrl);
        }

        private void update_contact_quality(Ellipse ec, string status)
        {
            switch (status)
            {
                case "EEG_CQ_NO_SIGNAL":
                    ec.Fill = Brushes.Black;
                    break;
                case "EEG_CQ_VERY_BAD":
                    ec.Fill = Brushes.Red;
                    break;
                case "EEG_CQ_POOR":
                    ec.Fill = Brushes.Orange;
                    break;
                case "EEG_CQ_FAIR":
                    ec.Fill = Brushes.Yellow;
                    break;
                case "EEG_CQ_GOOD":
                    ec.Fill = Brushes.LimeGreen;
                    break;
                default:
                    ec.Fill = Brushes.MediumPurple;
                    break;
            }
        }

        private void update_signal_quality(Ellipse ec, string status)
        {
            switch (status)
            {
                case "NO_SIGNAL":
                    ec.Fill = Brushes.Black;
                    break;
                case "BAD_SIGNAL":
                    ec.Fill = Brushes.Red;
                    break;
                case "GOOD_SIGNAL":
                    ec.Fill = Brushes.LimeGreen;
                    break;
                default:
                    ec.Fill = Brushes.MediumPurple;
                    break;
            }
        }
        private void update_battery(int battery)
        {
            if (battery_level_rect == null)
            {
                battery_level_rect = new Rectangle[] { battery_1, battery_2, battery_3, battery_4, battery_5 };
            }
            for (int i = 0;i < 5;i++)
            {
                if (i < battery)
                {
                    battery_level_rect[i].Visibility = Visibility.Visible;
                    battery_level_rect[i].Fill = Brushes.LimeGreen;
                }
                else

                    battery_level_rect[i].Visibility = Visibility.Hidden;

            }
            if (battery == 1)
            {
                battery_level_rect[0].Fill = Brushes.Red;
            }
        }


    }



}

