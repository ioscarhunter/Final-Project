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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for NewsPage.xaml
    /// </summary>
    public partial class NewsPage:UserControl, ISwitchable
    {
        public NewsPage()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToString("HH:mm:ss");
        }
        public void sendcommand(int command)
        {
            switch (command)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    Switch_back();
                    break;
            }
        }
        #endregion

        private void back_button(object sender, RoutedEventArgs e)
        {
            Switch_back();

        }
        private void Switch_back()
        {
            Switcher.Switch(new MainMenu());
        }
    }
}
