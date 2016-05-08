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
		int windowsSize = 256;
		int sample = 256;
		int emotivSample = 128;


		int multiplyer;
		int times;

		int totaltime = 20;



		int count = 0;
		string chartHeader = ",Rest,Active";
		string folderPath;

		string processPath = "processed\\";
		string chartPath = "chart\\";
		string subChartPath = "subchart\\";

		double[][] outnum;
		double[][] active;

		double[][] difrent;
		double[][] peaks;

		double[] tempo1;
		double[] tempo2;
		double[] preout;
		double[] freq;

		int lowbound = 5;
		int highbond = 31;
		int sec = 20;

		public static T[] SubArray<T>(T[] data, int indexfrom, int indexto)
		{
			T[] result = new T[indexto - indexfrom];
			Array.Copy(data, indexfrom, result, 0, indexto - indexfrom);
			return result;
		}

		private double[][] readtoArray()
		{
			string[] Line = File.ReadAllLines(folderPath + filePath + ".csv");
			double[][] data = new double[Line.Length-1][];
			for (int i = 1; i < data.Length-1; i++)
			{
				data[i-1] = new double[3];
				string[] temp = Line[i].Split(',');
				if (!temp[0].Equals(" "))
					data[i-1][0] = Convert.ToDouble(temp[0]);
				data[i-1][1] = Convert.ToDouble(temp[1]);
				data[i-1][2] = Convert.ToDouble(temp[2]);
			}
			return data;
		}

		private void readfile()
		{
			int Row = 0;
			int count = 0;
			int preRowCount = 0;
			int preCount = 0;

			StreamReader sr = new StreamReader(folderPath + filePath + ".csv");
			string[] Line = sr.ReadLine().Split(',');


			while (!sr.EndOfStream)
			{

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

						//tempo1 = sn.MovingAverage(tempo1, 3);
						//tempo2 = sn.MovingAverage(tempo2, 3);


						for (int i = 0; i < outnum[0].Length; i++)
						{
							preout[i] += ((tempo1[i] + tempo2[i]) / (2.0 * 5.0));
						}

						//preout = sn.HighPassFilter(preout, 1, 1);
						//preout = sn.Process(preout);
						preout = sn.MovingAverage(preout, 3);

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
						try
						{
							for (int i = 0; i < outnum[count].Length; i++)
							{
								outnum[count][i] += (tempo1[i] + tempo2[i]) / 2.0;
							}

							count++;
						}
						catch (IndexOutOfRangeException e)
						{
							INdata = false;
						}
					}
					//Console.WriteLine(Row + "," + o1 + "," + o2);
				}
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

				else
				{
					INdata = true;
					predata = false;
				}
			}

		}

		private void processing()
		{
			preout = sn.RemoveBaseline(preout);
			for (int i = 0; i < active.Length; i++)
			{
				active[i] = new double[windowsSize];
				for (int j = 0; j < active[i].Length; j++)
				{
					active[i][j] = (outnum[2 * i][j] + outnum[(2 * i) + 1][j]) / 2.0;
				}
			}

			// copy array
			Array.Copy(freq, 5 * multiplyer, freq, 0, (highbond - lowbound) * multiplyer);
			Array.Copy(preout, 5 * multiplyer, preout, 0, (highbond - lowbound) * multiplyer);
			Array.Resize(ref freq, (highbond - lowbound) * multiplyer);
			Array.Resize(ref preout, (highbond - lowbound) * multiplyer);
			//Array.Resize(ref outnum, multiplyer);

			difrent = new double[active.Length][];
			peaks = new double[active.Length][];

			for (int i = 0; i < active.Length; i++)
			{
				Array.Copy(active[i], lowbound * multiplyer, active[i], 0, (highbond - lowbound) * multiplyer);
				Array.Resize(ref active[i], (highbond - lowbound) * multiplyer);

				difrent[i] = new double[active[0].Length];
				for (int j = 0; j < active[i].Length; j++)
				{
					difrent[i][j] = active[i][j] - preout[j];
				}
				difrent[i] = sn.zerostandard(difrent[i]);

				peaks[i] = sn.FindPeaks(difrent[i], 3);
			}

		}

		private void writeResult()
		{
			Directory.CreateDirectory(folderPath + processPath);
			Directory.CreateDirectory(folderPath + chartPath);
			Directory.CreateDirectory(folderPath + subChartPath);

			TextWriter outfile = new StreamWriter(folderPath + processPath + filePath + "-d-ft-all.csv", false);
			//TextWriter outfile2 = new StreamWriter(foldername + "\\band\\" + filePath + "-d-band-all.csv", false);

			outfile.Write("Frequency,");
			for (int j = 1; j <= active.Length; j += 1)
			{
				outfile.Write("," + "Baseline " + j + "," + "Active " + j + "," + "Diffrent " + j + "," + "Peak " + j + ",");
			}
			outfile.WriteLine();

			for (int i = 0; i < active[0].Length; i++)
			{
				outfile.Write(freq[i] + ",");
				for (int j = 0; j < active.Length; j += 1)
				{
					if (peaks[j][i] != 0)
						outfile.Write("," + preout[i] + "," + active[j][i] + "," + difrent[j][i] + "," + peaks[j][i] + ",");
					else
						outfile.Write("," + preout[i] + "," + active[j][i] + "," + difrent[j][i] + "," + ",");
				}
				outfile.WriteLine();
			}

			outfile.Close();


			TextWriter outfile_sub = new StreamWriter(folderPath + subChartPath + filePath + "-d-ft-chart.csv", false);
			//TextWriter outfile2 = new StreamWriter(foldername + "\\band\\" + filePath + "-d-band-all.csv", false);

			outfile_sub.Write("Frequency,");
			for (int j = 1; j <= active.Length; j += 1)
			{
				outfile_sub.Write("Trial " + j + ",");
			}
			outfile_sub.WriteLine();

			for (int i = 0; i < active[0].Length; i++)
			{
				outfile_sub.Write(freq[i] + ",");
				for (int j = 0; j < active.Length; j += 1)
				{
					//if (peaks[j][i] != 0)
					outfile_sub.Write(peaks[j][i] + ",");
					//else
					//	outfile_sub.Write(",");
				}
				outfile_sub.WriteLine();
			}

			outfile_sub.Close();

			//TextWriter outfile_sub_chart = new StreamWriter(folderPath + subChartPath + filePath + "-d-ft-chart.csv", false);

			//double[] stimu = new double[] { 8, 11, 14, 17, 20, 23, 26, 29 };
			//double wide = 1.0;

			//foreach (double f in stimu)
			//{
			//	int low = Array.FindIndex(freq, x => x == f - wide);
			//	int high = Array.FindIndex(freq, x => x == f + wide);
			//	outfile_sub_chart.Write((f - wide) + " to " + (f + wide));
			//	for (int i = 0; i < difrent.Length; i++)
			//	{
			//		outfile_sub_chart.Write("," + sn.FindPeaks(SubArray(difrent[i], low, high), 5));
			//	}
			//	outfile_sub_chart.WriteLine(",");

			//}

			//outfile_sub_chart.Close();

			//TextWriter outfile_chart = new StreamWriter(folderPath + chartPath + filePath + "-d-ft-chart.csv", false);
			//outfile_chart.WriteLine(chartHeader);
			//outfile_chart.WriteLine("elta," + delta2 + "," + delta);

			//outfile_chart.WriteLine("Theta," + theta2 + "," + theta);

			//outfile_chart.WriteLine("Alpha," + alpha2 + "," + alpha);

			//outfile_chart.WriteLine("Beta," + beta2 + "," + beta);

			//outfile_chart.Close();
		}

		private void init(String folder, String filename)
		{
			multiplyer = (int)Math.Ceiling(windowsSize / (double)emotivSample);
			times = (int)Math.Ceiling(emotivSample * totaltime / (double)windowsSize);

			folderPath = folder;
			filePath = filename;

			outnum = new double[times][];
			active = new double[times / 2][];
			for (int i = 0; i < outnum.Length; i++)
			{
				outnum[i] = new double[windowsSize];
			}
			tempo1 = new double[windowsSize];
			tempo2 = new double[windowsSize];
			preout = new double[windowsSize];
			freq = new double[windowsSize / 2];

			for (int i = 0; i < freq.Length; i++)
			{
				freq[i] = (i / (double)multiplyer);
			}
		}


		public strip(String folder, String filename)
		{


			init(folder, filename);
			readtoArray();
			readfile();
			processing();
			writeResult();



		}
	}
}
