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
        private double time_stamp = 0;

        private double gyroX;
        private double gyroY;

        int[] interest = new int[] { 0, 1, 9, 10 };

        String[] eachSignal = new String[16];

        public System.Timers.Timer _timer;

        private SignalProcessing sn = new SignalProcessing();

        private const int timesec = 18;
        private const int times = 7;
        private int samplecollect = 64;
        int[] count = new int[8];

        double[][] a_data = new double[8][];
        double[][][] threedata = new double[8][][];
        double[][][] zthreedata = new double[8][][];
        double[][] z_data = new double[8][];

        double[] temp_o1 = new double[128 * timesec];
        double[] temp_marker = new double[128 * timesec];

        public EEG_Logger()
        {
            // create the engine
            engine = EmoEngine.Instance;
            linkEmo();

            // connect to Emoengine.            
            engine.Connect();

            // create a header for our output file
            //WriteHeader();


            for (int i = 0;i < a_data.Length;i++)
            {
                a_data[i] = new double[samplecollect];
            }
            for (int i = 0;i < threedata.Length;i++)
            {
                threedata[i] = new double[times + 10][];
                zthreedata[i] = new double[times + 10][];
                for (int j = 0;j < threedata[i].Length;j++)
                {
                    threedata[i][j] = new double[samplecollect];
                    zthreedata[i][j] = new double[samplecollect];
                }
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

            // ask for up to 7 second of buffered data
            engine.EE_DataSetBufferSizeInSec(timesec);

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
            _timer.Interval = 100;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(processEvent);
            _timer.Enabled = true;
            //engine.EE_DataSetBufferSizeInSec(7);
        }


        public void setmarker(int mark)
        {
            engine.DataSetMarker((uint) userID, mark);
            //Console.WriteLine(mark);
        }

        public void getEEG()
        {

            Console.WriteLine(engine.EE_DataGetBufferSizeInSec());
            Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint) userID);
            if (data == null)
                return;

            int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;

            Console.WriteLine("Writing " + _bufferSize.ToString() + " lines of data ");


            fastcopy(data[EdkDll.EE_DataChannel_t.O1], data[EdkDll.EE_DataChannel_t.MARKER]);
            writedata();
            filteroutdata();
            printtofile();
            analysis();
            clear_temp();

        }

        private void fastcopy(double[] input_o1, double[] input_time)
        {
            //Console.WriteLine(input_o1.Length);
            Array.Copy(input_o1, 0, temp_o1, 0, input_o1.Length);
            Array.Copy(input_time, 0, temp_marker, 0, input_time.Length);
            temp_o1 = sn.HighPassFilter(temp_o1);

        }

        private void filteroutdata()
        {
            Console.WriteLine("filter");
            for (int i = 0;i < temp_o1.Length;i++)
            {
                if (temp_marker[i] != 0)
                {
                    int countnum = 0;
                    double[] nom = new double[samplecollect];
                    double[] zero = new double[samplecollect];

                    for (int j = 0;j < samplecollect;j++)
                    {
                        //Console.WriteLine(nom[j]);
                        if (temp_marker[i + j + 1] != 0) { Console.WriteLine(temp_marker[i + j + 1]); break; }
                        //a_data[(int) temp_marker[i]][j] += nom[j] / times;
                        countnum++;
                    }
                    Array.Copy(temp_o1, i, nom, 0, countnum);
                    Array.Copy(temp_o1, i, zero, 0, countnum);

                    nom = sn.normalization(nom);
                    zero = sn.zerostandard(zero);

                    Array.Copy(nom, 0, threedata[(int) temp_marker[i]][count[(int) temp_marker[i]]], 0, countnum);
                    Array.Copy(zero, 0, zthreedata[(int) temp_marker[i]][count[(int) temp_marker[i]]], 0, countnum);
                    count[(int) temp_marker[i]]++;
                    Console.WriteLine(count[(int) temp_marker[i]]);
                    i += samplecollect-1;
                }
            }
        }

        private void printtofile()
        {
            Console.WriteLine("ptf");
            TextWriter file1 = new StreamWriter("filterednormal.csv", true);
            TextWriter file2 = new StreamWriter("filteredzero.csv", true);
            TextWriter file3 = new StreamWriter("normal.csv", true);
            TextWriter file4 = new StreamWriter("zmean.csv", true);
            for (int i = 1;i < a_data.Length;i += 2)    //LED num
            {

                //double sd = sn.standard_deviation(a_data[i]);
                for (int j = 0;j < a_data[i].Length;j++)    //sample
                {

                    if ((threedata[i][0][j] == threedata[i][1][j]) && (threedata[i][1][j] == threedata[i][2][j]) && threedata[i][1][j] == 0)
                    {
                        break;
                    }
                    else
                    {
                        double origi = 0;
                        double zerom = 0;
                        //file2.WriteLine(i + ", " + a_data[i][j] + ", " /*+ sd + ", "*/);
                        file3.Write(i + ", ");
                        file4.Write(i + ", ");
                        for (int k = 0;k < count[i];k++)    //times
                        {
                            origi += threedata[i][k][j] / count[i];
                            zerom += zthreedata[i][k][j] / count[i];

                            file3.Write(threedata[i][k][j] + ", ");
                            file4.Write(zthreedata[i][k][j] + ", ");

                        }
                        a_data[i][j] = zerom;
                        file3.WriteLine();
                        file4.WriteLine();
                        file1.WriteLine(i + ", " + origi + ", ");
                        file2.WriteLine(i + ", " + zerom + ", ");
                    }
                }
            }
            file1.Close();
            file2.Close();
            file3.Close();
            file4.Close();
        }

        private void clear_temp()
        {
            temp_o1 = new double[128 * timesec];
            temp_marker = new double[128 * timesec];

            count = new int[8];

            for (int i = 0;i < threedata.Length;i++)
            {
                threedata[i] = new double[times + 10][];
                zthreedata[i] = new double[times + 10][];
                for (int j = 0;j < threedata[i].Length;j++)
                {
                    threedata[i][j] = new double[samplecollect];
                    zthreedata[i][j] = new double[samplecollect];
                }
            }

            for (int i = 0;i < a_data.Length;i++)
            {
                a_data[i] = new double[samplecollect];
            }
        }

        public void analysis()
        {
            double[] pos38 = new double[8];
            double[] pos39 = new double[8];
            double[] pos40 = new double[8];
            int[] score = new int[8];
            for (int i = 0;i < a_data.Length;i++)
            {
                pos38[i] = a_data[i][37];
                pos39[i] = a_data[i][38];
                pos40[i] = a_data[i][39];
            }
            double maxofmax38 = pos38.Max();
            int maxIndex38 = pos38.ToList().IndexOf(maxofmax38);
            score[maxIndex38]++;
            double maxofmax39 = pos39.Max();
            int maxIndex39 = pos39.ToList().IndexOf(maxofmax39);
            score[maxIndex39]++;
            double maxofmax40 = pos40.Max();
            int maxIndex40 = pos40.ToList().IndexOf(maxofmax40);
            score[maxIndex40]++;

            int which = score.Max();
            if (which >= 2)
            {
                OnledUpdate(score.ToList().IndexOf(which));
                
            }
            else
            {
                OnledUpdate(-1);
            }
        }

        public void writedata()
        {
            Console.WriteLine("write)");
            TextWriter file2 = new StreamWriter("raw.csv", true);
            for (int i = 0;i < temp_marker.Length;i++)
            {
                //Console.WriteLine(temp_marker[i]);
                file2.WriteLine(temp_marker[i] + ", " + temp_o1[i] + ", ");
            }
            file2.Close();
        }

        public void which(double[] indata, int led, int times)
        {
            Console.WriteLine("L:" + indata.Length);
            for (int j = 0;j < indata.Length;j++)
            {
                //Console.WriteLine((oCsvList[i + j][1]));
                a_data[led][j] += (indata[j] / times);
            }
            count[led]++;
        }

        public void fft()
        {
            TextWriter file = new StreamWriter("fft.csv", true);
            Console.WriteLine("FFT");
            for (int i = 0;i < count.Length;i++)
            {

                if (count[i] != 0)
                {
                    double[] fft = new double[64];
                    //data[i] = diff(data[i]);
                    fft = sn.Process(a_data[i]);
                    for (int k = 0;k < 64;k++)
                    {
                        //Console.WriteLine(data[i][k]);
                        file.WriteLine(i + ", " + fft[k]);
                    }
                    count[i] = 0;
                }

            }
            file.Close();
            findmax();

        }

        private void findmax()
        {
            //TextWriter file = new StreamWriter("result.csv", true);
            double[] max = new double[count.Length];
            for (int m = 0;m < count.Length;m++)
            {
                //max[m] = data[m][10]; Console.WriteLine(m + " " + max[10]);
                Console.WriteLine(m + ":" + a_data[m][5]);
                if (a_data[m][4] < a_data[m][5] && a_data[m][5] > a_data[m][6]) { max[m] = a_data[m][5]; Console.WriteLine(m + " " + max[m]); }
                a_data[m] = new double[64];
            }
            double maxofmax = max.Max();
            int maxIndex = max.ToList().IndexOf(maxofmax);
            Console.WriteLine(maxIndex + ":" + maxofmax);
            OnledUpdate(maxIndex);
            //file.WriteLine(maxIndex + ", ");
            //file.Close();
        }

        private double[] diff(double[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0;i < input.Length - 1;i++)
            {
                output[i] = input[i + 1] - input[i];
            }
            return output;

        }


        private void doublestore(double[] input, int led)
        {
            int start = 0;
            if (count[led] == 1)
            {
                start = 64;
                count[led] = 0;
            }
            else count[led]++;

            for (int i = start;i < start + 64;i++)
            {
                a_data[led][i] = input[i - start];
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

            int deltax;
            int deltay;
            engine.HeadsetGetGyroDelta((uint) userID, out deltax, out deltay);
            gyroX += radtodec(deltax / 100.0);
            gyroY += radtodec(deltay / 100.0);
            Console.WriteLine(gyroX + ", " + gyroY);
            OnStatusUpdate(timeFromStart,
                headsetOn, signalStrength.ToString(), chargeLevel, maxChargeLevel, eachSignal);

        }
        private double radtodec(double rad)
        {
            return (rad * 180) / Math.PI;
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