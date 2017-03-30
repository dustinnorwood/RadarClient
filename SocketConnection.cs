using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

// State object for receiving data from remote device.
namespace RadarClient
{
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class SocketDataReceivedArgs : EventArgs
    {
        public string Data;

        public SocketDataReceivedArgs(string s):base()
        {
            Data = s;
        }
    }

    public delegate void SocketEvent(object sender, SocketDataReceivedArgs e);

    public class AsynchronousClient
    {
        // The port number for the remote device.

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        private Socket _client;
        public event SocketEvent DataReceived, StartLogging, StopLogging;
        private bool DidConnect = false;
        //public event 
        public AsynchronousClient()
        {
       
        }

        private void _timeout(object data)
        {
            DidConnect = false;
            ((ManualResetEvent)data).Set();
        }

        public bool StartClient(IPAddress addr, int port, string name, string X, string Y)
        {
            // Connect to a remote device.
            try
            {
                DidConnect = true;
                connectDone.Reset();
                IPEndPoint remoteEP = new IPEndPoint(addr, port);

                // Create a TCP/IP socket.
                _client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                System.Threading.Timer t = new Timer(new TimerCallback(_timeout), connectDone, 1500, Timeout.Infinite);
                _client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), _client);
                connectDone.WaitOne();
                if (!DidConnect) return false;
                // Send test data to the remote device.
                Send(_client, Encoding.Default.GetBytes("RADAR," + name + "," + X + "," + Y));

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public bool Receive()
        {
            if (_client != null && _client.Connected)
            {
                Receive(_client);
                return true;
            }
            else return false;
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Clear();
                    state.sb.Append(Encoding.Default.GetString(state.buffer, 0, bytesRead));

                   
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        string content = state.sb.ToString();
                        SocketDataReceivedArgs e = new SocketDataReceivedArgs(content);
                        if(content.IndexOf("<STOP>") > -1)
                        {
                            this.Close();
                            return;
                        }
                        else if(content.IndexOf("START_LOG") > -1)
                        {
                            if (this.StartLogging != null)
                                this.StartLogging(this, e);
                        }
                        else if(content.IndexOf("STOP_LOG") > -1)
                        {
                            if(this.StopLogging != null)
                            {
                                this.StopLogging(this, e);
                            }
                        }
                        else if(this.DataReceived != null)
                            this.DataReceived(this, e);
                    }

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public bool Send(byte[] data)
        {
            if (_client != null && _client.Connected)
            {
                Send(_client, data);
                return true;
            }
            else return false;
        }

        private void Send(Socket client, byte[] byteData)
        {
            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);

                Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Close()
        {
            if (_client!=null && _client.Connected)
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
        }
    }
}