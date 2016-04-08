using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Math;

namespace Frequencytest.Logger
{
	class SignalProcessing
	{
		const double sample_rate = 128;
		public double[] Process(double[] input)
		{
			//double[] filteredSamples = HighPassFilter(input);

			//input = zerostandard(input);
			//double[] windowedSamples = HannigWindowing(normalized);


			input = HammingWindowing(input);
			input = FastFourierTransform(input);

			//input = convertToLog(input);

			return input;
		}
		public double[] normalization(double[] input)
		{
			double[] output = new double[input.Length];
			double max = input.Max();
			double min = input.Min();
			if (Math.Abs(min) > max)
			{
				max = Math.Abs(min);
			}
			for (int i = 0; i < input.Length; i++)
			{
				output[i] = input[i] / max;
			}
			return output;
		}

		public double standard_deviation(double[] input)
		{
			double[] temp = new double[input.Length];
			double average = input.Average();
			for (int i = 0; i < input.Length; i++)
			{
				temp[i] = Math.Pow((input[i] - average), 2);
			}
			//double sumOfSquaresOfDifferences = input.Select(val => (val - average) * (val - average)).Sum();
			double sum = temp.Sum();
			double sd = Math.Sqrt(sum / input.Length);
			return sd;
		}

		public double[] HighPassFilter(double[] input, double fCut)
		{
			double W = 2.0F * 128;

			fCut *= 6.28318530717959F; // 2.0F * Math.Pi
			double norm = 1.0F / (fCut + W);
			double a0 = W * norm;
			double a1 = -a0;
			double b1 = (W - fCut) * norm;

			double[] output = new double[input.Length];

			for (int i = 1; i < input.Length; i++)
			{
				output[i] = input[i] * a0 +
							input[i - 1] * a1 +
							output[i - 1] * b1;
			}

			return output;
		}

		public double[] LowPassFilter(double[] input, double fCut)
		{
			const float W = 2.0F * 128;

			fCut = 1 / (fCut * 6.28318530717959F); // 2.0F * Math.Pi
			double norm = 1.0F / (fCut + W);
			double a0 = W * norm;
			double a1 = a0;
			double b1 = (W - fCut) * norm;

			double[] output = new double[input.Length];

			for (int i = 0; i < input.Length - 1; i++)
			{
				if (i - 1 > 0)
					output[i] = input[i] * a0 + input[i - 1] * a1 + output[i - 1] * b1;
			}

			return output;


		}

		public double[] HighPassFilter(double[] input, double fCut, double r)
		{

			double c = Math.Tan(Math.PI * fCut / sample_rate);

			double a1 = 1.0 / (1.0 + r * c + c * c);
			double a2 = -2 * a1;
			double a3 = a1;
			double b1 = 2.0 * (c * c - 1.0) * a1;
			double b2 = (1.0 - r * c + c * c) * a1;

			double[] output = new double[input.Length];

			for (int i = 2; i < input.Length; i++)
			{
				output[i] = input[i] * a1 +
							input[i - 1] * a2 +
							input[i - 2] * a3 -
							output[i - 1] * b1 -
							output[i - 2] * b2;
			}

			return output;
		}

		public double[] LowPassFilter(double[] input, double fCut, double r)
		{
			double c = 1 / Math.Tan(Math.PI * fCut / sample_rate);

			double a1 = 1.0 / (1.0 + r * c + c * c);
			double a2 = 2 * a1;
			double a3 = a1;
			double b1 = 2.0 * (1.0 - c * c) * a1;
			double b2 = (1.0 - r * c + c * c) * a1;

			double[] output = new double[input.Length];

			for (int i = 2; i < input.Length; i++)
			{
				output[i] = input[i] * a1 +
							input[i - 1] * a2 +
							input[i - 2] * a3 -
							output[i - 1] * b1 -
							output[i - 2] * b2;
			}

			return output;

		}



		public double[] zerostandard(double[] input)
		{
			double[] output = new double[input.Length];
			double sd = standard_deviation(input);
			double average = input.Average();
			for (int i = 0; i < input.Length; i++)
			{
				output[i] = (input[i] - average) / sd;
			}
			return output;
		}

		public double[] HannigWindowing(double[] filteredSamples)
		{
			double[] hanningWindow = new double[filteredSamples.Length];
			for (int i = 0; i < filteredSamples.Length; i++)
			{
				double multiplier = Convert.ToSingle(0.5 * (1 - Math.Cos(2 * Math.PI * i / filteredSamples.Length)));
				hanningWindow[i] = multiplier * filteredSamples[i];
			}

			return hanningWindow;
		}

		public double[] HammingWindowing(double[] filteredSamples)
		{
			double alpha = 0.54, beta = 1 - alpha;
			double[] hammingWindow = new double[filteredSamples.Length];
			for (int i = 0; i < filteredSamples.Length; i++)
			{
				double multiplier = Convert.ToSingle(alpha - beta * Math.Cos((2 * Math.PI * i) / (filteredSamples.Length - 1)));
				hammingWindow[i] = multiplier * filteredSamples[i];
			}

			return hammingWindow;
		}

		private double[] FastFourierTransform(double[] windowedSamples)
		{
			double[] magnitude = new double[windowedSamples.Length];
			Complex[] complex = new Complex[windowedSamples.Length];
			for (int i = 0; i < windowedSamples.Length; i++)
			{
				complex[i] = new Complex(windowedSamples[i], 0);
			}

			FourierTransform.FFT(complex, FourierTransform.Direction.Forward);
			for (int i = 0; i < windowedSamples.Length; i++)
			{
				magnitude[i] = Math.Pow(complex[i].Magnitude, 2);
			}
			//return Array.ConvertAll(complex.Select(x => Convert.ToSingle(x.Re)).ToArray(), x=>(double)x);
			return magnitude;
		}

		public double[] MovingAverage(double[] data, int period)
		{
			double[] buffer = new double[period];
			double[] output = new double[data.Length];
			int current_index = 0;
			for (int i = 0; i < data.Length; i++)
			{
				buffer[current_index] = data[i] / period;
				double ma = 0.0;
				for (int j = 0; j < period; j++)
				{
					ma += buffer[j];
				}
				output[i] = ma;
				current_index = (current_index + 1) % period;
			}
			return output;
		}


		public double[] convertToLog(double[] data)
		{
			double[] output = new double[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				output[i] = 10*Math.Log10(data[i]);
			}
				return output;
		}
	}
}
