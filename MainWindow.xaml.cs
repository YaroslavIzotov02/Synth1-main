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
        bool chck1 = false;
        bool chck2 = false;
        double mf = 1;
        private IWaveProvider provider;
        WaveFormat format;
        static int keysCount = 0;
        List<string> presets = new List<string>();


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
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (Directory.Exists(path))
            {
                presets = Directory.GetFiles(path).ToList();
                List<string> temp = new List<string>();
                foreach (string preset in presets)
                {
                    temp.Add(preset.Remove(0, preset.LastIndexOf("\\") + 1));
                }
                Presets.ItemsSource = temp;
            }

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SaveDialog sd = new SaveDialog();
            if (sd.ShowDialog() == true)
            {
                if (!File.Exists(path + "\\" + sd.PrName + ".txt"))
                {
                    using (File.Create(path + "\\" + sd.PrName + ".txt"));
                }
                    if (!chck1 && !chck2)
                    {
                        string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value) + '\n';
                        string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value) + '\n';
                        File.AppendAllText(path + "\\" + sd.PrName + ".txt", os1);
                        File.AppendAllText(path + "\\" + sd.PrName + ".txt", os2);
                    }
                    else
                    {
                        if (chck1)
                        {
                            string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value, Ratio.Value) + '\n';
                            string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value) + '\n';
                            File.AppendAllText(path + "\\" + sd.PrName + ".txt", os1);
                            File.AppendAllText(path + "\\" + sd.PrName + ".txt", os2);
                        }
                        else
                        {
                            string os1 = SetStr(Osc1Wave.Text.ToString(), a1.Value, d1.Value, s1.Value, r1.Value, Osc1V.Value) + '\n';
                            string os2 = SetStr(Osc2Wave.Text.ToString(), a2.Value, d2.Value, s2.Value, r2.Value, Osc2V.Value, Ratio.Value) + '\n';
                            File.AppendAllText(path + "\\" + sd.PrName + ".txt", os1);
                            File.AppendAllText(path + "\\" + sd.PrName + ".txt", os2);
                        }
                    }
            }
            
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
                if (!(bool)chck1 && !(bool)chck2)
                {
                    Generator g1 = new Generator();
                    g1.SetAmplitude(8000);
                    g1.SetWave(wt);
                    Generator g2 = new Generator();
                    g2.SetAmplitude(8000);
                    g2.SetWave(wt2);

                    s.AddCarrier(g1);
                    s.AddCarrier(g2);
                }
                else
                {
                    if ((bool)chck2)
                    {
                        Carrier c1 = new Carrier();
                        c1.SetAmplitude(8000);
                        c1.SetWave(wt);

                        Modulator m1 = new Modulator();
                        m1.SetWave(wt2);
                        m1.SetFreq(freq);
                        m1.SetRatio(ref mf);
                        c1.SetModulator(m1);

                        s.AddCarrier(c1);
                    }

                    else
                    {
                        Carrier c1 = new Carrier();
                        c1.SetAmplitude(8000);
                        c1.SetWave(wt2);

                        Modulator m1 = new Modulator();
                        m1.SetWave(wt);
                        m1.SetFreq(freq);
                        m1.SetRatio(ref mf);
                        c1.SetModulator(m1);
                        s.AddCarrier(c1);
                       
                    }
                }
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

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            chck1 = true;
            checkBox2.IsEnabled = false;
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            chck1 = false;
            checkBox2.IsEnabled = true;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            chck2 = true;
            checkBox1.IsEnabled = false;
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            chck2 = false;
            checkBox1.IsEnabled = true;
        }

        /*private (Generator, Generator) CreateScheme()
        {
            if (!(bool)chck1 && !(bool)chck2)
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
                if ((bool)chck2)
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
        }*/

        private string SetStr(string w, double a, double d, double s, double r, double v)
        {
            return w + " " + false + " " + a + " " + d + " " + s + " " + r + " " + v;
        }

        private string SetStr(string w, double a, double d, double s, double r, double v, double rt)
        {
            return w + " " + true + " " + a + " " + d + " " + s + " " + r + " " + v + " " + rt;
        }

        private void Presets_DropDownClosed(object sender, EventArgs e)
        {
            string path = $"C:\\Users\\{Environment.UserName}\\Documents\\TestSynth\\Presets";
            if (Directory.Exists(path))
            {
               if (File.Exists(path + "\\" + Presets.Text.ToString()))
               {
                    string[] temp = File.ReadAllLines(path + "\\" + Presets.Text.ToString());
                    string[] st1 = temp[0].Split(" ");
                    string[] st2 = temp[1].Split(" ");

                    Osc1Wave.Text = st1[0];
                    Osc2Wave.Text = st2[0];
                    Enum.TryParse(st1[0], out wt);
                    Enum.TryParse(st2[0], out wt2);
                    checkBox1.IsChecked = chck1 = Convert.ToBoolean(st1[1]);
                    checkBox2.IsChecked = chck2 = Convert.ToBoolean(st2[1]);
                    a1.Value = Convert.ToDouble(st1[2]);
                    a2.Value = Convert.ToDouble(st2[2]);
                    d1.Value = Convert.ToDouble(st1[3]);
                    d2.Value = Convert.ToDouble(st2[3]);
                    s1.Value = Convert.ToDouble(st1[4]);
                    s2.Value = Convert.ToDouble(st2[4]);
                    r1.Value = Convert.ToDouble(st1[5]);
                    r2.Value = Convert.ToDouble(st2[5]);
                    Osc1V.Value = Convert.ToDouble(st1[6]);
                    Osc2V.Value = Convert.ToDouble(st2[6]);



                    if (Convert.ToBoolean(st1[1]))
                    {
                        mf = Ratio.Value = Convert.ToDouble(st1[7]);
                    }

                    if (Convert.ToBoolean(st2[1]))
                    {
                        mf = Ratio.Value = Convert.ToDouble(st2[7]);
                    }
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

        private void Ratio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mf = Ratio.Value;
        }
    }
}
