using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.StartListening();

        }

        private void StartListening()
        {
            Socket handler;            
            //IPAddress ip = IPAddress.Parse("10.13.15.211");
            //IPEndPoint localEndPoint = new IPEndPoint(ip, 1500);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1500);            
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
               
                // Start listening for connections.

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();                     
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
                return;
            }
        }
    }
}
