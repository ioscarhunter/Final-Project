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
        }
        private void Buttonmusic_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MusicPage());
        }

        private void Buttonsetting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Setting");
        }
        private void ButtonTV_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MoviePage());
        }
        private void ButtonNews_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("NEWs Feed");
        }
        private void ButtonFan_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fan opened");
        }
        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
