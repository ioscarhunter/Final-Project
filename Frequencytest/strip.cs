using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frequencytest.Logger;

namespace Frequencytest
{
	class strip
	{
		String filePath;

		bool INdata = false;
		bool predata = false;
		SignalProcessing sn = new SignalProcessing();
		int sample = 128;

		int count = 0;

		public strip(String filename)
		{
			double[] outnum = new double[sample];
			double[] tempo1 = new double[sample];
			double[] tempo2 = new double[sample];
			double[] preout = new double[sample];

			int Row = 0;
			int count = 0;
			int preRowCount = 0;
			int preCount = 0;

			filePath = filename;
			StreamReader sr = new StreamReader(filePath + ".csv");
			string[] Line = sr.ReadLine().Split(',');

			while (!sr.EndOfStream)
			{
				Line = sr.ReadLine().Split(',');


				if ((Line[0].Equals(" ")))
				{

				}
				else if (Line[0].Equals("1"))
				{
					predata = true;
				}
				else if (INdata)
				{
					foreach (var i in Enumerable.Range(0, 128))
						sr.ReadLine();  //skip

					INdata = false;
				}

				else INdata = true;

				//if (predata )
				//{
				//	double o1 = Double.Parse(Line[1]);
				//	double o2 = Double.Parse(Line[2]);
				//	tempo1[preRowCount] = o1;
				//	tempo2[preRowCount] = o2;
				//	preRowCount++;

				//	if (preRowCount == sample)
				//	{
				//		preCount++;
				//		Console.WriteLine(preCount + " = " + preRowCount);
				//		preRowCount = 0;
				//		tempo1 = sn.Process(tempo1);
				//		tempo2 = sn.Process(tempo2);
				//		for (int i = 0; i < outnum.Length; i++)
				//		{
				//			preout[i] += tempo1[i];
				//			preout[i] += tempo2[i];

				//		}
				//		if(preCount == 9)
				//		{
				//			predata = false;
				//		}
				//	}

				//}

				if (INdata)
				{
					//Console.WriteLine(Line[0] + "," + Line[1] + "," + Line[2]);
					double o1 = Double.Parse(Line[1]);
					double o2 = Double.Parse(Line[2]);
					tempo1[Row] = o1;
					tempo2[Row] = o2;
					Row++;

					if (Row == sample)
					{

						count++;
						Console.WriteLine(count + " = " + Row);
						Row = 0;
						tempo1 = sn.Process(tempo1);
						tempo2 = sn.Process(tempo2);
						for (int i = 0; i < outnum.Length; i++)
						{
							outnum[i] += tempo1[i];
							outnum[i] += tempo2[i];

						}

					}
					//Console.WriteLine(Row + "," + o1 + "," + o2);
				}

			}
			TextWriter outfile = new StreamWriter(filePath + "-d-ft.csv", true);
			for (int i = 0; i < outnum.Length; i++)
			{
				outnum[i] /= count;
				preout[i] /= preCount;
				outfile.WriteLine(i +/* "," + preout[i] + */"," + outnum[i] + ",");
			}
			outfile.Close();
		}
	}
}
