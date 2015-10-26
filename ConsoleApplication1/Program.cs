using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {



        static void Main(string[] args)
        {

            SignalProcessing sn = new SignalProcessing();

            string sFileContents = "";

            using (StreamReader oStreamReader = new StreamReader(File.OpenRead("outfile_oscar_strobe_500ms_4_1.csv")))
            {
                sFileContents = oStreamReader.ReadToEnd();
            }

            List<string[]> oCsvList = new List<string[]>();

            string[] sFileLines = sFileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string sFileLine in sFileLines)
            {
                oCsvList.Add(sFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            int[] count = new int[4];

            double[][] data = new double[4][];
            for (int i = 0;i < data.Length;i++)
            {
                data[i] = new double[64];
            }

            for (int i = 256;i < oCsvList.Count;i += 64)
            {
                int led = 0;
                led = Int32.Parse(oCsvList[i][0]);

                try
                {
                    for (int j = 0;j < 64;j++)
                    {
                        //Console.WriteLine((oCsvList[i + j][1]));
                        data[led][j] += (Double.Parse(oCsvList[i + j][1])/3);
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
                    if(count.Max() == 0)
                    {
                        double[] max = new double[count.Length];
                        for(int m = 0;m < count.Length;m++)
                        {
                            double[] temp = new double[3];
                            Array.Copy(data[m], 5, temp, 0, 1);
                            max[m] = temp.Max();
                            data[m] = new double[64];
                        }
                        double maxofmax = max.Max();

                    }
                }
                catch (Exception e)
                {
                    break;
                }
            }

            //int iColumnNumber = 0;
            //int iRowNumber = 0;

            //Console.WriteLine("Column{0}, Row{1} = \"{2}\"", iColumnNumber, iRowNumber, oCsvList[iColumnNumber][iRowNumber]);

            //iColumnNumber = 4;
            //iRowNumber = 2;

            //Console.WriteLine("Column{0}, Row{1} = \"{2}\"", iColumnNumber, iRowNumber, oCsvList[iColumnNumber][iRowNumber]);

        }
    }
}
