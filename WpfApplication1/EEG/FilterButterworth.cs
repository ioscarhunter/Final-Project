using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class FilterButterworth
    {
        /// <summary>
        /// rez amount, from sqrt(2) to ~ 0.1
        /// </summary>
        private readonly double resonance;

        private readonly double frequency;
        private readonly int sampleRate;
        private readonly PassType passType;

        private readonly double c, a1, a2, a3, b1, b2;

        /// <summary>
        /// Array of input values, latest are in front
        /// </summary>
        private double[] inputHistory = new double[2];

        /// <summary>
        /// Array of output values, latest are in front
        /// </summary>`
        private double[] outputHistory = new double[3];

        public FilterButterworth(double frequency, int sampleRate, PassType passType, double resonance)
        {
            this.resonance = resonance;
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;

            switch (passType)
            {
                case PassType.Lowpass:
                    c = 1.0f / (double) Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = 2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (1.0f - c * c) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                case PassType.Highpass:
                    //c = (double) Math.Tan(Math.PI * frequency / sampleRate);
                    //a1 = 1.0f / (1.0f + resonance * c + c * c);
                    //a2 = -2f * a1;
                    //a3 = a1;
                    //b1 = 2.0f * (c * c - 1.0f) * a1;
                    //b2 = (1.0f - resonance * c + c * c) * a1;

                    frequency *= 6.28318530717959F; // 2.0F * Math.Pi
                    c = 1.0F / (frequency + 2.0 * sampleRate);
                    a1 = 2.0*sampleRate * c;
                    a2 = -a1;
                    b1 = ((2.0 * sampleRate) - frequency) * c;
                    break;
            }
        }

        public enum PassType
        {
            Highpass,
            Lowpass,
        }

        public double Update(double newInput)
        {
            double newOutput = (newInput * a1) + (this.inputHistory[0] * a2) +(this.outputHistory[0] * b1);
            //double newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];

            this.inputHistory[1] = this.inputHistory[0];
            this.inputHistory[0] = newInput;

            this.outputHistory[2] = this.outputHistory[1];
            this.outputHistory[1] = this.outputHistory[0];
            this.outputHistory[0] = newOutput;
            return this.outputHistory[0];
        }

        public double Value
        {
            get { return this.outputHistory[0]; }
        }
    }
}
