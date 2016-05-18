using System.Windows.Controls;

namespace WpfApplication1
{
    public static class Switcher
    {
        public static PageSwitcher pageSwitcher;

        public static void Switch(UserControl newPage) 
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, object state) 
        {
            pageSwitcher.Navigate(newPage, state);
        }

        public static void starteeg()
        {
            pageSwitcher.starteeg();
        }

        public static void remotechange(int command)
        {
            pageSwitcher.remotechangepage(command);
        }

        public static void loadsetting()
        {
            pageSwitcher.showsetting();
        }

        public static void turnonlight()
        {
            pageSwitcher.turnlighon();
        }
        public static void turnofflight()
        {
            pageSwitcher.turnlighoff();
        }
    }
}
