using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Synth_1
{
    /// <summary>
    /// 
    /// </summary>
    public class Carrier : Generator
    {

        #region Aggregations


        #endregion

        #region Compositions


        #endregion

        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        private Modulator modulator;



        #endregion


        #region Public methods

        public Carrier() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns>short int</returns>
        public override double GetOut()
        {
            this.Frequency = this.Frequency + modulator.GetOut();
            return dic[waveType]();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modulator"></param>
        /// <returns></returns>
        public void SetModulator(Modulator modulator)
        {
            this.modulator = modulator;
        }

        #endregion


        #region Protected methods

        #endregion


        #region Private methods

        #endregion


    }
}
