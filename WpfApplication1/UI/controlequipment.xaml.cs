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

namespace WpfApplication1.UI
{
    /// <summary>
    /// Interaction logic for controlequipment.xaml
    /// </summary>
    public partial class controlequipment:UserControl, ISwitchable
    {
        Boolean lighton;
        public controlequipment()
        {
            InitializeComponent();
            lighton = false;
        }


        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
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
                    fanclick();
                    break;
                case 4:
                    Switch_back();
                    break;
            }
        }

        private void back_button(object sender, RoutedEventArgs e)
        {
            Switch_back();

        }
        private void Switch_back()
        {
            Switcher.Switch(new MainMenu());
        }

        private void ButtonFan_Click(object sender, RoutedEventArgs e)
        {
            fanclick();
        }

        private void fanclick()
        {
            if (lighton)
            {
                lightblub.Fill = Brushes.Black;
                lighton = false;
                Switcher.turnofflight();
            }

            else
            {
                lightblub.Fill = Brushes.Yellow;
                lighton = true;
                Switcher.turnonlight();
            }
        }

    }
}
