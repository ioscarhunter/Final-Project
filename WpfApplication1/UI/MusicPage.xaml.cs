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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MusicPage.xaml
    /// </summary>
    public partial class MusicPage : UserControl,ISwitchable
    {
        private MediaPlayer mediaplayer = new MediaPlayer();
        private int music_status = 0;
        private string track1 = "C:/Users/i_osc/Desktop/Fast.mp3";
        private string track2 = "C:/Users/i_osc/Desktop/English.mp3";
        public MusicPage()
        {
            InitializeComponent();
            listbox1.Items.Add("Fast");
            listbox1.Items.Add("English");
            mediaplayer.Open(new Uri(track1));
            mediaplayer.Play();
            
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void back_button(object sender, System.Windows.RoutedEventArgs e)
        {
            Switch_back();
        }
        private void Switch_back() {
            mediaplayer.Stop();
            Switcher.Switch(new MainMenu());
        }

        private void inc_button(object sender, RoutedEventArgs e)
        {
            inc_volume();

        }
        private void inc_volume() {
            if (mediaplayer.Volume >= 0 && mediaplayer.Volume < 1)
            {
                mediaplayer.Volume += 0.25;
                Volume.Value += 0.25;
            }

            else if (mediaplayer.Volume >= 1)
            {
                mediaplayer.Volume = 1;
            }
            
        
        }
        private void dec_button(object sender, RoutedEventArgs e)
        {
            dec_volume();
        }
        private void dec_volume() {
            if (mediaplayer.Volume > 0 && mediaplayer.Volume <= 1)
            {
                mediaplayer.Volume -= 0.25;
                Volume.Value -= 0.25;
            }

            else if (mediaplayer.Volume > 0)
            {
                mediaplayer.Volume = 0;
            }
        }
        private void forward_button(object sender, RoutedEventArgs e)
        {

            forward_track();
        }
        private void forward_track() {
            if (music_status == 0)
            {
                mediaplayer.Open(new Uri(track2));
                mediaplayer.Play();
                songname.Content = "English premier";
                artist.Content = "Andy Cole";
                music_status = 1;
            }
            else
            {
                mediaplayer.Open(new Uri(track1));
                songname.Content = "Get low";
                artist.Content = "Dillon Francis";
                mediaplayer.Play();
                music_status = 0;

            }
        }

        public void sendcommand(int command)
        {
            switch (command)
            {
                case 1:
                    Switch_back();
                    break;
                case 2:
                    inc_volume();
                    break;
                case 3:
                    forward_track();
                    break;
                case 4:
                    dec_volume();
                    break;
            }
        }
        #endregion




    }
}
