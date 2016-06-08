using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        int cont = 0;
        MotionDetector3Optimized motionDetector;
        byte[] referenceBitmap;
        //oppure usare:
        //MotionDetector4 motionDetector = new MotionDetector4();
        System.Drawing.Bitmap lastFrame = null;
        //grandezza immagine che mi aspetto di ricevere
        long lungImage = 230454;
        //livello che dice quando far scattare allarme ed è compreso tra 0 e 1
        private double alarmLevel = 0.005;
        private const string OK = "200OK\0";
        private const string UNKNOW_COMMND = "400BAD_REQUEST-";
        private const string ALARM = "yes-";
        private const string NOALARM = "no-";
        private long lasttime = 0;
        private Database mDatabase;

        public ClientManager(String mac, int porta)
        {
            this.myMac = mac;
            this.porta = porta;
            this.motionDetector = new MotionDetector3Optimized();
            this.mDatabase = Database.getInstance();

        }


        public void start()
        {
            Socket handlerClient;
            IPAddress ip = IPAddress.Parse("192.168.137.1");
            IPEndPoint localEndPoint = new IPEndPoint(ip, porta);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            mDatabase.OpenConnect();
            bool result = mDatabase.insertClient(this.myMac, this.porta);
            mDatabase.CloseConnect();

            if(!result){
                Console.WriteLine("Error: Impossible to save new client in db");
            }

            try
            {

                Thread keepalive = new Thread(() => checkKeepAlive());
                keepalive.Start();

                listener.Bind(localEndPoint);
                listener.Listen(2);

                // Start listening for connections.
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
                return;
            }
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Waiting for a connection..." + "from client: " + this.myMac + "on port:" + porta);
                        Console.WriteLine("");
                        // Program is suspended while waiting for an incoming connection.
                        handlerClient = listener.Accept();

                        MySocket clientsock = new MySocket(handlerClient);

                        String cmd = receiveString(clientsock.s);
                        switch (cmd)
                        {
                            case "keep\0":
                                Console.WriteLine(cmd);
                                String macaddr2 = clientsock.receiveString();
                                String time = clientsock.receiveString();
                                if (macaddr2.Equals(this.myMac))
                                {
                                    Console.WriteLine("Received keepalive from: " + macaddr2 + "at: " + time);
                                    lasttime = Int64.Parse(time);
                                }
                                
                                byte[] toSend = System.Text.Encoding.UTF8.GetBytes(OK);
                                clientsock.Send(toSend, toSend.Length, SocketFlags.None);
                                clientsock.Close();
                                break;

                            case "firstImage\0":
                                Console.WriteLine("Received first image from: " + myMac);
                                System.Drawing.Bitmap current1 = receiveFile(lungImage, clientsock.s);
                                Thread thread = new Thread(() => elaborazione(current1, clientsock));
                                thread.Start();
                                break;


                            case "manageImage\0":
                                Console.WriteLine("Received image from: " + myMac);
                                System.Drawing.Bitmap current = receiveFile(lungImage, clientsock.s);
                                Thread thread2 = new Thread(() => elaborazione(current, clientsock));
                                thread2.Start();
                                break;

                            default:
                                Console.WriteLine("Error : Unknokwn command");
                                byte[] toSend2 = System.Text.Encoding.UTF8.GetBytes(UNKNOW_COMMND);
                                clientsock.Send(toSend2, toSend2.Length, SocketFlags.None);
                                clientsock.Close();
                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected Error");
                        Console.WriteLine("Source : " + e.Source);
                        Console.WriteLine("Message : " + e.Message);
                    }
                }
        }


        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void checkKeepAlive()
        {
            Boolean check= true;

            while (check)
            {
                if(lasttime != 0){
                    if (CurrentTimeMillis() - lasttime > 20000)
                    {
                        String subject = "Warning client disconnected";
                        String message = "The client: " + myMac.Remove(myMac.Length - 1) + " is shut down at: " + DateTime.Now.ToLongTimeString() +" of: " + DateTime.Now.ToLongDateString();
                        sendMail(subject, message, null);
                        break;
                    }
                }
                
            }

        }


        private void elaborazione(Bitmap current, MySocket socket)
        {
            //abilito il calcolo
            motionDetector.MotionLevelCalculation = true;
            processImage(current, socket);

        }

        private void processImage(System.Drawing.Bitmap current, MySocket socket)
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
                    byte[] toSend = System.Text.Encoding.UTF8.GetBytes(ALARM);
                    socket.Send(toSend, toSend.Length, SocketFlags.None);
                    socket.Close();
                    Console.WriteLine("Scatta allarme");
                    Console.WriteLine("");
                }
                else
                {
                    byte[] toSend = System.Text.Encoding.UTF8.GetBytes(NOALARM);
                    socket.Send(toSend, toSend.Length, SocketFlags.None);
                    socket.Close();
                }
            }
        }


        public Bitmap receiveFile(long lung, Socket s)
        {
            byte[] buffer = new byte[lung];


            long totRicevuti = 0;
            int ricevuti = -1;
            long mancanti = lung - totRicevuti;
            while ((mancanti = lung - totRicevuti) > 0)
            {
                if (mancanti >= lung)
                    ricevuti = s.Receive(buffer, (int)lung, SocketFlags.None);
                else
                    ricevuti = s.Receive(buffer, (int)totRicevuti, (int)mancanti, SocketFlags.None);
                totRicevuti += ricevuti;
            }


            var imageConverter = new ImageConverter();
            var image = (Image)imageConverter.ConvertFrom(buffer);
            Bitmap a = new Bitmap(image);
            a.Save(@"C:\Users\Alfonso-LAPTOP\Desktop\image" + cont + ".jpg");
            cont++;        
            return a;

        }

        public byte[] receiveFileAsByteArray(long lung, Socket s)
        {
            byte[] buffer = new byte[lung];


            long totRicevuti = 0;
            int ricevuti = -1;
            long mancanti = lung - totRicevuti;
            while ((mancanti = lung - totRicevuti) > 0)
            {
                if (mancanti >= lung)
                    ricevuti = s.Receive(buffer, (int)lung, SocketFlags.None);
                else
                    ricevuti = s.Receive(buffer, (int)totRicevuti, (int)mancanti, SocketFlags.None);
                totRicevuti += ricevuti;
            }


            return buffer;

        }

        private void sendMail(String subject, String message, String attachmentFilename)
        {
            var fromAddress = new MailAddress("fez03noreply@gmail.com", "From Name");
            var toAddress = new MailAddress("valenzise@tiscali.it", "To Name");
            String fromPassword = "fez03password";

            SmtpClient smtpclient = new SmtpClient();

            smtpclient.Host = "smtp.gmail.com";
            smtpclient.Port = 587;
            smtpclient.EnableSsl = true;
            smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpclient.UseDefaultCredentials = false;
            smtpclient.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);


            MailMessage mymailmex = new MailMessage(fromAddress, toAddress);
            mymailmex.Subject = subject;
            mymailmex.Body = message;

            if (attachmentFilename != null)
                mymailmex.Attachments.Add(new Attachment(attachmentFilename));

            try
            {
                smtpclient.Send(mymailmex);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
            
        }


        public string receiveString(Socket socket)
        {

            byte[] buffer = new byte[1000];
            int ricevuti = 0;
            char vOut = 'a';
            while (vOut != '-')
            {
                ricevuti += socket.Receive(buffer, ricevuti, 1, SocketFlags.None);
                vOut = Convert.ToChar(buffer[ricevuti - 1]);

            }

            byte[] buf = new byte[ricevuti];

            for (int i = 0; i < ricevuti - 1; i++)
            {
                buf[i] = buffer[i];
            }

            return System.Text.Encoding.UTF8.GetString(buf);
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }


    }

    


}
