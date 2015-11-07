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
    /// Interaction logic for MoviePage.xaml
    /// </summary>
    public partial class MoviePage : UserControl,ISwitchable
    {
        public MoviePage()
        {
            InitializeComponent();
        }
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #region ISwitchable Members
        private void back_button1(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
        private void inc_button1(object sender, RoutedEventArgs e)
        {

        }
        private void dec_button1(object sender, RoutedEventArgs e)
        {

        }
        private void play_button(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}
