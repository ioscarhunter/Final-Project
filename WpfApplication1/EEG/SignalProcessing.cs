using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Math;

namespace WpfApplication1
{
    class SignalProcessing
    {
        private FilterButterworth butterfillter = new FilterButterworth(0.16,128,FilterButterworth.PassType.Highpass,Math.PI);
        double[] output;
        public double[] Process(double[] input)
        {
            double[] filteredSamples = HighPassFilter(input);
            double[] windowedSamples = HannigWindowing(filteredSamples);
            double[] transformedSamples = FastFourierTransform(windowedSamples);
            return transformedSamples;
        }

        public double[] Butterworth(double[] input)
        {
            double[] output = new double[input.Length];

            for (int i = 0;i < input.Length;i++)
            {
                output[i] = butterfillter.Update(input[i]);
            }
            return output;
        }

        public double[] HighPassFilter(double[] input)
        {
            double fCut = 0.16F;
            const double W = 2.0F * 128;

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
            Complex[] complex = new Complex[1024];
            for (int i = 0;i < 1024 - 1;i++)
            {
                complex[i] = new Complex(windowedSamples[i], 0);
            }

            FourierTransform.FFT(complex, FourierTransform.Direction.Forward);
            return Array.ConvertAll(complex.Select(x => Convert.ToSingle(x.Re)).ToArray(), x=>(double)x);
        }
    }
}
