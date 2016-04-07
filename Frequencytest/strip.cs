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
		int sample = 512;

		int count = 0;
		string chartHeader = ",Rest,Active";
		string folderPath;

		string processPath = "processed\\";
		string chartPath = "chart\\";
		string subChartPath = "subchart\\";

		double[][] outnum;
		double[] tempo1;
		double[] tempo2;
		double[] preout;


		public strip(String folder, String filename)
		{

			double delta = 0;
			double theta = 0;
			double alpha = 0;
			double beta = 0;

			double delta2 = 0;
			double theta2 = 0;
			double alpha2 = 0;
			double beta2 = 0;


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

			folderPath = folder;
			filePath = filename;

			double[][] outnum = new double[5][];
			for (int i = 0; i < outnum.Length; i++)
			{
				outnum[i] = new double[sample];
			}
			tempo1 = new double[sample];
			tempo2 = new double[sample];
			preout = new double[sample];

			int Row = 0;
			int count = 0;
			int preRowCount = 0;
			int preCount = 0;

			StreamReader sr = new StreamReader(folderPath + filePath + ".csv");
			string[] Line = sr.ReadLine().Split(',');

			Directory.CreateDirectory(folderPath + processPath);
			Directory.CreateDirectory(folderPath + chartPath);
			Directory.CreateDirectory(folderPath + subChartPath);

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

					if (preRowCount == sample)
					{

						
						Console.WriteLine(preCount + " = " + preRowCount);
						preRowCount = 0;

						tempo1 = sn.Process(tempo1);
						tempo2 = sn.Process(tempo2);

						tempo1 = sn.MovingAverage(tempo1, 3);

						tempo2 = sn.MovingAverage(tempo2, 3);
						tempo1 = sn.MovingAverage(tempo1, 3);
						tempo2 = sn.MovingAverage(tempo2, 3);



						for (int i = 0; i < outnum[0].Length; i++)
						{
							preout[i] += ((tempo1[i] + tempo2[i]) / (2.0 * 9.0));
						}

						//preout = sn.HighPassFilter(preout, 1, 1);
						//preout = sn.Process(preout);
						//preout = sn.MovingAverage(preout, 7);

						if (preCount == 4)
						{
							predata = false;
						}
						else
						{
							preCount++;
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

					if (Row == sample)
					{

						
						Console.WriteLine(count + " = " + Row);
						Row = 0;

						//tempo1 = sn.HighPassFilter (tempo1, 3, 1);
						//tempo2 = sn.HighPassFilter(tempo2, 3, 1);
						tempo1 = sn.Process(tempo1);
						tempo2 = sn.Process(tempo2);

						for (int i = 0; i < outnum[count].Length; i++)
						{
							outnum[count][i] += (tempo1[i] + tempo2[i]) / 2.0;
						}

						count++;
					}
					//Console.WriteLine(Row + "," + o1 + "," + o2);
				}

			}

			TextWriter outfile = new StreamWriter(folderPath + processPath + filePath + "-d-ft-all.csv", false);
			//TextWriter outfile2 = new StreamWriter(foldername + "\\band\\" + filePath + "-d-band-all.csv", false);

			for (int i = 0; i < outnum[0].Length / 2; i++)
			{
				double freq = (i / 4.0);
				if (freq >= 5 && freq <= 20)
					outfile.Write(freq + ",");
				for (int j = 0; j < outnum.Length; j++)
				{

					if (freq >= 0.1 && freq < 2.25)
					{
						ldelta += (outnum[j][i]);
						ldelta2 += (preout[i]);

						delta += (outnum[j][i]);
						delta2 += (preout[i]);
					}

					if (freq >= 2.25 && freq <= 3.5)
					{
						hdelta += (outnum[j][i]);
						hdelta2 += (preout[i]);

						delta += (outnum[j][i]);
						delta2 += (preout[i]);
					}

					if (freq >= 4.0 && freq < 5.75)
					{
						ltheta += (outnum[j][i]);
						ltheta2 += (preout[i]);

						theta += (outnum[j][i]);
						theta2 += (preout[i]);
					}
					if (freq >= 5.75 && freq <= 7.5)
					{
						htheta += (outnum[j][i]);
						htheta2 += (preout[i]);

						theta += (outnum[j][i]);
						theta2 += (preout[i]);
					}

					if (freq >= 8.0 && freq < 10.5)
					{
						lalpha += (outnum[j][i]);
						lalpha2 += (preout[i]);

						alpha += (outnum[j][i]);
						alpha2 += (preout[i]);
					}
					if (freq >= 10.5 && freq <= 13.0)
					{
						halpha += (outnum[j][i]);
						halpha2 += (preout[i]);

						alpha += (outnum[j][i]);
						alpha2 += (preout[i]);
					}

					if (freq >= 14 && freq < 22)
					{
						lbeta += (outnum[j][i]);
						lbeta2 += (preout[i]);

						beta += (outnum[j][i]);
						beta2 += (preout[i]);
					}
					if (freq >= 22 && freq <= 30)
					{
						hbeta += (outnum[j][i]);
						hbeta2 += (preout[i]);

						beta += (outnum[j][i]);
						beta2 += (preout[i]);
					}

					if (freq >= 5 && freq <= 20)
					{
						outfile.Write("," + preout[i] + "," + outnum[j][i] + "," + (outnum[j][i] - preout[i]) + ",");
					}
				}
				if (freq >= 5 && freq <= 20)
					outfile.WriteLine();
			}

			outfile.Close();

			TextWriter outfile_sub_chart = new StreamWriter(folderPath + subChartPath + filePath + "-d-ft-chart.csv", false);
			outfile_sub_chart.WriteLine(chartHeader);
			outfile_sub_chart.WriteLine("LDelta," + ldelta2 + "," + ldelta);
			outfile_sub_chart.WriteLine("HDelta," + hdelta2 + "," + hdelta);
			outfile_sub_chart.WriteLine("LTheta," + ltheta2 + "," + ltheta);
			outfile_sub_chart.WriteLine("HTheta," + htheta2 + "," + htheta);
			outfile_sub_chart.WriteLine("LAlpha," + lalpha2 + "," + lalpha);
			outfile_sub_chart.WriteLine("HAlpha," + halpha2 + "," + halpha);
			outfile_sub_chart.WriteLine("LBeta," + lbeta2 + "," + lbeta);
			outfile_sub_chart.WriteLine("HBeta," + hbeta2 + "," + hbeta);
			outfile_sub_chart.Close();

			TextWriter outfile_chart = new StreamWriter(folderPath + chartPath + filePath + "-d-ft-chart.csv", false);
			outfile_chart.WriteLine(chartHeader);
			outfile_chart.WriteLine("elta," + delta2 + "," + delta);

			outfile_chart.WriteLine("Theta," + theta2 + "," + theta);

			outfile_chart.WriteLine("Alpha," + alpha2 + "," + alpha);

			outfile_chart.WriteLine("Beta," + beta2 + "," + beta);

			outfile_chart.Close();

		}
	}
}
