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

namespace VideoPage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int status = 0;
        public MainWindow()
        {
            InitializeComponent();
            //IsPlaying(false);
            MediaPlayer.Source = new Uri("C:/Users/Compark/Desktop/2.mp4");
            MediaPlayer.Play();
            Console.WriteLine(MediaPlayer.Volume);
           
        }
        
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (status == 0)
            {
                MediaPlayer.Pause();
                status = 1;
            }
            else {
                MediaPlayer.Play();
                status = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Volume >= 0 && MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume += 0.25;
                Volume.Value += 0.25;
            }
            
            else if (MediaPlayer.Volume >= 1)
             {
                    MediaPlayer.Volume = 1;
             }
            Console.WriteLine(MediaPlayer.Volume);
            Console.WriteLine(Volume.Value);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Volume > 0 && MediaPlayer.Volume <= 1)
            {
                MediaPlayer.Volume -= 0.25;
                Volume.Value -= 0.25;
            }
            else if (MediaPlayer.Volume <= 0)
            {
                    MediaPlayer.Volume = 0;
            }
            
            Console.WriteLine(MediaPlayer.Volume);
            Console.WriteLine(Volume.Value);
        }
    }
}


        

      
       

        
        
        

     
       
    

