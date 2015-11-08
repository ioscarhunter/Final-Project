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
using System.Windows.Threading;

namespace WPFProject
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
        private void Switch_Setting() { Switcher.Switch(new SettingPage()); }
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

        private void Switch_Fan() { MessageBox.Show("Fan opened"); }
        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
