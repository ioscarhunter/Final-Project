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
       
        double[] output;
        public double[] Process(double[] input)
        {
            //double[] filteredSamples = HighPassFilter(input);
            //double[] windowedSamples = HannigWindowing(filteredSamples);
            double[] transformedSamples = FastFourierTransform(input);
            return transformedSamples;
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
            for (int i = 0;i < input.Length;i++)
            {
                output[i] = input[i] / max;
            }
            return output;
        }

        public double standard_deviation(double[] input)
        {
            double[] temp = new double[input.Length];
            double average = input.Average();
            for (int i = 0;i < input.Length;i++)
            {
                temp[i] = Math.Pow((input[i] - average), 2);
            }
            //double sumOfSquaresOfDifferences = input.Select(val => (val - average) * (val - average)).Sum();
            double sum = temp.Sum();
            double sd = Math.Sqrt(sum / input.Length);
            return sd;
        }

        public double[] HighPassFilter(double[] input)
        {
            double fCut = 0.16F;
            double W = 2.0F * 128;

            fCut *= 6.28318530717959F; // 2.0F * Math.Pi
            double norm = 1.0F / (fCut + W);
            double a0 = W * norm;
            double a1 = -a0;
            double b1 = (W - fCut) * norm;

            if (output == null)
                output = new double[input.Length];

            for (int i = 1;i < input.Length;i++)
            {
                output[i] = input[i] * a0 + input[i - 1] * a1 + output[i - 1] * b1;
            }

            return output;
        }

        public double[] zerostandard(double[] input)
        {
            double[] output = new double[input.Length];
            double sd = standard_deviation(input);
            double average = input.Average();
            for(int i = 0;i < input.Length;i++)
            {
                output[i] = (input[i] - average) / sd;
            }
            return output;
        }

        public double[] HannigWindowing(double[] filteredSamples)
        {
            double[] hanningWindow = new double[filteredSamples.Length];
            for (int i = 0;i < filteredSamples.Length;i++)
            {
                double multiplier = Convert.ToSingle(0.5 * (1 - Math.Cos(2 * Math.PI * i / filteredSamples.Length)));
                hanningWindow[i] = multiplier * filteredSamples[i];
            }

            return hanningWindow;
        }
        private double[] FastFourierTransform(double[] windowedSamples)
        {
            double[] magnitude = new double[windowedSamples.Length];
            Complex[] complex = new Complex[windowedSamples.Length];
            for (int i = 0;i < windowedSamples.Length;i++)
            {
                complex[i] = new Complex(windowedSamples[i], 0);
            }

            FourierTransform.FFT(complex, FourierTransform.Direction.Forward);
            for (int i = 0;i < windowedSamples.Length;i++)
            {
                magnitude[i] = complex[i].Magnitude;
            }
            //return Array.ConvertAll(complex.Select(x => Convert.ToSingle(x.Re)).ToArray(), x=>(double)x);
            return magnitude;
        }
    }
}
