using System;
using System.Collections.Generic;
using Emotiv;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace WpfApplication1
{

    public class EEG_Logger
    {

        public int led_num { private get; set; }

        public event EventHandler<EEG_LoggerEventArgs> DataUpdate;
        public event EventHandler<EEG_StatusEventArgs> StatusUpdate;
        public event EventHandler<EEG_WhichEventArgs> whichsUpdate;

        private EmoEngine engine; // Access to the EDK is viaa the EmoEngine 
        private int userID = -1; // userID is used to uniquely identify a user's headset
        private string filename = ".\\outfile.csv"; // output filename

        private uint userId = 0;
        private Profile profile = new Profile();
        private string profileName = "";


        private bool isLoad = true;
        private bool oneTime = false;

        private int IIR_TC = 256; // 2 second memory - adjust as required 
        private double back_o1 = 0;
        private double back_o2 = 0;

        private double data_o1 = 0;
        private double data_o2 = 0;

        int[] interest = new int[] { 0, 1, 9, 10 };

        String[] eachSignal = new String[16];

        public System.Timers.Timer _timer;

        private SignalProcessing sn = new SignalProcessing();

        int[] count = new int[4];

        double[][] data = new double[4][];



        public EEG_Logger()
        {
            // create the engine
            engine = EmoEngine.Instance;
            linkEmo();

            // connect to Emoengine.            
            engine.Connect();

            // create a header for our output file
            //WriteHeader();


            for (int i = 0;i < data.Length;i++)
            {
                data[i] = new double[64];
            }
        }

        private void linkEmo()
        {
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
            engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(Instance_EmoStateUpdated);
            engine.EmoEngineEmoStateUpdated += new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoEngineEmoStateUpdated);
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
            profile = engine.GetUserProfile((uint) userId);
            engine.SetUserProfile(userId, profile);
        }

        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("User Added Event has occured" + e.userId);

            // record the user 
            userID = (int) e.userId;

            // enable data aquisition for this user.
            engine.DataAcquisitionEnable((uint) userID, true);

            // ask for up to 1 second of buffered data
            engine.EE_DataSetBufferSizeInSec(1);

        }

        private void processEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Handle any waiting events
            engine.ProcessEvents();
            //Console.WriteLine(userID);
            // If the user has not yet connected, do not proceed
            if ((int) userID == -1)
            {
                //return;
                Console.WriteLine("return");
                throw new NotConnectException();
            }
        }

        public void Run()
        {
            //while(true)
            engine.ProcessEvents();

            _timer = new System.Timers.Timer();
            _timer.Interval = 0.1;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(processEvent);
            _timer.Enabled = true;
        }

        public void getEEG(int max, int sample, int start)
        {

            Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint) userID);
            if (data == null)
                return;

            int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;

            //if (back_o1 == 0 && back_o2 == 0)
            //{
            //    back_o1 = data[EdkDll.EE_DataChannel_t.O1].Average();
            //    back_o2 = data[EdkDll.EE_DataChannel_t.O2].Average();
            //}

            Console.WriteLine("Writing " + _bufferSize.ToString() + " lines of data ");

            // Write the data to a file
            //TextWriter file = new StreamWriter(filename, true);

            data[EdkDll.EE_DataChannel_t.O1] = sn.HighPassFilter(data[EdkDll.EE_DataChannel_t.O1]);
            data[EdkDll.EE_DataChannel_t.O2] = sn.HighPassFilter(data[EdkDll.EE_DataChannel_t.O2]);
            OnDataUpdate(data[EdkDll.EE_DataChannel_t.O1], data[EdkDll.EE_DataChannel_t.O2]);

            double[] temp = new double[64];

            for (int i = 0;i < max;i++)
            {
                // now write the data
                //foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                //    file.Write(data[channel][i] + ",");
                //file.WriteLine("");

                // now write the data
                //file.Write(start + (i / sample) + ", ");
                if (i < _bufferSize)
                {

                    Array.Copy(data[EdkDll.EE_DataChannel_t.O1], temp, max);
                    which(temp, start + (i / sample));
                    //foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                    //{
                    //    if (channel == EdkDll.EE_DataChannel_t.O1)
                    //    {
                    //        //back_o1 = (back_o1 * (IIR_TC - 1) + data[channel][i]) / IIR_TC;
                    //        //data_o1 = data[channel][i] - back_o1;
                    //        data_o1 = data[channel][i];



                    //        //file.Write(data_o1 + ", ");
                    //        file.Write(data_o1 + ", ");
                    //        //Console.Write(data_o1 + ", ");
                    //        //OnDataUpdate(data_o1, data_o2);
                    //    }
                    //    else if (channel == EdkDll.EE_DataChannel_t.O2)
                    //    {
                    //        //back_o2 = (back_o2 * (IIR_TC - 1) + data[channel][i]) / IIR_TC;
                    //        //data_o2 = data[channel][i] - back_o2;
                    //        data_o2 = data[channel][i];
                    //        //file.Write(data_o2 + ", ");
                    //        file.Write(data_o2 + ", ");
                    //        //Console.Write(data_o2 + ", ");
                    //        //
                    //    }

                    //}

                }
                else
                {
                    Console.WriteLine(start + (i / 42));
                    //file.Write("0, 0");
                }
                //file.WriteLine("");
                //Console.WriteLine("");
            }
            //file.Close();
        }

        public void which(double[] indata, int led)
        {

            for (int j = 0;j < 64;j++)
            {
                //Console.WriteLine((oCsvList[i + j][1]));
                data[led][j] += (indata[j] / 3);
            }
            count[led]++;
            if (count[led] == 3)
            {
                data[led] = sn.Process(data[led]);
                for (int k = 0;k < 64;k++)
                {
                    Console.WriteLine(data[led][k]);
                    //file.WriteLine(led + ", " + data[led][k]);
                }

                count[led] = 0;
            }
            if (count.Max() == 0)
            {
                TextWriter file = new StreamWriter(filename, true);
                double[] max = new double[count.Length];
                for (int m = 0;m < count.Length;m++)
                {
                    double[] temp = new double[3];
                    Array.Copy(data[m], 5, temp, 0, 1);
                    max[m] = temp.Max();
                    data[m] = new double[64];
                }
                double maxofmax = max.Max();
                int maxIndex = max.ToList().IndexOf(maxofmax);
                Console.WriteLine(maxIndex);
                file.WriteLine(maxIndex + ", ");
                file.Close();
            }


        }

        public void WriteHeader()
        {
            TextWriter file = new StreamWriter(filename, false);

            string header = "COUNTER,INTERPOLATED,RAW_CQ,AF3,F7,F3, FC5, T7, P7, O1, O2,P8" +
                            ", T8, FC6, F4,F8, AF4,GYROX, GYROY, TIMESTAMP, ES_TIMESTAMP" +
                            "FUNC_ID, FUNC_VALUE, MARKER, SYNC_SIGNAL,";

            file.WriteLine(header);
            file.Close();
        }

        public bool isuserconnect()
        {
            if ((int) userID == -1)
                return false;
            else return true;
        }

        private void OnledUpdate(int led)
        {
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

        void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;

            Single timeFromStart = 0;
            timeFromStart = es.GetTimeFromStart();
            //Int32 numCqChan = es.GetNumContactQualityChannels();
            Int32 headsetOn;
            headsetOn = es.GetHeadsetOn();

            //EdkDll.EE_EEG_ContactQuality_t[] cq = es.GetContactQualityFromAllChannels();
            for (int i = 0;i < interest.Length;++i)
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
            OnStatusUpdate(timeFromStart,
                headsetOn, signalStrength.ToString(), chargeLevel, maxChargeLevel, eachSignal);

        }

    }

    public class EEG_LoggerEventArgs:EventArgs
    {
        public double[] Data_O1 { get; private set; }
        public double[] Data_O2 { get; private set; }
        public EEG_LoggerEventArgs(double[] data_o1, double[] data_o2)
        {
            Data_O1 = data_o1;
            Data_O2 = data_o2;
        }
    }

    public class EEG_WhichEventArgs:EventArgs
    {

        public int lednum { get; private set; }
        public EEG_WhichEventArgs(int led)
        {
            lednum = led;
        }
    }

    public class EEG_StatusEventArgs:EventArgs
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
}