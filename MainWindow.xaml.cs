using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Synth_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int BUF_SIZE = 44100;
        ReadMidi midi;
        static Synthesator[] synths = new Synthesator[128];
        static WaveType wt;
        static WaveType wt2;
        private IWaveProvider provider;
        WaveFormat format;
        static int keysCount = 0;


        public MainWindow()
        {
            midi = new ReadMidi(this);
            InitializeComponent();
            format = new WaveFormat(44100, 16, 1);           
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer1_Tick;
            timer.Start();
            Devices.ItemsSource = midi.SelectDevice().Select(x => x.Value);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Devices_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (Devices.SelectedIndex >= 0)
                midi.InvokeMidiIn(Devices.SelectedIndex);
            else
                midi.InvokeMidiIn(0);
        }

        public void NoteOn(double freq, int index)
        {
            if (synths[index] == null)
            {
                Synthesator s = new Synthesator(freq);
                (Generator, Generator) temp = CreateScheme();
                s.AddCarrier(temp.Item1);
                s.AddCarrier(temp.Item2);
                synths[index] = s;
                keysCount++;
            }
        }

        public void NoteOff(double freq, int index)
        {
            if (synths[index] != null)
            {
                synths[index] = null;
                keysCount--;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (keysCount != 0)
            {
                byte[] buf = new byte[BUF_SIZE];
                for (int i = 0; i < BUF_SIZE; i++)
                {
                    short v = 0;
                    for (int j = 0; j < synths.Length; j++)
                    {
                        if (synths[j] != null)
                            v = Mix(v, synths[j].GetOut());
                    }

                    buf[i++] = (byte)(v & 0xFF);
                    buf[i] = (byte)(v >> 8);
                }
                MemoryStream buff_stream = new MemoryStream(buf);
                provider = new RawSourceWaveStream(buff_stream, format);
                WaveOutEvent wo = new WaveOutEvent();
                wo.Init(provider);
                if (provider != null)
                    wo.Play();
            }
        }

        public static short Mix(double v1, double v2)
        {
            double v = v1 + v2;
            // if (v1 != 0 && v2 != 0)
            //     v = v / 2;
            if (v > short.MaxValue)
                v = short.MaxValue;
            if (v < short.MinValue)
                v = short.MinValue;
            return (short)v;
        }

        private (Generator, Generator) CreateScheme()
        {
            if (!(bool)checkBox1.IsChecked && !(bool)checkBox2.IsChecked)
            {
                Generator g1 = new Generator();
                g1.SetAmplitude(8000);
                g1.SetWave(wt);
                Generator g2 = new Generator();
                g2.SetAmplitude(8000);
                g2.SetWave(wt2);

                return (g1, g2);
            }
            else
            {
                if ((bool)checkBox2.IsChecked)
                {
                    Carrier c1 = new Carrier();
                    c1.SetAmplitude(8000);
                    c1.SetWave(wt);

                    Modulator m1 = new Modulator();
                    m1.SetWave(wt2);
                    c1.SetModulator(m1);

                    return (c1, null);
                }

                else
                {
                    Carrier c1 = new Carrier();
                    c1.SetAmplitude(8000);
                    c1.SetWave(wt2);

                    Modulator m1 = new Modulator();
                    m1.SetWave(wt);
                    c1.SetModulator(m1);

                    return (c1, null);
                }
            }
        }

        private void Osc1Wave_DropDownClosed(object sender, EventArgs e)
        {
            
            if (Osc1Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);
            }
            else
            {
                Osc1Wave.SelectedIndex = 0;
                Enum.TryParse(Osc1Wave.Text.ToString(), out wt);

            }
        }

        private void Osc2Wave_DropDownClosed(object sender, EventArgs e)
        {
           
            if (Osc2Wave.SelectedIndex >= 0)
            {
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt2);
            }
            else
            {
                Osc2Wave.SelectedIndex = 0;
                Enum.TryParse(Osc2Wave.Text.ToString(), out wt2);
            }
        }
    }
}
