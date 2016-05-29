using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace FinalService
{

    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Service1 : IService1
    {
        private const string IPADDR = "192.168.1.54";
        private const int PORT = 1500;
        private const string GETPORT = "getPort\0";
        private const string KEEPALIVE = "keepAlive\0";
        private const string OK = "200OK";

        public string[] getServerAddressWithPort(string myMacAddress)
        {
            string[] bRet = new string[3];

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // Addressing
            IPAddress ipAddress = IPAddress.Parse(IPADDR);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, PORT);
            
            clientSocket.Connect(serverEndPoint);

            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(GETPORT);

            byte[] toSend2 = System.Text.Encoding.UTF8.GetBytes((myMacAddress+'\0'));
            

            byte[] bufferReceive = new byte[1000];

           
            Send(clientSocket, toSend, 0, toSend.Length, 5000);
            Send(clientSocket, toSend2, 0, toSend2.Length, 5000);

            string port = receive(clientSocket);

            clientSocket.Close();

            bRet[0] = IPADDR;
            bRet[1] = port.Remove(port.Length - 1);
            bRet[2] = CurrentTimeMillis().ToString();
            
            /*
            byte[] buf = new byte[byterecived];

            for(int i=0; i<byterecived; i++){
                buf[i] = bufferReceive[i];
            }

          */

            return bRet;
        }

        private static readonly DateTime Jan1st1970 = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }


        public Boolean keepAlive(string myMacAddress, string mycurrentTime, int port)
        {

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Addressing
            IPAddress ipAddress = IPAddress.Parse(IPADDR);
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);


            byte[] bufferReceive = new byte[1000];
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(KEEPALIVE);
            byte[] toSend2 = System.Text.Encoding.UTF8.GetBytes((myMacAddress + '\0'));
            byte[] toSend3 = System.Text.Encoding.UTF8.GetBytes((mycurrentTime + '\0'));

            clientSocket.Connect(serverEndPoint);

            Send(clientSocket, toSend, 0, toSend.Length, 5000);
            Send(clientSocket, toSend2, 0, toSend2.Length, 5000);
            Send(clientSocket, toSend3, 0, toSend3.Length, 5000);

            string response = receive(clientSocket);
         
            clientSocket.Close();

            if (response.Remove(response.Length - 1).Equals(OK))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (sent < size);
        }

        public static int Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (received < size);

            return received;
        }

        public string receive(Socket s)
        {
            byte[] buffer = new byte[1000];
            int ricevuti = 0;
            char vOut = 'a';
            while (vOut != '\0')
            {
                try
                {
                    ricevuti += s.Receive(buffer, ricevuti, 1, SocketFlags.None);
                    vOut = Convert.ToChar(buffer[ricevuti - 1]);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                } 

            }

            byte[] buf = new byte[ricevuti];

            for (int i = 0; i < ricevuti; i++)
            {
                buf[i] = buffer[i];
            }

            return System.Text.Encoding.UTF8.GetString(buf); ;
        }
    }
}
