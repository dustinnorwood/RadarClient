using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace RadarClient
{
    public struct MRM_SET_CONFIG_REQUEST
    {
        public ushort MessageID;
        public uint NodeID;
        public int ScanStartPS, ScanStopPS;
        public ushort ScanResolutions;
        public ushort BaseIntegrationIndex;
        public ushort Seg1NumSamples, Seg2NumSamples, Seg3NumSamples, Seg4NumSamples;
        public byte Seg1IntegMult, Seg2IntegMult, Seg3IntegMult, Seg4IntegMult;
        public byte AntennaMode, TransmitGain, CodeChannel, PersistFlag;
    }

    public struct MRM_SET_CONFIG_CONFIRM
    {
        public ushort MessageID;
        public uint Status;
    }

    public struct MRM_GET_CONFIG_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_GET_CONFIG_CONFIRM
    {
        public ushort MessageID;
        public uint NodeID;
        public int ScanStartPS, ScanStopPS;
        public ushort ScanResolutions;
        public ushort BaseIntegrationIndex;
        public ushort Seg1NumSamples, Seg2NumSamples, Seg3NumSamples, Seg4NumSamples;
        public byte Seg1IntegMult, Seg2IntegMult, Seg3IntegMult, Seg4IntegMult;
        public byte AntennaMode, TransmitGain, CodeChannel, PersistFlag;
        public uint Timestamp, Status;
    }

    public struct MRM_CONTROL_REQUEST
    {
        public ushort MessageID;
        public uint ScanCount;
        public ushort Reserved;
        public uint ScanIntervalTime;
    }

    public struct MRM_CONTROL_CONFIRM
    {
        public ushort MessageID;
        public uint Status;
    }

    public struct MRM_SERVER_CONNECT_REQUEST
    {
        public ushort MessageID;
        public uint MrmIpAddress;
        public ushort MrmIpPort;
        public byte ConnectionType, Reserved;
    }

    public struct MRM_SERVER_CONNECT_CONFIRM
    {
        public ushort MessageID;
        public uint ConnectionStatus;
    }

    public struct MRM_SERVER_DISCONNECT_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_SERVER_DISCONNECT_CONFIRM
    {
        public ushort MessageID;
        public uint Status;
    }

    public struct MRM_SET_FILTER_CONFIG_REQUEST
    {
        public ushort MessageID, FilterMask;
        public byte MotionFilterIndex, Reserved;
    }

    public struct MRM_SET_FILTER_CONFIG_CONFIRM
    {
        public ushort MessageID;
        public uint Status;
    }

    public struct MRM_GET_FILTER_CONFIG_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_GET_FILTER_CONFIG_CONFIRM
    {
        public ushort MessageID, FilterMask;
        public byte MotionFilterIndex, Reserved;
        public uint Status;
    }

    public struct MRM_GET_STATUSINFO_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_GET_STATUSINFO_CONFIRM
    {
        public ushort MessageID;
        public byte MrmVersionMajor, MrmVersionMinor;
        public ushort MrmVersionBuild;
        public byte UwbKernelMajor, UwbKernelMinor;
        public ushort UwbKernelBuild;
        public byte FpgaFirmwareVersion, FpgaFirmwareYear, FpgaFirmwareMonth, FpgaFirmwareDay;
        public uint SerialNumber;
        public byte BoardRevision, PowerOnBitTestResult, TransmitterConfiguration;
        public int Temperature;
        public uint Status;
    }

    public struct MRM_REBOOT_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_REBOOT_CONFIRM
    {
        public ushort MessageID;
    }

    public struct MRM_SET_OPMODE_REQUEST
    {
        public ushort MessageID;
        public uint OperationalMode;
    }

    public struct MRM_SET_OPMODE_CONFIRM
    {
        public ushort MessageID;
        public uint OperationalMode, Status;
    }

    public struct MRM_SCAN_INFO
    {
        public ushort MessageID;
        public uint SourceID, Timestamp, Reserved1, Reserved2, Reserved3, Reserved4;
        public int ScanStartPS, ScanStopPS;
        public short ScanStepBins;
        public byte ScanType, Reserved5, AntennaID, OperationalMode;
        public ushort NumberOfSamplesInMessage;
        public uint NumberOfSamplesTotal;
        public ushort MessageIndex, NumberOfMessagesTotal;
        public List<int> ScanData;
    }

    public struct MRM_DETECTION
    {
        public ushort Index, Magnitude;
    }

    public struct MRM_DETECTION_LIST_INFO
    {
        public ushort MessageID, NumberOfDetections;
        public MRM_DETECTION[] Detections;
    }

    public struct MRM_SET_SLEEPMODE_REQUEST
    {
        public ushort MessageID;
        public uint SleepMode;
    }

    public struct MRM_SET_SLEEPMODE_CONFIRM
    {
        public ushort MessageID;
        public uint Status;
    }

    public struct MRM_GET_SLEEPMODE_REQUEST
    {
        public ushort MessageID;
    }

    public struct MRM_GET_SLEEPMODE_CONFIRM
    {
        public ushort MessageID;
        public uint SleepMode, Status;
    }

    public struct MRM_READY_INFO
    {
        public ushort MessageID;
    }

    public static class RadarRequest
    {
        public static bool CommandOut = false;
        public static bool SendMRM_SET_CONFIG_REQUEST(Radar p, ref MRM_SET_CONFIG_REQUEST t)
        {
            try
            {
                byte[] b = new byte[39];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x23;
                b[4] = 0x10; b[5] = 0x01;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)t.MessageID;
                b[8] = (byte)(t.NodeID >> 24);
                b[9] = (byte)(t.NodeID >> 16);
                b[10] = (byte)(t.NodeID >> 8);
                b[11] = (byte)(t.NodeID);
                b[12] = (byte)(t.ScanStartPS >> 24);
                b[13] = (byte)(t.ScanStartPS >> 16);
                b[14] = (byte)(t.ScanStartPS >> 8);
                b[15] = (byte)(t.ScanStartPS);
                b[16] = (byte)(t.ScanStopPS >> 24);
                b[17] = (byte)(t.ScanStopPS >> 16);
                b[18] = (byte)(t.ScanStopPS >> 8);
                b[19] = (byte)(t.ScanStopPS);
                b[20] = (byte)(t.ScanResolutions >> 8);
                b[21] = (byte)t.ScanResolutions;
                b[22] = (byte)(t.BaseIntegrationIndex >> 8);
                b[23] = (byte)t.BaseIntegrationIndex;
                b[24] = (byte)(t.Seg1NumSamples >> 8);
                b[25] = (byte)t.Seg1NumSamples;
                b[26] = (byte)(t.Seg2NumSamples >> 8);
                b[27] = (byte)t.Seg2NumSamples;
                b[28] = (byte)(t.Seg3NumSamples >> 8);
                b[29] = (byte)t.Seg3NumSamples;
                b[30] = (byte)(t.Seg4NumSamples >> 8);
                b[31] = (byte)t.Seg4NumSamples;
                b[32] = t.Seg1IntegMult;
                b[33] = t.Seg2IntegMult;
                b[33] = t.Seg3IntegMult;
                b[34] = t.Seg4IntegMult;
                b[35] = t.AntennaMode;
                b[36] = t.TransmitGain;
                b[37] = t.CodeChannel;
                b[38] = t.PersistFlag;
                p.Write(b, 0, 39);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_GET_CONFIG_REQUEST(Radar p, ref MRM_GET_CONFIG_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0x10; b[5] = 0x02;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)t.MessageID;
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_CONTROL_REQUEST(Radar p, ref MRM_CONTROL_REQUEST t)
        {
            try
            {
                byte[] b = new byte[16];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x0C;
                b[4] = 0x10; b[5] = 0x03;
                b[6] = (byte)(t.ScanCount >> 24);
                b[7] = (byte)(t.ScanCount >> 16);
                b[8] = (byte)(t.ScanCount >> 8);
                b[9] = (byte)(t.ScanCount);
                b[10] = (byte)(t.Reserved >> 8);
                b[11] = (byte)(t.Reserved);
                b[12] = (byte)(t.ScanIntervalTime >> 24);
                b[13] = (byte)(t.ScanIntervalTime >> 16);
                b[14] = (byte)(t.ScanIntervalTime >> 8);
                b[15] = (byte)(t.ScanIntervalTime);
                p.Write(b, 0, 16);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ReceiveMRM_CONTROL_CONFIRM(string b, out MRM_CONTROL_CONFIRM t)
        {
            
            t = new MRM_CONTROL_CONFIRM();
            try
            {
                byte[] s = Encoding.Default.GetBytes(b);
                t.MessageID = ToUshort(s, 0);
                t.Status = ToUint(s, 2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_SERVER_CONNECT_REQUEST(Radar p, ref MRM_SERVER_CONNECT_REQUEST t)
        {
            try
            {
                byte[] b = new byte[16];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x0C;
                b[4] = 0x10; b[5] = 0x04;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                b[8] = (byte)(t.MrmIpAddress >> 24);
                b[9] = (byte)(t.MrmIpAddress >> 16);
                b[10] = (byte)(t.MrmIpAddress >> 8);
                b[11] = (byte)(t.MrmIpAddress);
                b[12] = (byte)(t.MrmIpPort >> 8);
                b[13] = (byte)(t.MrmIpPort);
                b[14] = t.ConnectionType;
                b[15] = t.Reserved;
                p.Write(b, 0, 16);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_SERVER_DISCONNECT_REQUEST(Radar p, ref MRM_SERVER_DISCONNECT_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0x10; b[5] = 0x05;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_SET_FILTER_CONFIG_REQUEST(Radar p, ref MRM_SET_FILTER_CONFIG_REQUEST t)
        {
            try
            {
                byte[] b = new byte[12];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x08;
                b[4] = 0x10; b[5] = 0x06;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                b[8] = (byte)(t.FilterMask >> 8);
                b[9] = (byte)(t.FilterMask);
                b[10] = t.MotionFilterIndex;
                b[11] = t.Reserved;
                p.Write(b, 0, 12);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_GET_FILTER_CONFIG_REQUEST(Radar p, ref MRM_GET_FILTER_CONFIG_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0x10; b[5] = 0x07;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_GET_STATUSINFO_REQUEST(Radar p, ref MRM_GET_STATUSINFO_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0xF0; b[5] = 0x01;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_REBOOT_REQUEST(Radar p, ref MRM_REBOOT_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0xF0; b[5] = 0x02;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_SET_OPMODE_REQUEST(Radar p, ref MRM_SET_OPMODE_REQUEST t)
        {
            try
            {
                byte[] b = new byte[12];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x08;
                b[4] = 0xF0; b[5] = 0x03;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                b[8] = (byte)(t.OperationalMode >> 24);
                b[9] = (byte)(t.OperationalMode >> 16);
                b[10] = (byte)(t.OperationalMode >> 8);
                b[11] = (byte)(t.OperationalMode);
                p.Write(b, 0, 12);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ReceiveMRM_SCAN_INFO(string b, out MRM_SCAN_INFO t)
        {
            try
            {
                byte[] s = Encoding.Default.GetBytes(b);
                t.MessageID = ToUshort(s, 0);
                t.SourceID = ToUint(s, 2);
                t.Timestamp = ToUint(s, 6);
                t.Reserved1 = ToUint(s, 10);
                t.Reserved2 = ToUint(s, 14);
                t.Reserved3 = ToUint(s, 18);
                t.Reserved4 = ToUint(s, 22);
                t.ScanStartPS = ToInt(s, 26);
                t.ScanStopPS = ToInt(s, 30);
                t.ScanStepBins = ToShort(s, 34);
                t.ScanType = (byte)s[36];
                t.Reserved5 = (byte)s[37];
                t.AntennaID = (byte)s[38];
                t.OperationalMode = (byte)s[39];
                t.NumberOfSamplesInMessage = ToUshort(s, 40);
                t.NumberOfSamplesTotal = ToUint(s, 42);
                t.MessageIndex = ToUshort(s, 46);
                t.NumberOfMessagesTotal = ToUshort(s, 48);
                t.ScanData = new List<int>();
                for (int a = 50; a <= s.Length - 4; a += 4)
                    t.ScanData.Add(ToInt(s, a));
                return true;
            }
            catch
            {
                t.MessageID = 0;
                t.SourceID = 0;
                t.Timestamp = 0;
                t.Reserved1 = 0;
                t.Reserved2 = 0;
                t.Reserved3 = 0;
                t.Reserved4 = 0;
                t.ScanStartPS = 0;
                t.ScanStopPS = 0;
                t.ScanStepBins = 0;
                t.ScanType = 0;
                t.Reserved5 = 0;
                t.AntennaID = 0;
                t.OperationalMode = 0;
                t.NumberOfSamplesInMessage = 0;
                t.NumberOfSamplesTotal = 0;
                t.MessageIndex = 0;
                t.NumberOfMessagesTotal = 0;
                t.ScanData = new List<int>();
                return false;
            }
        }

        public static bool SendMRM_SCAN_INFO(out byte[] b, MRM_SCAN_INFO t)
        {
            try
            {
                b = new byte[56 + 4 * t.ScanData.Count];
                FromUshort(0xA5A5, b, 0);
                FromUshort((ushort)(b.Length - 4), b, 2);
                FromUshort(0xF201, b, 4);
                FromUshort(t.MessageID,b,6);
                FromUint(t.SourceID,b,8);
                FromUint(t.Timestamp,b,12);
                FromUint(t.Reserved1,b,16);
                FromUint(t.Reserved2,b,20);
                FromUint(t.Reserved3,b,24);
                FromUint(t.Reserved4,b,28);
                FromInt(t.ScanStartPS,b,32);
                FromInt(t.ScanStopPS,b,36);
                FromShort(t.ScanStepBins,b,40);
                b[42] = t.ScanType;
                b[43] = t.Reserved5;
                b[44] = t.AntennaID;
                b[45] = t.OperationalMode;
                FromUshort(t.NumberOfSamplesInMessage,b,46);
                FromUint(t.NumberOfSamplesTotal,b,48);
                FromUshort(t.MessageIndex,b,52);
                FromUshort(t.NumberOfMessagesTotal,b,54);
                for(int k = 56; k < b.Length - 3; k += 4)
                    FromInt(t.ScanData[(k - 56)/4],b,k);
                return true;
            }
            catch
            {
                b = null;
                return false;
            }
        }

        public static bool SendMRM_SET_SLEEPMODE_REQUEST(Radar p, ref MRM_SET_SLEEPMODE_REQUEST t)
        {
            try
            {
                byte[] b = new byte[12];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x08;
                b[4] = 0xF0; b[5] = 0x05;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                b[8] = (byte)(t.SleepMode >> 24);
                b[9] = (byte)(t.SleepMode >> 16);
                b[10] = (byte)(t.SleepMode >> 8);
                b[11] = (byte)(t.SleepMode);
                p.Write(b, 0, 12);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMRM_GET_SLEEPMODE_REQUEST(Radar p, ref MRM_GET_SLEEPMODE_REQUEST t)
        {
            try
            {
                byte[] b = new byte[8];
                b[0] = b[1] = 0xA5;
                b[2] = 0x00; b[3] = 0x04;
                b[4] = 0xF0; b[5] = 0x05;
                b[6] = (byte)(t.MessageID >> 8);
                b[7] = (byte)(t.MessageID);
                p.Write(b, 0, 8);
                CommandOut = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static uint ToUint(byte[] s, int i)
        {
            try
            {
                return (uint)(((uint)s[i] << 24) + ((uint)s[i + 1] << 16) + ((uint)s[i + 2] << 8) + (uint)s[i + 3]);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
        }

        public static void FromUint(uint a, byte[] b, int offset)
        {
            b[offset] = (byte)((a >> 24) & 0xff);
            b[offset+1] = (byte)((a >> 16) & 0xff);
            b[offset+2] = (byte)((a >> 8) & 0xff);
            b[offset+3] = (byte)(a & 0xff);
        }

        public static int ToInt(byte[] s, int i)
        {
            try
            {
                int k = (int)(((int)s[i] << 24) + ((int)s[i + 1] << 16) + ((int)s[i + 2] << 8) + (int)s[i + 3]);
                return k;
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
        }

        public static void FromInt(int a, byte[] b, int offset)
        {
            b[offset] = (byte)((a >> 24) & 0xff);
            b[offset + 1] = (byte)((a >> 16) & 0xff);
            b[offset + 2] = (byte)((a >> 8) & 0xff);
            b[offset + 3] = (byte)(a & 0xff);
        }

        public static ushort ToUshort(byte[] s, int i)
        {
            try
            {
                return (ushort)(((ushort)s[i] << 8) + (ushort)s[i + 1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
        }

        public static void FromUshort(ushort a, byte[] b, int offset)
        {
            b[offset] = (byte)((a >> 8) & 0xff);
            b[offset + 1] = (byte)(a & 0xff);
        }

        public static short ToShort(byte[] s, int i)
        {
            try
            {
                return (short)(((short)s[i] << 8) + (short)s[i + 1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
        }

        public static void FromShort(short a, byte[] b, int offset)
        {
            b[offset] = (byte)((a >> 8) & 0xff);
            b[offset + 1] = (byte)(a & 0xff);
        }
    }
}