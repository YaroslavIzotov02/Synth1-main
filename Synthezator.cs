using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public class Synthesator
    {



        /// <summary>
        /// 
        /// </summary>
        private List<Generator> carriers = new List<Generator>();
        private long time;

        private double freq;

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns>double int</returns>
        public double GetOut()
        {
            double res = 0;
            time++;
            for (int i = 0; i < carriers.Count; i++)
            {
                res += carriers[i].GetOut();
            }
            res = res / carriers.Count;
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Synthesator(double f)
        {
            freq = f;
            time = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public void AddCarrier(Generator carrier)
        {
            if (carrier != null)
            {
                carrier.SetFreq(freq);
                carriers.Add(carrier);
            }
        }
    }
} 