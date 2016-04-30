using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Emotiv;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Frequencytest.Logger
{

	class NotConnectException : Exception { }
	public class EEG_Logger
	{
		EmoEngine engine; // Access to the EDK is viaa the EmoEngine 
		int userID = -1; // userID is used to uniquely identify a user's headset
		string filename;
		string dir;

		uint userId = 0;
		Profile profile = new Profile();
		public string profileName = "";

		public bool isLoad = true;
		public bool oneTime = false;

		SignalProcessing sn = new SignalProcessing();

		int timeinsec;


		public EEG_Logger(int time, int freq, String prefix)
		{
			timeinsec = time;
			dir = ".\\" + DateTime.Now.ToString("MMddyy");
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			filename = dir + "\\" + prefix + "-" + (timeinsec - 11) + "," + freq + "-" + DateTime.Now.ToString("MMddyy-hhmmss") + ".csv"; // output filename
																																		  // create the engine
			engine = EmoEngine.Instance;
			engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
			engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(Instance_EmoStateUpdated);

			// connect to Emoengine.            
			engine.Connect();

			// create a header for our output file
			WriteHeader();

		}

		void Instance_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
		{
			if (isLoad)
			{
				LoadUP();
				isLoad = false;
			}
		}

		public void LoadUP()
		{
			engine.LoadUserProfile(userId, ".//starboy.emu");
			profile = engine.GetUserProfile((uint)userId);
			engine.SetUserProfile(userId, profile);
		}

		void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
		{
			Console.WriteLine("User Added Event has occured" + e.userId);

			// record the user 
			userID = (int)e.userId;

			// enable data aquisition for this user.
			engine.DataAcquisitionEnable((uint)userID, true);

			// ask for up to 1 second of buffered data
			engine.EE_DataSetBufferSizeInSec(timeinsec);

		}

		public void connect()
		{
			// Handle any waiting events
			engine.ProcessEvents();

		}

		public void Run()
		{

			Console.WriteLine(engine.EE_DataGetBufferSizeInSec());
			Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);
			if (data == null)
				return;

			int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;

			Console.WriteLine("Writing " + _bufferSize.ToString() + " lines of data ");

			// Write the data to a file
			TextWriter file = new StreamWriter(filename, true);
			EdkDll.EE_DataChannel_t[] selectedchannel = new EdkDll.EE_DataChannel_t[] { EdkDll.EE_DataChannel_t.MARKER, EdkDll.EE_DataChannel_t.O1, EdkDll.EE_DataChannel_t.O2 };


			//data[EdkDll.EE_DataChannel_t.O1] = sn.HighPassFilter(data[EdkDll.EE_DataChannel_t.O1], 0.3);
			//data[EdkDll.EE_DataChannel_t.O2] = sn.HighPassFilter(data[EdkDll.EE_DataChannel_t.O2], 0.3);


			for (int i = 0; i < _bufferSize; i++)
			{
				foreach (EdkDll.EE_DataChannel_t channel in selectedchannel)
				{
					// now write the data
					if (channel == EdkDll.EE_DataChannel_t.MARKER && data[channel][i] == 0)
						file.Write(" " + ",");
					else
						file.Write(data[channel][i] + ",");

				}
				file.WriteLine("");

			}
			file.Close();

		}

		public void WriteHeader()
		{
			TextWriter file = new StreamWriter(filename, false);

			string header = "MARKER, O1, O2, ";

			file.WriteLine(header);
			file.Close();
		}

		public void setMarker(int mark)
		{
			engine.DataSetMarker((uint)userID, mark);
			//Console.WriteLine(mark);
		}
	}
}