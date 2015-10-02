using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emotiv;

namespace LoadUserProfile
{
    public partial class Form1 : Form
    {
        uint userId = 0;
        Profile profile = new Profile();
        public string profileName="";       

        public bool isLoad = true;
        public bool oneTime = false;

        EmoEngine engine = EmoEngine.Instance;
        public Form1()
        {
            InitializeComponent();

            EmoEngine.Instance.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(Instance_EmoStateUpdated);
            EmoEngine.Instance.CognitivEmoStateUpdated += new EmoEngine.CognitivEmoStateUpdatedEventHandler(Instance_CognitivEmoStateUpdated);           

            engine.Connect();
        }

        void Instance_CognitivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            EdkDll.EE_CognitivAction_t currentAction = es.CognitivGetCurrentAction();
            if (currentAction == EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
                Console.WriteLine("Current Action is COG_NEUTRAL");
            if (currentAction == EdkDll.EE_CognitivAction_t.COG_PUSH)
                Console.WriteLine("Current Action is COG_PUSH");
            if (currentAction == EdkDll.EE_CognitivAction_t.COG_PULL)
                Console.WriteLine("Current Action is COG_PULL");
            if (currentAction == EdkDll.EE_CognitivAction_t.COG_LIFT)
                Console.WriteLine("Current Action is COG_LIFT");
            float power = es.CognitivGetCurrentActionPower();
            Console.WriteLine("Current action power {0}: " + power);
        }

        void Instance_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {            
            if (isLoad)
            {
                LoadUP();
                isLoad = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }        

        public void LoadUP()
        {            
            engine.LoadUserProfile(userId,"_hieu.emu");
            profile = engine.GetUserProfile((uint)userId);
            engine.SetUserProfile(userId, profile);                       
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            engine.ProcessEvents();
        }
    }
}
