using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emotiv;

//  a sample of training cognitiv action  : neutral and push 

namespace CognitivTrainingActionSample
{
    public partial class Form1 : Form
    {
        uint userId = 0;
        bool neutral = true;
        bool push = false;
        bool pull = false;
        bool write = false;
        int numOfActions = 0;
        Profile userProfile = new Profile();        

        public static EdkDll.EE_CognitivAction_t[] cognitivActionList = {
                                                                     EdkDll.EE_CognitivAction_t.COG_NEUTRAL,
                                                                     EdkDll.EE_CognitivAction_t.COG_PUSH,
                                                                     EdkDll.EE_CognitivAction_t.COG_PULL,
                                                                     EdkDll.EE_CognitivAction_t.COG_LIFT,
                                                                     EdkDll.EE_CognitivAction_t.COG_DROP,
                                                                     EdkDll.EE_CognitivAction_t.COG_LEFT,
                                                                     EdkDll.EE_CognitivAction_t.COG_RIGHT,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_LEFT,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_RIGHT,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_CLOCKWISE,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_COUNTER_CLOCKWISE,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_FORWARDS,
                                                                     EdkDll.EE_CognitivAction_t.COG_ROTATE_REVERSE,
                                                                     EdkDll.EE_CognitivAction_t.COG_DISAPPEAR
                                                                 };
        
        public Form1()
        {
            InitializeComponent();
            
            EmoEngine.Instance.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(Instance_EmoStateUpdated);
            EmoEngine.Instance.CognitivEmoStateUpdated += new EmoEngine.CognitivEmoStateUpdatedEventHandler(Instance_CognitivEmoStateUpdated);
            EmoEngine.Instance.CognitivTrainingStarted += new EmoEngine.CognitivTrainingStartedEventEventHandler(Instance_CognitivTrainingStarted);
            EmoEngine.Instance.CognitivTrainingSucceeded += new EmoEngine.CognitivTrainingSucceededEventHandler(Instance_CognitivTrainingSucceeded);
            EmoEngine.Instance.CognitivTrainingCompleted += new EmoEngine.CognitivTrainingCompletedEventHandler(Instance_CognitivTrainingCompleted);           
                        
            EmoEngine.Instance.Connect();
            
        }

        void StartCognitivTraining(EdkDll.EE_CognitivAction_t cognitivAction)
        {
            if (cognitivAction == EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
            {
                Console.WriteLine("Neutral Training");
                //    EmoEngine.Instance.CognitivSetActiveActions((uint)userId, 0x0000);
            }
            else if (cognitivAction == EdkDll.EE_CognitivAction_t.COG_PUSH)
            {
                Console.WriteLine("Push Training");
                //    EmoEngine.Instance.CognitivSetActiveActions((uint)userId, 0x0002);
            }
            else if (cognitivAction == EdkDll.EE_CognitivAction_t.COG_PULL)
            {
                Console.WriteLine("Pull Training");
                //    EmoEngine.Instance.CognitivSetActiveActions((uint)userId, 0x0004);
            }

            EmoEngine.Instance.CognitivSetTrainingAction(userId,cognitivAction);
            EmoEngine.Instance.CognitivSetTrainingControl(userId, EdkDll.EE_CognitivTrainingControl_t.COG_START);
        }

        void SetActiveActions()
        {
            uint cognitivActions = 0x0000;
            for (int i = 1; i < cognitivActionList.Length; i++)
            {
                cognitivActions = cognitivActions | (uint)cognitivActionList[i];
            }
            //EmoEngine.Instance.CognitivSetActiveActions(userId, cognitivActions);
        }

        void Instance_CognitivTrainingCompleted(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("Cognitiv Training completed");
            if (numOfActions == 1) push = true;
            if (numOfActions == 2) pull = true;
            if(write) EmoEngine.Instance.EE_SaveUserProfile((uint)userId, "_hieu.emu");                        
        }

        void Instance_CognitivTrainingSucceeded(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("Cognitiv Training Succeeded");
            EmoEngine.Instance.CognitivSetTrainingControl(userId, EdkDll.EE_CognitivTrainingControl_t.COG_ACCEPT);
        }

        void Instance_CognitivTrainingStarted(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("Cognitiv Training started");
        }

        void Instance_CognitivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            if (numOfActions > 2)
            {
                Console.WriteLine("Cognitiv EmoState Updated...");
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
        }

        void Instance_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {            
            //Console.WriteLine("EmoState Updated..."); 
           
            if (neutral)
            {
                EmoEngine.Instance.CognitivSetActiveActions((uint)userId, 0x0000 | 0x0002 | 0x0004);
                StartCognitivTraining(EdkDll.EE_CognitivAction_t.COG_NEUTRAL);
                neutral = false;
                numOfActions++;
            }
            if (push)
            {
                StartCognitivTraining(EdkDll.EE_CognitivAction_t.COG_PUSH);
                push = false;
                numOfActions++;
            }
            if (pull)
            {
                StartCognitivTraining(EdkDll.EE_CognitivAction_t.COG_PULL);
                pull = false;
                write = true;
                numOfActions++;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EmoEngine.Instance.ProcessEvents();
        }
    }
}
