using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public enum WaveType
    {
        Sine,
        Square,
        Saw,
        Triangle,
        Noise
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate double Out();


    /// <summary>
    /// 
    /// </summary>
    public class Generator
    {

        #region Aggregations


        #endregion

        #region Compositions


        #endregion

        #region Attributes
        /// <summary>
        /// 
        /// </summary>
        private protected Dictionary<WaveType, Out> dic;

        /// <summary>
        /// 
        /// </summary>
        WaveType waveType;

        /// <summary>
        /// 
        /// </summary>
        private protected double Frequency;

        /// <summary>
        /// 
        /// </summary>
        private protected short Amplitude = 7000;

        /// <summary>
        /// 
        /// </summary>
        private protected short Velocity;

        private protected double phaseAngle = 0;

        #endregion


        #region Public methods

        public Generator()
        {
            dic = new Dictionary<WaveType, Out>()
            {
                {WaveType.Sine, Sine},
                {WaveType.Saw, Saw},
                {WaveType.Square, Square},
                {WaveType.Triangle, Triangle},
                {WaveType.Noise, Noise}
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WaveType"></param>
        /// <returns></returns>
        public void SetWave(WaveType waveTypeIn)
        {
            waveType = waveTypeIn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Freq"></param>
        /// <returns></returns>
        public void SetFreq(double Freq)
        {
            Frequency = Freq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Amp"></param>
        /// <returns></returns>
        public void SetAmplitude(short Amp)
        {
            Amplitude = Amp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>double</returns>
        public double GetOut()
        {
            return dic[waveType]();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Velocity"></param>
        /// <returns></returns>
        public void SetVelocity(short Vel)
        {
            Velocity = Vel;
        }

        #endregion


        #region Protected methods

        #endregion


        #region Private methods
        private protected double Sine()
        {
            if (phaseAngle > 2 * Math.PI)
                phaseAngle -= 2 * Math.PI;

            phaseAngle += 2 * Math.PI * Frequency / 44100;

            return (Amplitude * Math.Sin(phaseAngle));
        }

        private protected double Saw()
        {
            if (phaseAngle > 2 * Math.PI)
                phaseAngle -= 2 * Math.PI;

            phaseAngle += 2 * Math.PI * Frequency / 44100;

            return (2 * Amplitude * Math.Asin(Math.Sin(phaseAngle)) / Math.PI);
        }

        private protected double Square()
        {
            if (phaseAngle > 2 * Math.PI)
                phaseAngle -= 2 * Math.PI;

            phaseAngle += 2 * Math.PI * Frequency / 44100;

            return (Amplitude * Math.Sign(Math.Sin(phaseAngle)));
        }

        private protected double Triangle()
        {
            if (phaseAngle > 2 * Math.PI)
                phaseAngle -= 2 * Math.PI;

            phaseAngle += 2 * Math.PI * Frequency / 44100;

            return (2 * Amplitude * (Math.PI / (2 - Math.Atan(Math.Tan(phaseAngle)))) / Math.PI);
        }

        private protected double Noise()
        {
            if (phaseAngle > 2 * Math.PI)
                phaseAngle -= 2 * Math.PI;

            phaseAngle += 2 * Math.PI * Frequency / 44100;

            return (Amplitude * (short)new Random().Next());
        }
        #endregion
    }
}
