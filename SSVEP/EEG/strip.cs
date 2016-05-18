using System;
using System.IO;
using System.Linq;

namespace WpfApplication1
{
	class strip
	{
		String filePath;

		bool INdata = false;
		bool predata = false;
		SignalProcessing sn = new SignalProcessing();
		int windowsSize = 512;
		int sample = 512;
		int emotivSample = 128;
		double overlapsize = 0.5;

		int multiplyer;
		int times;

		int resttime = 6;
		int activetime = 12;

		int resttimes;
		int activetimes;

		int count = 0;
		string chartHeader = ",Rest,Active";
		string folderPath;

		string diffrentPath = "diffrent\\";
		string chartPath = "chart\\";
		string peakPath = "peak\\";

		double[][] outnum;
		double[][] active;

		double[][] difrent;
		double[][] peaks;

		double[] tempo1;
		double[] tempo2;
		double[] preout;
		double[] freq;

		double bestpeak;

		int lowbound = 11;
		int highbond = 21;
		private double[] tempavg;
		private int[] peakcount;
		private double[] avgall;
		private double[] avgpeak;

		public static T[] SubArray<T>(T[] data, int indexfrom, int indexto)
		{
			T[] result = new T[indexto - indexfrom];
			Array.Copy(data, indexfrom, result, 0, indexto - indexfrom);
			return result;
		}

		private double[][] readtoArray()
		{
			Console.WriteLine(folderPath + filePath + ".csv");
			string[] Line = File.ReadAllLines(folderPath + filePath + ".csv");
			double[][] data = new double[3][];

			data[0] = new double[Line.Length + 100];
			data[1] = new double[Line.Length + 100];
			data[2] = new double[Line.Length + 100];

			for (int i = 1; i < Line.Length - 1; i++)
			{

				string[] temp = Line[i].Split(',');
				if (!temp[0].Equals(" "))
					data[0][i - 1] = Convert.ToDouble(temp[0]);
				data[1][i - 1] = Convert.ToDouble(temp[1]);
				data[2][i - 1] = Convert.ToDouble(temp[2]);
			}
			return data;
		}

		private void processFile(double[][] data)
		{
			int Row = 0;
			int count = 0;
			int preRowCount = 0;
			int preCount = 0;

			double[][] baseline = new double[2][];
			baseline[0] = new double[emotivSample * resttime];
			baseline[1] = new double[emotivSample * resttime];

			double[][] active = new double[2][];
			active[0] = new double[(emotivSample * activetime) + 1];
			active[1] = new double[(emotivSample * activetime) + 1];

			double overlap = Math.Floor(windowsSize * overlapsize);
			resttimes = (int)Math.Floor(emotivSample * resttime / overlap);
			activetimes = (int)Math.Floor(emotivSample * activetime / overlap);

			int startindex = Array.IndexOf(data[0], 1) + 1;
			if (startindex > 500)
			{
				startindex = 0;
			}
			Array.Copy(data[1], startindex, baseline[0], 0, emotivSample * resttime);
			Array.Copy(data[2], startindex, baseline[1], 0, emotivSample * resttime);
			startindex += (emotivSample * resttime) - 100;

			for (int i = startindex; i < startindex + 100; i++)
			{
				if (data[0][i] > 1)
				{
					startindex = i;
					break;
				}
			}
			startindex += 128;
			Array.Copy(data[1], startindex, active[0], 0, emotivSample * activetime);
			Array.Copy(data[2], startindex, active[1], 0, emotivSample * activetime);

			baseline[0] = sn.HighPassFilter(baseline[0], 0.16, 0.5);
			baseline[1] = sn.HighPassFilter(baseline[1], 0.16, 0.5);

			active[0] = sn.HighPassFilter(active[0], 0.16, 0.5);
			active[1] = sn.HighPassFilter(active[1], 0.16, 0.5);

			int index = 0;
			#region baseline
			for (int i = 0; i < resttimes - 1; i++)
			{
				Array.Copy(baseline[0], index, tempo1, 0, windowsSize);
				Array.Copy(baseline[1], index, tempo2, 0, windowsSize);

				//tempo1 = sn.RemoveBaseline(tempo1);
				//tempo2 = sn.RemoveBaseline(tempo2);

				//tempo1 = sn.zerostandard(tempo1);
				//tempo2 = sn.zerostandard(tempo2);

				//for (int j = 0; j < preout.Length; j++)
				//{
				//	tempavg[j] = (tempo1[j] + tempo2[j]) / 2.0;
				//}

				//tempavg = sn.Process(tempavg);
				//tempavg = sn.MovingAverage(tempavg, 3);

				tempo1 = sn.Process(tempo1);
				tempo2 = sn.Process(tempo2);

				tempo1 = sn.MovingAverage(tempo1, 3);
				tempo2 = sn.MovingAverage(tempo2, 3);

				for (int j = 0; j < preout.Length; j++)
				{
					preout[j] += ((tempo1[j] + tempo2[j]) / (2.0 * resttimes - 1));
					//preout[j] += (tempavg[j]) / (resttimes - 1);
				}
				preout = sn.RemoveBaseline(preout);
				index += (int)overlap;
			}
			#endregion

			#region Activ
			index = 0;
			outnum = new double[activetimes][];

			for (int i = 0; i < activetimes; i++)
			{
				outnum[i] = new double[windowsSize];
				try
				{
					Array.Copy(active[0], index, tempo1, 0, windowsSize);
					Array.Copy(active[1], index, tempo2, 0, windowsSize);
				}
				catch (ArgumentException e)
				{
					Array.Copy(active[0], index, tempo1, 0, (int)overlap);
					Array.Copy(active[1], index, tempo2, 0, (int)overlap);
				}

				//tempo1 = sn.zerostandard(tempo1);
				//tempo2 = sn.zerostandard(tempo2);

				tempo1 = sn.Process(tempo1);
				tempo2 = sn.Process(tempo2);

				//for (int j = 0; j < preout.Length; j++)
				//{
				//	tempavg[j] = (tempo1[j] + tempo2[j]) / 2.0;
				//}
				//tempavg = sn.Process(tempavg);

				for (int j = 0; j < windowsSize; j++)
				{
					outnum[i][j] += ((tempo1[j] + tempo2[j]) / (2.0));
					//outnum[i][j] = tempavg[j];
				}
				outnum[i] = sn.RemoveBaseline(outnum[i]);
				index += (int)overlap;
			}
			#endregion
		}

		private int getfreq()
		{
			if (bestpeak >= 12.5 && bestpeak <= 13.5)
			{
				return 13;
			}

			else if (bestpeak > 13.5 && bestpeak <= 14.5)
			{
				return 14;
			}

			else if (bestpeak > 14.5 && bestpeak <= 15.5)
			{
				return 15;
			}
			else if (bestpeak > 15.5 && bestpeak <= 16.5)
			{
				return 16;
			}

			else if (bestpeak > 16.5 && bestpeak <= 17.5)
			{
				return 17;
			}
			else if (bestpeak > 17.5 && bestpeak <= 18.5)
			{
				return 18;
			}

			else return -1;
		}

		internal int getdata()
		{
			int [] freset = PageSwitcher.freqset;
			int index = Array.IndexOf(freset,getfreq());

			switch (index)
			{
				case 1:	return 1;
				case 3:	return 2;
				case 5: return 3;
				case 7:	return 4;
				default: return 0;

			}

			return 0;
		}

		private void processing()
		{
			active = new double[outnum.Length / 2][];

			//preout = sn.RemoveBaseline(preout);
			for (int i = 0; i < active.Length; i++)
			{
				active[i] = new double[windowsSize];
				for (int j = 0; j < active[i].Length; j++)
				{
					active[i][j] = (outnum[2 * i][j] + outnum[(2 * i) + 1][j]) / 2.0;
				}
			}

			// copy array
			Array.Copy(freq, lowbound * multiplyer, freq, 0, (highbond - lowbound) * multiplyer);
			Array.Copy(preout, lowbound * multiplyer, preout, 0, (highbond - lowbound) * multiplyer);
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
				//difrent[i] = sn.windowsZscore(difrent[i],40);

				peaks[i] = sn.FindPeaks(difrent[i], 3);
			}


			avgpeak = new double[active[0].Length];
			peakcount = new int[active[0].Length];
			avgall = new double[active[0].Length];

			for (int i = 0; i < active[0].Length; i++)
			{
				for (int j = 0; j < active.Length; j += 1)
				{
					avgall[i] += difrent[j][i] / (double)active.Length;
					if (peaks[j][i] != 0)
						peakcount[i]++;
					avgpeak[i] += peaks[j][i] / (double)active.Length;
				}
			}
			avgall = sn.FindPeaks(avgall, 3);   //find peak
			double[] interest = new double[] { 13.0, 15.0, 17.0, 19.0, 21.0, 23.0, 25.0, 27.0, 29.0 };
			double[] notinterest = new double[] { 14.0, 16.0, 18.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0 };
			//peakcount = neutron(peakcount, 5, interest);
			//avgall = neutron(avgall, 5, interest);
			//avgpeak = neutron(avgpeak, 5, interest);

			int[] pXa = new int[peakcount.Length];
			double[] max = new double[peakcount.Length];

			for (int i = 0; i < peakcount.Length; i++)
			{
				if (avgall[i] > 0 && peakcount[i] > 0 /*&& !notinterest.Contains(freq[i])*/);
				{
					pXa[i] = peakcount[i];
				}
				max[i] = pXa[i] * (avgpeak[i] + avgall[i]) / 2.0;
			}

			// Positioning max

			int p = Array.IndexOf(max, max.Max());
			Console.WriteLine(freq[p]);
			bestpeak = freq[p];
		}

		private void writeResult()
		{
			Directory.CreateDirectory(folderPath + diffrentPath);
			Directory.CreateDirectory(folderPath + chartPath);
			Directory.CreateDirectory(folderPath + peakPath);

			TextWriter outfile = new StreamWriter(folderPath + diffrentPath + filePath + "-d-ft-all.csv", false);
			//TextWriter outfile2 = new StreamWriter(foldername + "\\band\\" + filePath + "-d-band-all.csv", false);

			outfile.Write("Frequency,,");
			for (int j = 1; j <= active.Length; j += 1)
			{
				outfile.Write("Diffrent " + j + ",");
			}
			outfile.WriteLine("Avg,");

			for (int i = 0; i < active[0].Length; i++)
			{
				outfile.Write(freq[i] + ",,");
				for (int j = 0; j < active.Length; j += 1)
				{
					if (peaks[j][i] != 0)
						outfile.Write(difrent[j][i] + ",");
					else
						outfile.Write(difrent[j][i] + ",");
				}
				outfile.WriteLine(avgall[i] + ",");
			}

			outfile.Close();

			TextWriter outfile_sub = new StreamWriter(folderPath + peakPath + filePath + "-d-ft-chart.csv", false);
			//TextWriter outfile2 = new StreamWriter(foldername + "\\band\\" + filePath + "-d-band-all.csv", false);

			outfile_sub.Write("Frequency,,");
			for (int j = 1; j <= active.Length; j += 1)
			{
				outfile_sub.Write("Trial " + j + ",");
			}
			outfile_sub.WriteLine("Peak Count, Avg Peak, Avg all, multiply");

			for (int i = 0; i < active[0].Length; i++)
			{
				outfile_sub.Write(freq[i] + ",,");
				for (int j = 0; j < active.Length; j += 1)
				{
					//if (peaks[j][i] != 0)
					outfile_sub.Write(peaks[j][i] + ",");
					//else
					//	outfile_sub.Write(",");
				}
				outfile_sub.Write(peakcount[i] + "," + avgall[i]);
				outfile_sub.Write("," + (avgpeak[i] + avgall[i]) / 2.0);
				outfile_sub.Write("," + peakcount[i] * (avgpeak[i] + avgall[i]) / 2.0);


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
			times = (int)Math.Ceiling(emotivSample * activetime / (double)windowsSize);

			folderPath = folder;
			filePath = filename;


			active = new double[times / 2][];

			tempo1 = new double[windowsSize];
			tempo2 = new double[windowsSize];
			preout = new double[windowsSize];
			freq = new double[windowsSize / 2];

			tempavg = new double[windowsSize];

			for (int i = 0; i < freq.Length; i++)
			{
				freq[i] = (i / (double)multiplyer);
			}
		}


		public strip(String folder, String filename)
		{
			init(folder, filename);

			processFile(readtoArray());
			processing();
			writeResult();
		}

		private int[] neutron(int[] input, int size, double[] number)
		{
			int[] output = new int[input.Length];
			for (int i = 0; i < input.Length; i++)
			{
				if (number.Contains(freq[i]))
				{
					for (int j = -((size / 2) - 1); j < (size / 2) - 1; j++)
						output[i] += input[i + j];
				}
			}

			return output;
		}

		private double[] neutron(double[] input, int size, double[] number)
		{
			double[] output = new double[input.Length];
			for (int i = 0; i < input.Length; i++)
			{
				if (number.Contains(freq[i]))
				{
					for (int j = -((size / 2) - 1); j < (size / 2) - 1; j++)
						output[i] += input[i + j];
				}
			}

			return output;
		}
	}
}
