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
    /// Interaction logic for MusicPage.xaml
    /// </summary>
    public partial class MusicPage : UserControl,ISwitchable
    {
        private MediaPlayer mediaplayer = new MediaPlayer();
        private int play_status = 0;
        private string track1 = "C:/Users/Compark/Desktop/Fast.mp3";
        private string track2 = "C:/Users/Compark/Desktop/English.mp3";
        public MusicPage()
        {
            InitializeComponent();
            listbox1.Items.Add("Fast");
            listbox1.Items.Add("English");
            mediaplayer.Open(new Uri(track1));
            mediaplayer.Play();
            Console.WriteLine(mediaplayer.Volume);
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void back_button(object sender, System.Windows.RoutedEventArgs e)
        {
            mediaplayer.Stop();
            Switcher.Switch(new MainMenu());
        }
        private void inc_button(object sender, RoutedEventArgs e)
        {
            if (mediaplayer.Volume >= 0 && mediaplayer.Volume < 1)
            {
                mediaplayer.Volume += 0.25;
                Volume.Value += 0.25;
            }

            else if (mediaplayer.Volume >= 1)
            {
                mediaplayer.Volume = 1;
            }
            Console.WriteLine(mediaplayer.Volume);
            Console.WriteLine(Volume.Value);

        }
        private void dec_button(object sender, RoutedEventArgs e)
        {
            if (mediaplayer.Volume > 0 && mediaplayer.Volume <= 1)
            {
                mediaplayer.Volume -= 0.25;
                Volume.Value -= 0.25;
            }

            else if (mediaplayer.Volume > 0)
            {
                mediaplayer.Volume = 0;
            }
            Console.WriteLine(mediaplayer.Volume);
            Console.WriteLine(Volume.Value);

        }
        private void forward_button(object sender, RoutedEventArgs e)
        {
            mediaplayer.Open(new Uri(track2));
            mediaplayer.Play();

        }
        

        #endregion

       
       
       
    }
}
