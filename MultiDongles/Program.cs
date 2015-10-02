using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace MultiDongles
{
    class Program
    {
        EmoEngine engine;
        static void Main(string[] args)
        {
            Program program = new Program();

            program.mainLoop();
        }

        void mainLoop()
        {
            engine = EmoEngine.Instance;
            engine.EmoStateUpdated += new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);
            engine.Connect();

            while (true)
            {
                engine.ProcessEvents(1000);
            }

        }

        void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {            
            if (e.userId == 0)
            {
                EmoState es = e.emoState;
                Console.WriteLine("{0} ; excitement: {1} " ,e.userId, es.AffectivGetEngagementBoredomScore());
            }
            else if( e.userId == 1)
            {
                EmoState es = e.emoState;
                Console.WriteLine("{0} ; excitement: {1} ", e.userId, es.AffectivGetEngagementBoredomScore());
            }
        }
    }
}
