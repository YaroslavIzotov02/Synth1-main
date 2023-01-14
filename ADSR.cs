using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synth_1
{
    class ADSR
    {
        private double attack;
        private double decay;
        private double sustain;
        private double release;

        private double step = 0;

        enum State {
        attack,
        decay,
        sustain,
        release
        }

        State state;

        public ADSR(double a, double d, double s, double r)
        {
            attack = a;
            decay = d;
            sustain = s;
            release = r;
            state = State.attack;
        }

        public double Attack { get => attack; set { attack = value; }}
        public double Decay { get => decay; set { decay = value; }}
        public double Sustain { get => sustain; set { sustain = value; }}
        public double Release { get => release; set { release = value; }}

        /*public double Enveloping()
        {
            switch (state)
            {
                case State.attack: return ();
                case State.decay: return ();
                case State.sustain: return ();
                case State.release: return ();
            }
        }*/
    }
}
