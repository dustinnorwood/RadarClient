using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarClient
{
    static class Utility
    {
        private enum DataType //All of the possible fields that can be read in a proper MRM CSV file
        {
            None, Timestamp, Mode, NodeId, ScanStartPs, ScanStopPs, ScanResolutionBins, BaseIntegrationIndex,
            Segment1NumSamples, Segment2NumSamples, Segment3NumSamples, Segment4NumSamples,
            Segment1AdditionalIntegration, Segment2AdditionalIntegration, Segment3AdditionalIntegration, Segment4AdditionalIntegration,
            AntennaMode, TransmitGain, CodeChannel,
            MessageId, SourceId, EmbeddedTimestamp, Reserved1, Reserved2, Reserved3, Reserved4, StartPs, StopPs, ScanStepBins, Filtering, AntennaId, Reserved5, NumSamples, ScanData
        }


        //Reads a CSV file bound to System.IO.StreamReader tr, and stores the data into Radar object n.
        public static void ReadDataFromFile(System.IO.StreamReader tr, Radar n)
        {
            DataType entry = DataType.Timestamp;
            MRM_SCAN_INFO tempScan;
            int[,] prev = new int[2, 1750];
            ushort msgId = 0;
            uint sourceId = 0;
            uint timestamp = 0;
            int scanStartPs = 0, scanStopPs = 0;
            short scanStepBins = 0;
            byte scanFiltering = 0, antennaId = 0;
            ushort numSamples = 0;
            int scanMag = 0, maxScanMag;
            string msg;
            bool initialized = false;
            n.Scans.Clear();
            while ((msg = tr.ReadLine()) != null)
            {
                string tempVal = "";
                int a = 0, b = 0;
                maxScanMag = 1;
                tempScan = new MRM_SCAN_INFO();
                tempScan.ScanData = new List<int>();
                while (a < msg.Length)
                {
                    b = a;
                    while (b < msg.Length && msg[b] != ',')
                        b++;
                    if (b >= msg.Length - 1 || (msg[b + 1] == ',' && msg[b - 1] == ','))
                        break;
                    tempVal = msg.Substring(a, b - a);
                    switch (entry)
                    {
                        case DataType.None:
                            break;
                        case DataType.Timestamp:
                            try
                            {
                                timestamp = (uint)double.Parse(tempVal);

                                entry = DataType.Mode;
                            }
                            catch (System.FormatException)
                            {

                                entry = DataType.None;
                            }
                            break;
                        case DataType.Mode:
                            if (tempVal == " MrmFullScanInfo")
                                entry = DataType.MessageId;
                            else entry = DataType.None;
                            //  else if (tempVal == " Config")
                            //      entry = DataType.NodeId;
                            break;
                        /* case DataType.NodeId:
                             if (tempVal == " NodeId")
                             {
                                 entry = DataType.None;
                                 break;
                             }
                             else entry++;
                         break;
                         case DataType.ScanStartPs:
                         break;
                         case DataType.ScanStopPs:
                         break;
                         case DataType.ScanResolutionBins:
                         break;
                         case DataType.BaseIntegrationIndex:
                         break;
                         case DataType.Segment1NumSamples:
                         break;
                         case DataType.Segment2NumSamples:
                         break;
                         case DataType.Segment3NumSamples:
                         break;
                         case DataType.Segment4NumSamples:
                         break;
                         case DataType.Segment1AdditionalIntegration:
                         break;
                         case DataType.Segment2AdditionalIntegration:
                         break;
                         case DataType.Segment3AdditionalIntegration:
                         break;
                         case DataType.Segment4AdditionalIntegration:
                         break;
                         case DataType.AntennaMode:
                         break;
                         case DataType.TransmitGain:
                         break;
                         case DataType.CodeChannel:
                         break;*/
                        case DataType.MessageId:
                            if (tempVal == " MessageId")
                            {
                                entry = DataType.None;
                                break;
                            }
                            else
                            {
                                try
                                {
                                    msgId = ushort.Parse(tempVal);
                                    entry = DataType.SourceId;
                                }
                                catch (System.FormatException)
                                {
                                    entry = DataType.None;
                                }


                            }
                            break;
                        case DataType.SourceId:
                            try
                            {
                                sourceId = uint.Parse(tempVal);
                                entry = DataType.EmbeddedTimestamp;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }


                            break;
                        case DataType.EmbeddedTimestamp:
                        case DataType.Reserved1:
                        case DataType.Reserved2:
                        case DataType.Reserved3:
                        case DataType.Reserved4:
                        case DataType.Reserved5:
                            entry++;
                            break;
                        case DataType.StartPs:
                            try
                            {
                                scanStartPs = int.Parse(tempVal);

                                entry = DataType.StopPs;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }
                            break;
                        case DataType.StopPs:
                            try
                            {
                                scanStopPs = int.Parse(tempVal);
                                entry = DataType.ScanStepBins;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }


                            break;
                        case DataType.ScanStepBins:
                            try
                            {
                                scanStepBins = short.Parse(tempVal);
                                entry = DataType.Filtering;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }
                            break;
                        case DataType.Filtering:
                            try
                            {
                                scanFiltering = byte.Parse(tempVal);
                                if (scanFiltering == 1)
                                    entry = DataType.AntennaId;
                                else
                                    entry = DataType.None;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }
                            break;
                        case DataType.AntennaId:
                            try
                            {
                                antennaId = byte.Parse(tempVal);

                                entry = DataType.Reserved5;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }

                            break;
                        case DataType.NumSamples:
                            try
                            {
                                numSamples = ushort.Parse(tempVal);
                                tempScan.NumberOfSamplesInMessage = numSamples;
                                tempScan.MessageID = msgId;
                                tempScan.SourceID = sourceId;
                                tempScan.Timestamp = timestamp;
                                tempScan.Reserved1 = tempScan.Reserved2 = tempScan.Reserved3 = tempScan.Reserved4 = 0;
                                tempScan.ScanStartPS = scanStartPs;
                                tempScan.ScanStopPS = scanStopPs;
                                tempScan.ScanStepBins = scanStepBins;
                                tempScan.ScanType = scanFiltering;
                                tempScan.Reserved5 = 0;
                                tempScan.AntennaID = antennaId;
                                tempScan.OperationalMode = 0;
                                tempScan.NumberOfSamplesTotal = 0;
                                tempScan.MessageIndex = 0;
                                tempScan.NumberOfMessagesTotal = 0;
                                initialized = true;
                                entry = DataType.ScanData;
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }

                            break;
                        case DataType.ScanData:
                            try
                            {
                                // int threshold = 1000000;
                                scanMag = int.Parse(tempVal);
                                //scanMag = (Math.Abs(tempScan.scanStartPs + index * PSPERBIN * tempScan.scanStepBins) < 30000 ? (scanMag > threshold ? threshold : scanMag) : scanMag);
#if MOTION
                                prev[1,index] = prev[0,index];
                                //prev[1,index] = prev[2,index];
                                //prev[2,index] = prev[3,index];
                                //prev[3, index] = prev[4, index];
                                //prev[4, index] = prev[5, index];
                                //prev[5, index] = prev[6, index];
                                //prev[6, index] = prev[7, index];
                                //prev[7, index] = prev[8, index];
                                prev[0,index] = scanMag;
                                scanMag = prev[0, index] - prev[1, index];
#endif
                                if (Math.Abs(scanMag) > Math.Abs(maxScanMag))
                                    maxScanMag = scanMag;
                                tempScan.ScanData.Add(scanMag);
                            }
                            catch (System.FormatException)
                            {
                                entry = DataType.None;
                            }
                            break;
                    }
                    a = b + 1;
                }
                entry = DataType.Timestamp;
                if (initialized)
                {
                    n.Scans.Add(tempScan);
                    initialized = false;
                }
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
            b[offset + 1] = (byte)((a >> 16) & 0xff);
            b[offset + 2] = (byte)((a >> 8) & 0xff);
            b[offset + 3] = (byte)(a & 0xff);
        }

        public static int ToInt(byte[] s, int i)
        {
            try
            {
                return (int)(((int)s[i] << 24) + ((int)s[i + 1] << 16) + ((int)s[i + 2] << 8) + (int)s[i + 3]);
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
