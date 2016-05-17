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
	public partial class MusicPage : UserControl, ISwitchable
	{
		private MediaPlayer mediaplayer = new MediaPlayer();
		private int music_status = 0;
		private string folder = "D:\\SkyDrive\\Documents\\Visual Studio 2015\\Projects\\examples_DotNet\\build\\Debug\\";

		private string[] track = new string[6];
		int currsong;
		public MusicPage()
		{
			track[1] = "I'm All About You.Aaron Carter.mp3";
			track[0] = "Swear It Again.Westlife.mp3";
			track[2] = "One Better.Aaron Carther.mp3";
			track[3] = "Mandy.Westlife.mp3";
			track[4] = "Shape Of My Heart.Backstreet Boy.mp3";
			track[5] = "Incomplete.Backstreet Boy.mp3";
			currsong = 0;
			InitializeComponent();
			listbox1.Items.Clear();
			for (int i = 1; i < track.Length; i++)
			{
				int j = currsong + i + 1;
				j %= track.Length;
				string[] temp = track[i].Split('.');
				listbox1.Items.Add(temp[0] + " - " + temp[1]);
			}
			
			mediaplayer.Open(new Uri(folder + track[currsong]));
			songname.Content = track[currsong].Split('.')[0];
			artist.Content = track[currsong].Split('.')[1];
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
		private void Switch_back()
		{
			mediaplayer.Stop();
			Switcher.Switch(new MainMenu());
		}

		private void inc_button(object sender, RoutedEventArgs e)
		{
			inc_volume();

		}
		private void inc_volume()
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


		}
		private void dec_button(object sender, RoutedEventArgs e)
		{
			dec_volume();
		}
		private void dec_volume()
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
		}
		private void forward_button(object sender, RoutedEventArgs e)
		{

			forward_track();
		}
		private void forward_track()
		{
			currsong++;
			currsong %= track.Length;
			listbox1.Items.Clear();
			if (music_status == 0)
			{
				
				
				string[] temp = track[currsong].Split('.');
				mediaplayer.Open(new Uri(folder+track[currsong]));
				mediaplayer.Play();
				songname.Content = temp[0];
				artist.Content = temp[1];
				music_status = 1;
			}
			else
			{

				string[] temp = track[currsong].Split('.');
				mediaplayer.Open(new Uri(folder + track[currsong]));
				songname.Content = temp[0];
				artist.Content = temp[1];
				mediaplayer.Play();
				music_status = 0;

			}
			for (int i = 0; i < track.Length; i++)
			{
				int j = currsong + i + 1;
				j %= track.Length;
				string[] temp = track[j].Split('.');
				listbox1.Items.Add(temp[0] + " - " + temp[1]);
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
