using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
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
