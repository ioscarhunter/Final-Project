using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emotiv;
using System.Threading;

namespace EEG_Reader
{
    public partial class Form1 : Form
    {
        EEG_Logger p;

        public Form1()
        {
            InitializeComponent();

            Console.WriteLine("EEG Data Reader Example");
            p = new EEG_Logger();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(i);
                    p.Run();
                    Thread.Sleep(500);
                }
            }
            catch (NotConnectException error)
            {
                Console.WriteLine("not connect");
            }
        }
    }
}
