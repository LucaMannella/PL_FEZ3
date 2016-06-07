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

        private int startingport = Constants.PORT + 1;
        private int progressiveport = 0;
        private const string OK = "200OK\0";

        /**
         * The entry point of the server.
         */ 
        static void Main(string[] args)
        {
            Database db = Database.getInstance();
            if (db.removeAllClient())
                Console.WriteLine("The database was succesfully initialized!");
            else
                Console.WriteLine("Warning: Problem in database initialization...\n" +
                            "if it is the first time that you start our system, ignore this warning!");

            Program p = new Program();
            p.StartListening();
        }

        /**
         * This method create a socket and start listening for clients requests.
         */ 
        private void StartListening()
        {
            Socket handler;
            IPAddress ip = IPAddress.Parse(Constants.SERVER_IP_ADDR);
            IPEndPoint localEndPoint = new IPEndPoint(ip, 1500);
            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1500);            
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            

            try {
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
            catch (Exception e) {
                Console.WriteLine("Unexpected Error");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
                return;
            }
        }

        /**
         * This method manage the execution of a client request.
         */ 
        private void manageCommand() {
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
                    
                default:
                    break;
            }
        }

        /**
         * This method manage the comunication between a client and the server.
         * It should be instatiated as a thread.
         */ 
        private void manageNewClient(string macaddr, int port)
        {
            ClientManager client = new ClientManager(macaddr, port);
            client.start();
        }

        /**
         * This method sends to the client the port that must be used
         * to contact the server.
         */ 
        private void sendPort(int port)
        {
            string myString = port.ToString() + '\0';
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(myString);

            s.Send(toSend, toSend.Length, SocketFlags.None);
        }

    }

}
