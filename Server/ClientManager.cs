using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientManager
    {
        private String myMac;
        private int porta;
        MotionDetector3Optimized motionDetector;
        //oppure usare:
        //MotionDetector4 motionDetector = new MotionDetector4();
        System.Drawing.Bitmap lastFrame = null;
        //grandezza immagine che mi aspetto di ricevere
        long lungImage = 230454;
        //livello che dice quando far scattare allarme ed è compreso tra 0 e 1
        private double alarmLevel = 0.005;
        private const string OK = "200OK\0";
        private long lasttime = 0;

        public ClientManager(String mac, int porta)
        {
            this.myMac = mac;
            this.porta = porta;
            this.motionDetector = new MotionDetector3Optimized();

        }


        public void start()
        {
            Socket handlerClient;
            IPAddress ip = IPAddress.Parse("192.168.1.54");
            IPEndPoint localEndPoint = new IPEndPoint(ip, porta);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           

            try
            {

                Thread keepalive = new Thread(() => checkKeepAlive());
                keepalive.Start();

                listener.Bind(localEndPoint);
                listener.Listen(2);

                // Start listening for connections.

                while (true)
                {
                    Console.WriteLine("Waiting for a connection..." + "from client: " + this.myMac + "on port:" + porta);
                    // Program is suspended while waiting for an incoming connection.
                    handlerClient = listener.Accept();

                    MySocket clientsock = new MySocket(handlerClient);

                    String cmd = clientsock.receiveString();
                    switch (cmd)
                    {
                        case "keepAlive\0":
                            Console.WriteLine(cmd);
                            String macaddr2 = clientsock.receiveString();
                            String time = clientsock.receiveString();
                            if (macaddr2.Equals(this.myMac + '\0'))
                            {
                                ;
                            }
                            Console.WriteLine("Received keepalive from: " + macaddr2 + "at: " + time);
                            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(OK);
                            clientsock.Send(toSend, toSend.Length, SocketFlags.None);
                            clientsock.Close();
                            break;

                        case "manageImage\0":

                            Thread thread = new Thread(() => elaborazione(clientsock));
                            thread.Start();
                            break;

                        default:
                            Console.WriteLine("Error : Unknokwn command");
                            clientsock.Close();
                            break;
                    }


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


        private void checkKeepAlive()
        {
            Boolean check= true;

            while (check)
            {
                if(lasttime != 0){
                    if (DateTime.Now.Millisecond - lasttime > 15000)
                    {
                        //TODO client morto segnalare
                    }
                }
                
            }

        }


        private void elaborazione(MySocket clientsock)
        {
            //abilito il calcolo
            motionDetector.MotionLevelCalculation = true;
            //mi faccio mandare l'immagine
            System.Drawing.Bitmap current = clientsock.receiveFile(lungImage);
            processImage(current);

        }

        private void processImage(System.Drawing.Bitmap current)
        {
            if (lastFrame != null)
            {
                lastFrame.Dispose();
            }

            lastFrame = (System.Drawing.Bitmap)current.Clone();

            // apply motion detector
            if (motionDetector != null)
            {
                motionDetector.ProcessFrame(ref lastFrame);

                // check motion level
                if (motionDetector.MotionLevel >= alarmLevel)
                {
                    //funzione che esegue operazioni quando scatta allarme
                }
            }
        }

    }





}
