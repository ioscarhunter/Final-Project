using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using AForge.Math;

namespace WpfApplication1
{
    class SignalProcessing
    {
        //public float[] Process(float[] input)
        //{
        //    float[] filteredSamples = HighPassFilter(input);
        //    float[] windowedSamples = HannigWindowing(filteredSamples);
        //    float[] transformedSamples = FastFourierTransform(windowedSamples);
        //    return transformedSamples;
        //}

        public static double[] HighPassFilter(double[] input)
        {
            double fCut = 0.16F;
            const double W = 2.0F * 128;

            fCut *= 6.28318530717959F; // 2.0F * Math.Pi
            double norm = 1.0F / (fCut + W);
            double a0 = W * norm;
            double a1 = -a0;
            double b1 = (W - fCut) * norm;

            double[] output = new double[input.Length];

            for (int i = 0;i < input.Length - 1;i++)
            {
                if (i - 1 > 0)
                    output[i] = input[i] * a0 + input[i - 1] * a1 + output[i - 1] * b1;
            }

            return output;
        }

        public static float[] HannigWindowing(float[] filteredSamples)
        {
            float[] hanningWindow = new float[filteredSamples.Length];
            for (int i = 0;i < filteredSamples.Length;i++)
            {
                float multiplier = Convert.ToSingle(0.5 * (1 - Math.Cos(2 * Math.PI * i / filteredSamples.Length)));
                hanningWindow[i] = multiplier * filteredSamples[i];
            }

            return hanningWindow;
        }

        //private float[] FastFourierTransform(float[] windowedSamples)
        //{
        //    Complex[] complex = new Complex[1024];
        //    for (int i = 0;i < 1024 - 1;i++)
        //    {
        //        complex[i] = new Complex(windowedSamples[i], 0);
        //    }

        //    FourierTransform.FFT(complex, FourierTransform.Direction.Forward);
        //    return complex.Select(x => Convert.ToSingle(x.Re)).ToArray();
        //}
    }
}
