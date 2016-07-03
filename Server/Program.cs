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
        private static Database db;

        private int startingport = Constants.PORT + 1;
        private int progressiveport = 0;
        private const string OK = "200OK\0";

        /**
         * The entry point of the server.
         */ 
        static void Main(string[] args)
        {
            db = Database.getInstance();
            db.OpenConnect();

            if (db.removeAllClient())
                Console.WriteLine("The database was succesfully initialized!");
            else
                Console.WriteLine("Warning: Problem in database initialization...\n" +
                            "if it is the first time that you start our system, ignore this warning!");

            Program p = new Program();
            p.StartListening();

            db.CloseConnect();
            return;
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
                    
                    new Thread(manageCommand).Start();
                }
            }
            catch (Exception e) {
                Console.WriteLine("Unexpected Error");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            finally {
                db.CloseConnect();
            }
        }

        /**
         * This method manage the execution of a client request.
         */ 
        private void manageCommand() {
            String cmd = s.receiveString();
            try
            {
                switch (cmd)
                {
                    case "getPort\0":
                        Console.WriteLine(cmd);
                        String macaddr = s.receiveString();
                        Console.WriteLine(macaddr);

                        String macToCheck = macaddr.Substring(0, macaddr.Length - 1);

                        int port = startingport + progressiveport;
                        progressiveport++;

                        Boolean ok = db.insertClient(macToCheck, port);
                        if (!ok)
                        {
                            Console.WriteLine("Error: Impossible to save new client in db!");
                            sendPort(-1);
                            return;
                        }

                        sendPort(port);

                        // This method manage the comunication between this client and the server.
                        ClientManager client = new ClientManager(macaddr, port);
                        client.start();

                        Console.WriteLine("Client: " + macaddr + " on port: " + port + " has ended!\n");
                        break;

                    default:
                        Console.WriteLine("Error: received unknown request!\nCommand: " + cmd + "\n");
                        break;
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error: manage commeand "+e.Message+"\n");

                if(s != null){
                    s.Close();
                }
                
            }
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
