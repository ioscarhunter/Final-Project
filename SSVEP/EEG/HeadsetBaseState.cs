using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{

    public enum SignalStrength_enum
    {
        NO_SIGNAL,  //
        BAD_SIGNAL,
        GOOD_SIGNAL
    }
    public enum EEG_ContactQuality_enum
    {
        EEG_CQ_NO_SIGNAL, //black
        EEG_CQ_VERY_BAD, //red
        EEG_CQ_POOR,    //orange
        EEG_CQ_FAIR,    //yellow
        EEG_CQ_GOOD  //green
    }

    //public class HeadsetBaseData
    //    {
    //        public float TimeStamp;
    //        public SignalStrength_enum SignalStrength;
    //        public bool HeadsetOn;

    //        public int BatteryChargeLevel;
    //        public int BatteryChargeMax;
    //        public int? BatteryChargePercentage;
    //    }
       
    
}
