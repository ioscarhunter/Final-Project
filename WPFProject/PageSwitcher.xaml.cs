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

namespace WPFProject
{
    /// <summary>
    /// Interaction logic for PageSwitcher.xaml
    /// </summary>
    public partial class PageSwitcher:Window
    {
        private ISwitchable s;
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new MainMenu());
            Switcher.remotechange(1);

        }
        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }
        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable!"
                    + nextPage.Name.ToString());
        }
        public void remotechangepage(int command)
        {
            s = this.Content as ISwitchable;
            s.sendcommand(command);
        }
    }
}
