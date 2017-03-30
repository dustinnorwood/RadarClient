using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace RadarClient
{
    public delegate void UIUpdater();
    public partial class Form1 : Form
    {
        private Radar _radar;
        private AsynchronousClient _async;
        private string _log, _prefix = "RetLog_";
        private System.IO.StreamWriter wr;
        private int scanIndex = 0;
        private System.Threading.Timer scanner;
        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string s in ports) comboBox1.Items.Add(s);
            if (ports.Length > 0)
            {
                comboBox1.Text = ports[0];
                comboBox1.SelectedItem = ports[0];
            }
            _radar = new Radar();
            _radar.ScanReceived += radar_ScanReceived;
            _radar.CommandReceived += radar_ControlReceived;
            _async = new AsynchronousClient();
            _async.DataReceived += async_DataReceived;
            _async.StartLogging += async_StartLogging;
            _async.StopLogging += async_StopLogging;
            _log = Properties.Settings.Default.SavedDirectory;
            if(_log == null || _log == "")
            {
                _log = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            folderBrowserDialog1.SelectedPath = _log;
            _prefix = Properties.Settings.Default.SavedPrefix;
            toolStripTextBox1.Text = _prefix;
            textBox1.Text = Properties.Settings.Default.SavedIP;
            textBox2.Text = Properties.Settings.Default.SavedPort.ToString();
            textBox3.Text = Properties.Settings.Default.SavedName;
            textBox4.Text = Properties.Settings.Default.X.ToString();
            textBox5.Text = Properties.Settings.Default.Y.ToString();

            StreamReader sr = new StreamReader(@"C:\Users\Dustin\Radar Data\Dustin\Enoch Circle.csv");
            Utility.ReadDataFromFile(sr, _radar);
            sr.Dispose();

            scanner = new System.Threading.Timer(new TimerCallback(timer1_Tick), null,Timeout.Infinite, Timeout.Infinite);
        }


        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string p in ports)
                comboBox1.Items.Add(p);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RadarFunc();
        }

        private void RadarFunc()
        {
            if (InvokeRequired) Invoke(new UIUpdater(RadarFunc));
            else
            {
                if (_radar.IsConnected)
                {
                    if (_radar.Disconnect())
                        button2.Text = "Connect";
                }
                else if (comboBox1.SelectedItem != null)
                {
                    if(_radar.Connect((string)comboBox1.SelectedItem))
                        button2.Text = "Disconnect";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpFunc();
        }

        private void TcpFunc()
        {
            if (InvokeRequired)
                this.Invoke(new UIUpdater(TcpFunc));
            else
            {
            TryConnect:
                if (button1.Text == "Connect")
                {
                    button1.Text = "Connecting";
                    IPAddress address;
                    int port;
                    if (IPAddress.TryParse(textBox1.Text, out address) && int.TryParse(textBox2.Text, out port))
                    {
                        if (_async.StartClient(address, port, textBox3.Text, textBox4.Text, textBox5.Text))
                            button1.Text = "Disconnect";
                        else
                        {
                            button1.Text = "Connect";
                            if (MessageBox.Show("Could not connect to server", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Retry)
                                goto TryConnect;
                            
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid format for IP Address or Port Number", "Invalid Data Field", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       button1.Text = "Connect";
                    }
                }
                else if(button1.Text == "Disconnect")
                {
                    _async.Send(Encoding.Default.GetBytes("<STOP>"));
                     button1.Text = "Connect";
                }
            }
        }

        private void EnableButton()
        {
            button1.Text = "Connect";
        }

        private void DisableButton()
        {
            button1.Text = "Disconnect";
        }

        private void radar_ScanReceived(object sender, MRM_SCAN_INFO ts)
        {
            byte[] b;
            if (RadarRequest.SendMRM_SCAN_INFO(out b, ts))
            {
                _async.Send(b);
            }
            //string s = "Scan " + m.MessageID.ToString() + " of length " + m.ScanData.Count.ToString() + " was received";
            //_async.Send(Encoding.Default.GetBytes(s));
            try
            {
                if (wr != null)
                {
                    wr.Write(System.DateTime.Now.Ticks + ", MrmFullScanInfo, " + ts.MessageID + ", " + ts.SourceID + ", " + ts.Timestamp + ", ");
                    wr.Write(ts.Reserved1 + ", " + ts.Reserved2 + ", " + ts.Reserved3 + ", " + ts.Reserved4 + ", " + ts.ScanStartPS + ", " + ts.ScanStopPS + ", " + ts.ScanStepBins + ", ");
                    wr.Write("1, " + ts.AntennaID + ", " + ts.Reserved5 + ", " + ts.NumberOfSamplesTotal);
                    foreach (int az in ts.ScanData)
                        wr.Write(", " + az);
                    wr.Write('\n');
                }
            }
            catch(ObjectDisposedException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
  
        }

        private void radar_ControlReceived(object sender, string s)
        {
            byte[] prefix = new byte[4];
            prefix[0] = prefix[1] = 0xA5;
            RadarRequest.FromUshort((ushort)s.Length,prefix,2);
            string str = Encoding.Default.GetString(prefix) + s;
            _async.Send(Encoding.Default.GetBytes(str));
        }

        private void async_DataReceived(object sender, SocketDataReceivedArgs e)
        {
            byte[] data = Encoding.Default.GetBytes(e.Data);
#if RADAR
            if(!_radar.IsConnected)
            {
                _async.Send(Encoding.Default.GetBytes("Command Failed: Radar Disconnected"));
                return;
            }
            else if ((data[0] == 0xA5) && (data[1] == 0xA5)) //Radar Command. Send the data directly to the radar
            {
                _radar.Write(data, 0, data.Length);
            }   
#endif
            if ((data[0] == 0xA5) && (data[1] == 0xA5))
                ProcCmd(data);
        }

        private void ProcCmd(byte[] b)
        {
            uint time, count;
            int rate;
            if ((b[4] == 0x10) && (b[5] == 0x03))
            {
                count = ((uint)b[6] << 24) + ((uint)b[7] << 16) + ((uint)b[8] << 8) + ((uint)b[9]);
                time = ((uint)b[12] << 24) + ((uint)b[13] << 16) + ((uint)b[14] << 8) + ((uint)b[15]);
                if (count == 0)
                    scanner.Change(Timeout.Infinite, Timeout.Infinite);
                else
                {
                    scanIndex = 0;
                    rate = (int)(time / 1000);
                    scanner.Change(rate, rate);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (button1.Text == "Disconnect")
            {
                _async.Send(Encoding.Default.GetBytes("<STOP>"));
            }
            _radar.Dispose();
            _async.Close();
            Properties.Settings.Default.SavedDirectory = _log;
            Properties.Settings.Default.SavedPrefix = _prefix;
            Properties.Settings.Default.SavedIP = textBox1.Text;
            Properties.Settings.Default.SavedName = textBox3.Text;
            int a;
            if(int.TryParse(textBox2.Text, out a))
            Properties.Settings.Default.SavedPort = a;
            if (int.TryParse(textBox4.Text, out a))
                Properties.Settings.Default.X = a;
            if (int.TryParse(textBox5.Text, out a))
                Properties.Settings.Default.Y = a;
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        private void startLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartLogging();
        }

        private void StartLogging()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UIUpdater(StartLogging));
            }
            else
            {
            TryLog:
                try
                {
                    if (wr != null)
                    {
                        wr.Close();
                        wr.Dispose();
                    }
                    int k = 0;
                    string fullFileName = "";
                    while (k < 1000)
                    {
                        if (!System.IO.File.Exists(_log + "\\" + _prefix + string.Format("{0}{1}{2}", k / 100, (k / 10) % 10, k % 10) + ".csv"))
                        {
                            fullFileName = _log + "\\" + _prefix + string.Format("{0}{1}{2}", k / 100, (k / 10) % 10, k % 10) + ".csv";
                            break;
                        }
                        k++;
                    }
                    wr = new StreamWriter(fullFileName);
                    this.Text = "Logging";
                }
                catch(UnauthorizedAccessException)
                {
                    _log = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    goto TryLog;
                }
            }
        }

        private void stopLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
                StopLogging();
        }

        private void StopLogging()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UIUpdater(StopLogging));
            }
            else
            {
                if (wr != null)
                {
                    wr.Close();
                }
                this.Text = "Radar Client";
            }
        }

        private void changeLogDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _log = folderBrowserDialog1.SelectedPath;
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            _prefix = toolStripTextBox1.Text;
        }

        private void async_StartLogging(object sender, SocketDataReceivedArgs e)
        {
            StartLogging();
        }

        private void async_StopLogging(object sender, SocketDataReceivedArgs e)
        {
            StopLogging();
        }

        private void timer1_Tick(object sender)
        {
            tT();
        }

        private void tT()
        {
            if (InvokeRequired) Invoke(new MethodInvoker(tT));
            else
            {
                if (this.Text == "Radar Client") this.Text = "Radar Client +";
                else this.Text = "Radar Client";
                byte[] data;
                if (RadarRequest.SendMRM_SCAN_INFO(out data, _radar.Scans[scanIndex]))
                    _async.Send(data);
                if ((++scanIndex) >= _radar.Scans.Count)
                    scanIndex = 0;
            }
        }

    }
}
