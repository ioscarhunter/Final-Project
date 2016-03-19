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
		int sample = 4096;

		int count = 0;
		string header = ",Rest,Active";

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
					//foreach (var i in Enumerable.Range(0, 128))
					//	sr.ReadLine();  //skip

					INdata = false;
				}

				else {
					INdata = true;
					predata = false;
				}

				if (predata)
				{
					double o1 = Double.Parse(Line[1]);
					double o2 = Double.Parse(Line[2]);
					tempo1[preRowCount] = o1;
					tempo2[preRowCount] = o2;
					preRowCount++;

					if (preRowCount == 1260)
					{
						preCount++;
						Console.WriteLine(preCount + " = " + preRowCount);

						tempo1 = sn.Process(tempo1);
						tempo2 = sn.Process(tempo2);
						for (int i = 0; i < outnum.Length; i++)
						{
							preout[i] += tempo1[i];
							preout[i] += tempo2[i];

						}
						predata = false;
						if (preCount == 9)
						{

						}
					}

				}

				if (INdata)
				{
					//Console.WriteLine(Line[0] + "," + Line[1] + "," + Line[2]);
					double o1 = Double.Parse(Line[1]);
					double o2 = Double.Parse(Line[2]);
					tempo1[Row] = o1;
					tempo2[Row] = o2;
					Row++;

					if (Row == 2540)
					{

						count++;
						Console.WriteLine(count + " = " + Row);

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

			double ldelta = 0;
			double ltheta = 0;
			double lalpha = 0;
			double lbeta = 0;

			double ldelta2 = 0;
			double ltheta2 = 0;
			double lalpha2 = 0;
			double lbeta2 = 0;

			double hdelta = 0;
			double htheta = 0;
			double halpha = 0;
			double hbeta = 0;

			double hdelta2 = 0;
			double htheta2 = 0;
			double halpha2 = 0;
			double hbeta2 = 0;

			TextWriter outfile = new StreamWriter(filePath + "-d-ft-all.csv", false);

			

			for (int i = 0; i < outnum.Length / 2; i++)
			{
				double freq = (i / 32.0);
				if (freq >= 0.1 && freq < 2.25)
				{
					ldelta += (outnum[i]);
					ldelta2 += (preout[i]);
				}

				if (freq >= 2.25 && freq <= 3.5)
				{
					hdelta += (outnum[i]);
					hdelta2 += (preout[i]);
				}

				if (freq >= 4.0 && freq < 5.75)
				{
					ltheta += (outnum[i]);
					ltheta2 += (preout[i]);
				}
				if (freq >= 5.75 && freq <= 7.5)
				{
					htheta += (outnum[i]);
					htheta2 += (preout[i]);
				}

				if (freq >= 8.0 && freq < 10.5)
				{
					lalpha += (outnum[i]);
					lalpha2 += (preout[i]);
				}
				if (freq >= 10.5 && freq <= 13.0)
				{
					halpha += (outnum[i]);
					halpha2 += (preout[i]);
				}

				if (freq >= 14 && freq < 22)
				{
					lbeta += (outnum[i]);
					lbeta2 += (preout[i]);
				}
				if (freq >= 22 && freq <= 30)
				{
					hbeta += (outnum[i]);
					hbeta2 += (preout[i]);
				}
				outnum[i] /= count;
				preout[i] /= preCount;
				outfile.WriteLine(i + "," + freq + "," + preout[i] + "," + outnum[i] + "," + (outnum[i] / preout[i]) + ",");
			}
			outfile.Close();
			TextWriter outfile_chart = new StreamWriter(filePath + "-d-ft-chart.csv", false);
			outfile_chart.WriteLine(header);
			outfile_chart.WriteLine("L Delta," + ldelta2 + "," + ldelta);
			outfile_chart.WriteLine("H Delta," + hdelta2 + "," + hdelta);
			outfile_chart.WriteLine("L Theta," + ltheta2 + "," + ltheta);
			outfile_chart.WriteLine("H Theta," + htheta2 + "," + htheta);
			outfile_chart.WriteLine("L Alpha," + lalpha2 + "," + lalpha);
			outfile_chart.WriteLine("H Alpha," + halpha2 + "," + halpha);
			outfile_chart.WriteLine("L Beta," + lbeta2 + "," + lbeta);
			outfile_chart.WriteLine("H Beta," + hbeta2 + "," + hbeta);
			outfile_chart.Close();

		}
	}
}
