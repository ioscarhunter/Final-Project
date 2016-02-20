using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frequencytest
{
    class strip
    {
        String filePath;
        int Row = 0;
        bool founddata = false;

        public strip(String filename)
        {
            filePath = filename;
            StreamReader sr = new StreamReader(filePath);

            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split(',');
                if((Line[0].Equals(" "))|| Line[0].Equals("1"))
                {

                }
                else
                {
                    if (founddata) founddata = false;
                    else founddata = true;
                }
                if (founddata)
                {
                    data.Add(Line);
                    Row++;
                    if (Row == 128)
                    {

                    }
                    Console.WriteLine(Row);
                }

            }
        }
    }
}
