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

namespace Mp3Page
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaplayer = new MediaPlayer();
        private int play_status = 0;
        private string track1 = "C:/Users/Compark/Desktop/Fast.mp3";
        private string track2 = "C:/Users/Compark/Desktop/English.mp3";
     

        public MainWindow()
        {


            InitializeComponent();
            listbox1.Items.Add("Fast");
            listbox1.Items.Add("English");
            mediaplayer.Open(new Uri(track1));
            mediaplayer.Play();
            Console.WriteLine(mediaplayer.Volume);
            
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (play_status == 0)
            {
                mediaplayer.Pause();
                play_status = 1;
            }
            else
            {
                mediaplayer.Play();
                play_status = 0;
            }


        }


        private void listbox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                e.Handled = true;
            }
        }

        private void listbox1_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                e.Handled = true;
            }
        }

      

        private void listbox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                mediaplayer.Open(new Uri(track2));
                mediaplayer.Play();
               
                
                
            }
        }

        private void listbox1_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                mediaplayer.Open(new Uri(track1));
                mediaplayer.Play();
               
            }

        }

        private void listbox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)listbox1.SelectedItem == "Fast")
            {
                mediaplayer.Open(new Uri(track1));
                mediaplayer.Play();
             
            }
            else
            {
                mediaplayer.Open(new Uri(track2));
                mediaplayer.Play();
                
             }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
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
    }
}
