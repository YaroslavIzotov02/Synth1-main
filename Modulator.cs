using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public class Modulator : Generator
    {

        #region Aggregations


        #endregion

        #region Compositions


        #endregion

        #region Attributes

        

        /// <summary>
        /// 
        /// </summary>
        private double Ratio;

        /// <summary>
        /// 
        /// </summary>
        private Modulator modulator;



        #endregion


        #region Public methods

        public Modulator() : base ()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns>double</returns>
        public override double GetOut()
        {
            return Ratio * (dic[waveType]() / Amplitude);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ratio"></param>
        /// <returns></returns>
        public void SetRatio(ref double ratio)
        {
            Ratio = ratio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modulator"></param>
        /// <returns></returns>
        public void SetModulator(Modulator mod)
        {
           modulator = mod;
        }
        #endregion


        #region Protected methods

        #endregion


        #region Private methods

        #endregion


    }
}
