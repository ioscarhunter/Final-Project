using System;
using System.Collections.Generic;
using Emotiv;
using System.IO;


namespace WpfApplication1
{
	public class EEG_Logger
	{
		public int led_num { private get; set; }

		public event EventHandler<EEG_LoggerEventArgs> DataUpdate;
		public event EventHandler<EEG_StatusEventArgs> StatusUpdate;
		public event EventHandler<EEG_WhichEventArgs> whichsUpdate;
		public event EventHandler<EEG_GyroEventArgs> GyroUpdate;

		String[] eachSignal = new String[16];

		EmoEngine engine; // Access to the EDK is viaa the EmoEngine 
		int userID = -1; // userID is used to uniquely identify a user's headset
		string filename;
		string folder;

		uint userId = 0;
		Profile profile = new Profile();
		public string profileName = "";

		int[] interest = new int[] { 0, 1, 9, 10 };

		public bool isLoad = true;
		public bool oneTime = false;


		int timeinsec;


		public EEG_Logger(int time, String prefix)
		{
			int freq = 0;
			timeinsec = time;
			folder = ".\\" + DateTime.Now.ToString("MMddyy") + "\\";
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			filename = prefix + "-" + (timeinsec - 11) + "," + freq + "-" + DateTime.Now.ToString("MMddyy-hhmmss") + ".csv"; // output filename
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
			TextWriter file = new StreamWriter(folder + filename, true);
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

			strip s = new strip(folder, filename.TrimEnd(".csv".ToCharArray()));
			OnledUpdate(s.getdata());
		}

		public void WriteHeader()
		{
			TextWriter file = new StreamWriter(folder + filename, false);

			string header = "MARKER, O1, O2, ";

			file.WriteLine(header);
			file.Close();
		}

		public void setMarker(int mark)
		{
			engine.DataSetMarker((uint)userID, mark);
			//Console.WriteLine(mark);
		}

		
		private void processEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Handle any waiting events
			engine.ProcessEvents();
			//Console.WriteLine(userID);
			// If the user has not yet connected, do not proceed
			if ((int)userID == -1)
			{
				//return;
				Console.WriteLine("return");
				throw new NotConnectException();
			}
		}


		public void setmarker(int mark)
		{
			engine.DataSetMarker((uint)userID, mark);
			//Console.WriteLine(mark);
		}



		public bool isuserconnect()
		{
			if ((int)userID == -1)
				return false;
			else return true;
		}

		private void OnledUpdate(int led)
		{
			Console.WriteLine(led);
			//Console.WriteLine("update Var");
			if (whichsUpdate != null)
				whichsUpdate(this, new EEG_WhichEventArgs(led));
		}


		private void OnDataUpdate(double[] data_o1, double[] data_o2)
		{
			//Console.WriteLine("update Var");
			if (DataUpdate != null)
				DataUpdate(this, new EEG_LoggerEventArgs(data_o1, data_o2));
		}

		private void OnStatusUpdate(Single time, int headsetstatus, String signal, int battery, int maxbatt, String[] eachSignal)
		{
			//Console.WriteLine("update Status");
			if (StatusUpdate != null)
				StatusUpdate(this, new EEG_StatusEventArgs(time, headsetstatus, signal, battery, maxbatt, eachSignal));
		}

		private void OngyroUpdate(double x, double y)
		{
			//Console.WriteLine("update Var");
			if (GyroUpdate != null)
				GyroUpdate(this, new EEG_GyroEventArgs(x, y));
		}

		void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
		{
			EmoState es = e.emoState;

			Single timeFromStart = 0;
			timeFromStart = es.GetTimeFromStart();
			//Int32 numCqChan = es.GetNumContactQualityChannels();
			Int32 headsetOn;
			headsetOn = es.GetHeadsetOn();

			//EdkDll.EE_EEG_ContactQuality_t[] cq = es.GetContactQualityFromAllChannels();
			for (int i = 0; i < interest.Length; ++i)
			{
				//if (interest.Contains(i))
				//{
				eachSignal[interest[i]] = es.GetContactQuality(interest[i]).ToString();
				//}
				//if (cq[i] != es.GetContactQuality(i))
				//{
				//    throw new Exception();
				//}
			}

			EdkDll.EE_SignalStrength_t signalStrength = es.GetWirelessSignalStatus();
			Int32 chargeLevel = 0;
			Int32 maxChargeLevel = 0;
			es.GetBatteryChargeLevel(out chargeLevel, out maxChargeLevel);

			//Console.Write(
			//    "{0},{1},{2},{3},{4},",
			//    timeFromStart,
			//    headsetOn, signalStrength, chargeLevel, maxChargeLevel);

			//for (int i = 0;i < cq.Length;++i)
			//{
			//    Console.Write("i = {0},", cq[i]);
			//}

			int deltax;
			int deltay;
			engine.HeadsetGetGyroDelta((uint)userID, out deltax, out deltay);
			OngyroUpdate(radtodec(deltax / 100.0), radtodec(deltay / 100.0));
			OnStatusUpdate(timeFromStart,
				headsetOn, signalStrength.ToString(), chargeLevel, maxChargeLevel, eachSignal);

		}


		private double radtodec(double rad)
		{
			return (rad * 180) / Math.PI;
		}
	}



	public class EEG_LoggerEventArgs : EventArgs
	{
		public double[] Data_O1 { get; private set; }
		public double[] Data_O2 { get; private set; }
		public EEG_LoggerEventArgs(double[] data_o1, double[] data_o2)
		{
			Data_O1 = data_o1;
			Data_O2 = data_o2;
		}
	}

	public class EEG_WhichEventArgs : EventArgs
	{

		public int lednum { get; private set; }
		public EEG_WhichEventArgs(int led)
		{
			lednum = led;
		}
	}

	public class EEG_StatusEventArgs : EventArgs
	{
		public Single timePass;
		public int headsetOn;
		public String signalStrength;
		public Int32 chargeLevel;
		public Int32 maxChargeLevel;
		public String[] eSignal;
		public EEG_StatusEventArgs(Single time, int headsetstatus, String signal, int battery, int maxbatt, String[] eachSignal)
		{
			timePass = time;
			headsetOn = headsetstatus;
			signalStrength = signal;
			chargeLevel = battery;
			maxChargeLevel = maxbatt;
			eSignal = eachSignal;

		}

	}

	public class EEG_GyroEventArgs : EventArgs
	{
		public double gyrox;
		public double gyroy;
		public EEG_GyroEventArgs(double x, double y)
		{
			gyrox = x;
			gyroy = y;
		}
	}
}