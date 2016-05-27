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
        MotionDetector3Optimized motionDetector = new MotionDetector3Optimized();
        //oppure usare:
        //MotionDetector4 motionDetector = new MotionDetector4();
        System.Drawing.Bitmap lastFrame = null;
        //grandezza immagine che mi aspetto di ricevere
        long lungImage = 230454;
        //livello che dice quando far scattare allarme ed è compreso tra 0 e 1
        private double alarmLevel = 0.005;
        
        static void Main(string[] args)
        {
            Program p = new Program(); 
            p.StartListening();
        }

        private void StartListening()
        {
            Socket handler;            
            IPAddress ip = IPAddress.Parse("192.168.137.1");
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
                    ThreadStart threadDelegate = new ThreadStart(elaborazione);
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

        
        private void elaborazione()
        {
            //abilito il calcolo
            motionDetector.MotionLevelCalculation = true;
            //mi faccio mandare l'immagine
            System.Drawing.Bitmap current=s.receiveFile(lungImage);
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
