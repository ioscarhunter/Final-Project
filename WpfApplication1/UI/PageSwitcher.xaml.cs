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
    /// Interaction logic for PageSwitcher.xaml
    /// </summary>
    public partial class PageSwitcher:Window
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

        double deltax;
        double deltay;

        SgraphControl ctrl;

        int cycle_count = 0;

        public System.Timers.Timer _timerEEGREC;
        public System.Timers.Timer _timerMARK;

        private bool reset;

        UserControl currentuc;

        private ISwitchable interfaces;
        public PageSwitcher()
        {

            InitializeComponent();

            Switcher.pageSwitcher = this;
            Switcher.Switch(new MainMenu());

            lineTrend = new LineTrend { Points = new ObservableCollection<TrendPoint>(), TrendColor = Brushes.Coral };


            p = new EEG_Logger();
            p.StatusUpdate += HandleStatusUpdate;
            p.whichsUpdate += HandleLedUpdate;
            p.GyroUpdate += HandleGyroUpdate;

            s = new SerialCom();

            connect = false;
            LEDrunning = false;
            //Switcher.remotechange(3);
        }


        internal void starteeg()
        {
            throw new NotImplementedException();
        }

        public void Navigate(UserControl nextPage)
        {
            if (stkPanel.Children.Count > 0)
            {
                stkPanel.Children.RemoveAt(stkPanel.Children.Count - 1);

            }
            stkPanel.Children.Add(nextPage);
            UserControl currentuc = nextPage;
            interfaces = currentuc as ISwitchable;
        }
        public void Navigate(UserControl nextPage, object state)
        {
            if (stkPanel.Children.Count > 0)
            {
                stkPanel.Children.RemoveAt(stkPanel.Children.Count - 1);
            }

            stkPanel.Children.Add(nextPage);
            UserControl currentuc = nextPage;
            interfaces = currentuc as ISwitchable;

            if (interfaces != null)
                interfaces.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable!"
                    + nextPage.Name.ToString());

        }

        public void remotechangepage(int command)
        {
            if (stkPanel.IsVisible)
            {
                foreach (object child in stkPanel.Children)
                    interfaces = child as ISwitchable;
                Console.WriteLine(interfaces);
                interfaces.sendcommand(command);
            }
        }


        public void showsetting()
        {
            stkPanel.Visibility = Visibility.Hidden;
        }


        private void HandleLedUpdate(object sender, EEG_WhichEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                switch (e.lednum)
                {
                    case 1:
                        remotechangepage(1);
                        break;
                    case 3:
                        remotechangepage(2);
                        break;
                    case 5:
                        remotechangepage(3);
                        break;
                    case 7:
                        remotechangepage(4);
                        break;

                }
            });

        }



        private void HandleStatusUpdate(object sender, EEG_StatusEventArgs e)
        {
            // dispatch the modification to the text box to the UI thread (main window dispatcher)
            Dispatcher.Invoke(() =>
            {
                //Console.WriteLine("ff");

                if (!connect && e.headsetOn == 1)
                {
                    connect = true;
                    //status.Content = "Connected";
                }
                if (connect && e.headsetOn == 0)
                {
                    connect = false;
                    ///status.Content = "not connect";
                }

                //signal.Content = "Signal " + e.signalStrength;
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


        private void HandleGyroUpdate(object sender, EEG_GyroEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (reset)
                {
                    deltax = 0;
                    deltay = 0;
                    reset = false;

                }

                deltax += e.gyrox;
                deltay += e.gyroy;
                //gyro.Content = deltax + ", " + deltay;
            });
        }

        private void connect_but_Click(object sender, RoutedEventArgs e)
        {
            if (!connect)
            {
                //status.Content = "not connect";
                connect_but.Content = "Connected";
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
                        //status.Content = "trying to connect";
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
                _timerEEGREC = new System.Timers.Timer();
                _timerEEGREC.Interval = 20000;
                _timerEEGREC.Elapsed += new System.Timers.ElapsedEventHandler(saveEEGdata);
                _timerEEGREC.Enabled = true;

                _timerMARK = new System.Timers.Timer();
                _timerMARK.Interval = 20000;
                _timerMARK.Elapsed += new System.Timers.ElapsedEventHandler(setmarker);
                _timerMARK.Enabled = true;

                LEDrunning = true;
            }
            else
            {
                LEDThread.Abort();
                LEDrunning = false;
            }
        }

        private void saveEEGdata(object sender, System.Timers.ElapsedEventArgs e)
        {
            p.getEEG();

        }

        private void setmarker(object sender, System.Timers.ElapsedEventArgs e)

        {
            reset = true;
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

        public void turnlighon()
        {
            s.turnonEquipment();
        }

        public void turnlighoff()
        {
            s.turnoffEquipment();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            stkPanel.Visibility = Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            reset = true;
        }
    }
}
