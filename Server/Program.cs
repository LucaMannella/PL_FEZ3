using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    class Program
    {
        MySocket s;

        private static string IPADDR = "192.168.1.54";
        private int startingport = 1501;
        private int progressiveport = 0;
        private const string OK = "200OK\0";

        
        static void Main(string[] args)
        {
            Program p = new Program(); 
            p.StartListening();
        }

        private void StartListening()
        {
            Socket handler;
            IPAddress ip = IPAddress.Parse(IPADDR);
            IPEndPoint localEndPoint = new IPEndPoint(ip, 1500);
            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1500);            
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
                   
                    s = new MySocket(handler);
                    ThreadStart threadDelegate = new ThreadStart(manageCommand);
                    Thread newThread = new Thread(threadDelegate);
                    newThread.Start();
                     
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



        private void manageCommand(){
            String cmd = s.receiveString();
            switch (cmd)
            {
                case "getPort\0":
                    Console.WriteLine(cmd);
                    String macaddr = s.receiveString();
                    Console.WriteLine(macaddr);

                    int port = startingport + progressiveport;
                    progressiveport++;

                    Thread thread = new Thread(() => manageNewClient(macaddr, port));
                    thread.Start();

                    
                    sendPort(port);

                    break;
                case "keepAlive\0":
                    Console.WriteLine(cmd);
                    String macaddr2 = s.receiveString();
                    String time = s.receiveString();
                    Console.WriteLine("Received keepalive from: "+ macaddr2 + "at: " +time);

                    //TODO check keepalive

                    byte[] toSend = System.Text.Encoding.UTF8.GetBytes(OK);
                    s.Send(toSend, toSend.Length, SocketFlags.None);
                    break;
                default:
                    break;
            }
            

        }

        private void sendPort(int port)
        {
            string myString = port.ToString() + '\0';


            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(myString);


            s.Send(toSend, toSend.Length, SocketFlags.None);

        }

        private void manageNewClient(string macaddr, int port)
        {
            ClientManager client = new ClientManager(macaddr, port);
            client.start();

        }

       


  
    }
}
