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

            status.Content = "EEG Data Reader Example";

            p = new EEG_Logger();
            p.DataUpdate += HandleDataUpdate;
            p.StatusUpdate += HandleStatusUpdate;
            p.whichsUpdate += HandleLedUpdate;
            p.GyroUpdate += HandleGyroUpdate;

            s = new SerialCom();
            s.LEDUpdate += HandleLEDUpdate;
            s.LEDFinish += HandleLEDFinish;

            connect = false;
            LEDrunning = false;
            status.Content = "not connect";
            //Switcher.remotechange(3);
        }

        private void HandleLEDFinish(object sender, LED_FinishEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                processGyro();
                reset = true;
            });
        }

        private void processGyro()
        {
            
            if (Math.Abs(deltax) > Math.Abs(deltay)&&Math.Abs(deltax) > 10)
            {
                if (deltax > 0)
                {
                    updateselection(3);
                }

                else
                {
                    updateselection(7);
                }
            }

            else if (Math.Abs(deltay) > Math.Abs(deltax) && Math.Abs(deltay) > 10)
            {
                if (deltay > 0)
                {
                    updateselection(5);
                }

                else
                {
                    updateselection(1);
                }
            }
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
                //updateselection(e.lednum);
            });

        }

        private void updateselection(int lednum)
        {
            switch (lednum)
            {
                case 1:
                    remotechangepage(1);
                    ledupdate.Content = "Down";
                    break;
                case 3:
                    remotechangepage(2);
                    ledupdate.Content = "Right";
                    break;
                case 5:
                    remotechangepage(3);
                    ledupdate.Content = "Up";
                    break;
                case 7:
                    remotechangepage(4);
                    ledupdate.Content = "Left";
                    break;

            }
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
                gyro.Content = deltax + ", " + deltay;
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
                _timerEEGREC = new System.Timers.Timer();
                _timerEEGREC.Interval = 13000;
                _timerEEGREC.Elapsed += new System.Timers.ElapsedEventHandler(saveEEGdata);
                _timerEEGREC.Enabled = true;

                _timerMARK = new System.Timers.Timer();
                _timerMARK.Interval = 13000;
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
