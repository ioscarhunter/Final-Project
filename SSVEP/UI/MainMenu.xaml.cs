using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl, ISwitchable
    {
        public MainMenu()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToString("HH:mm:ss");
        }
        private void Buttonmusic_Click(object sender, RoutedEventArgs e)
        {
            Switch_Music();
        }
        private void Switch_Music() { Switcher.Switch(new MusicPage()); }

        private void Buttonsetting_Click(object sender, RoutedEventArgs e)
        {
            Switch_Setting();
        }
        private void Switch_Setting() { Switcher.loadsetting(); }
        private void ButtonTV_Click(object sender, RoutedEventArgs e)
        {
            Switch_TV();
        }
        private void Switch_TV() { Switcher.Switch(new MoviePage()); }
        private void ButtonNews_Click(object sender, RoutedEventArgs e)
        {
            Switch_News();
        }
        private void Switch_News() { Switcher.Switch(new NewsPage()); }

        private void ButtonFan_Click(object sender, RoutedEventArgs e)
        {
            Switch_Fan();
        }

        private void Switch_Fan() { Switcher.Switch(new controlequipment()); }
        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        public void sendcommand(int command)
        {
            switch (command)
            {
                case 1:
                    Switch_News();
                    break;
                case 2:
                    Switch_Fan();
                    break;
                case 3:
                    Switch_Music();
                    break;
                case 4:
                    Switch_TV();
                    break;
            }
        }

        #endregion

    }
}
