using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RadarClient
{
    public delegate void RadarScanReceivedEvent(object sender, MRM_SCAN_INFO e);
    public delegate void RadarControlReceivedEvent(object sender, string e);

    public class Radar
    {
        public event RadarScanReceivedEvent ScanReceived;
        public event RadarControlReceivedEvent CommandReceived;
        private System.IO.Ports.SerialPort _port;
        private System.Threading.Thread readThread;
        private volatile bool KeepReading = false;

        public bool IsConnected { get { return (_port != null && _port.IsOpen); } }
        public string PortName
        {
            get { if (_port != null && _port.IsOpen) return _port.PortName; else return "Not Connected"; }
        }

        public List<MRM_SCAN_INFO> Scans;

        public Radar()
        {
            Scans = new List<MRM_SCAN_INFO>();
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool Connect(string name)
        {
            try
            {
                if (Disconnect())
                {
                    _port = new System.IO.Ports.SerialPort(name, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    _port.Encoding = Encoding.Default;
                    _port.Open();
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    KeepReading = true;
                    readThread = new System.Threading.Thread(new System.Threading.ThreadStart(readPort));
                    readThread.Start();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Disconnect()
        {
            try
            {
                if (_port != null)
                {
                    if (_port.IsOpen)
                    {
                        _port.DiscardInBuffer();
                        _port.DiscardOutBuffer();
                        KeepReading = false;
                        _port.Close();
                    }
                    _port.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void readPort()
        {
            try
            {
                MRM_SCAN_INFO v = new MRM_SCAN_INFO();
                v.ScanData = new List<int>();
                while (KeepReading)
                {
                    while (KeepReading && _port.BytesToRead < 4) ;
                    byte[] s = new byte[4];
                    int k = _port.Read(s, 0, 4);
                    if (k == 4)
                    {
                        int numBytes = 0;
                        if (s[0] == 0xA5 && s[1] == 0xA5)
                        {
                            numBytes = 256 * s[2] + s[3];
                            while (KeepReading && _port.BytesToRead < numBytes) ;
                            byte[] r = new byte[numBytes];
                            k = _port.Read(r, 0, numBytes);

                            ushort code = (ushort)((r[0] << 8) + r[1]);
                            if (code == 0xF201)
                            {
                                MRM_SCAN_INFO t;
                                if (RadarRequest.ReceiveMRM_SCAN_INFO(Encoding.Default.GetString(r, 2, r.Length - 2), out t))
                                {
                                    System.Diagnostics.Debug.WriteLine(t.MessageID + " " + t.MessageIndex);
                                    foreach (int i in t.ScanData)
                                        v.ScanData.Add(i);
                                    if (t.MessageIndex == t.NumberOfMessagesTotal - 1)
                                    {
                                        v.MessageID = t.MessageID;
                                        v.SourceID = t.SourceID;
                                        v.Timestamp = t.Timestamp;
                                        v.Reserved1 = t.Reserved1;
                                        v.Reserved2 = t.Reserved2;
                                        v.Reserved3 = t.Reserved3;
                                        v.Reserved4 = t.Reserved4;
                                        v.ScanStartPS = t.ScanStartPS;
                                        v.ScanStopPS = t.ScanStopPS;
                                        v.ScanStepBins = t.ScanStepBins;
                                        v.ScanType = t.ScanType;
                                        v.Reserved5 = t.Reserved5;
                                        v.AntennaID = t.AntennaID;
                                        v.OperationalMode = t.OperationalMode;
                                        v.NumberOfSamplesInMessage = t.NumberOfSamplesInMessage;
                                        v.NumberOfSamplesTotal = t.NumberOfSamplesTotal;
                                        v.MessageIndex = t.MessageIndex;
                                        v.NumberOfMessagesTotal = t.NumberOfMessagesTotal;
                                        //string path = "C:\\Users\\Dustin\\Radar Data\\Radar Scans\\test.csv";
                                        //if (!System.IO.File.Exists(path)) System.IO.File.Create(path);
                                        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                                        //{
                                        //    sw.Write(v.Timestamp + ", MrmFullScanInfo, " + v.MessageID + ", " + v.SourceID + ", " + v.Timestamp + ", ");
                                        //    sw.Write(v.Reserved1 + ", " + v.Reserved2 + ", " + v.Reserved3 + ", " + v.Reserved4 + ", " + v.ScanStartPS + ", " + v.ScanStopPS + ", " + v.ScanStepBins + ", ");
                                        //    sw.Write("1, " + v.AntennaID + ", " + v.Reserved5 + ", " + v.NumberOfSamplesTotal);
                                        //    foreach (int a in v.ScanData)
                                        //        sw.Write(", " + a);
                                        //    sw.Write('\n');
                                        //    sw.Close();

                                        //}
                                        if (ScanReceived != null)
                                        {
                                            ScanReceived(this, v);
                                        }
                                        v = new MRM_SCAN_INFO();
                                        v.ScanData = new List<int>();
                                    }
                                }
                            }
                            else
                            {
                                if (this.CommandReceived != null)
                                    this.CommandReceived(this, Encoding.Default.GetString(r));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void Write(byte[] data, int offset, int count)
        {
            if (_port.IsOpen) _port.Write(data, offset, count);
            else throw new System.InvalidOperationException();
        }

    }
}
