using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientManager
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const string OK = "200OK\0";
        private const string UNKNOW_COMMND = "400BAD_REQUEST-";
        private const string ALARM = "yes-";
        private const string NOALARM = "no-";
        MySocket clientsock;
        private Boolean isdead = false;
        private Boolean keepalive = true;
       // MotionDetector3Optimized motionDetector;
        //oppure usare:
        MotionDetector4 motionDetector;

        byte[] referenceBitmap;
        System.Drawing.Bitmap lastFrame = null;

        long lungImage = 230454;    //grandezza immagine che mi aspetto di ricevere

        //livello che dice quando far scattare allarme ed è compreso tra 0 e 1
        private double alarmLevel = 0.1;

        int cont = 0;
        private String myMac;
        private int porta;
        private long lastTime = 0;
        private Database mDatabase;


        public ClientManager(String mac, int porta)
        {
            this.myMac = mac;
            this.porta = porta;
            this.motionDetector = new MotionDetector4(mac);
            this.mDatabase = Database.getInstance();
        }


        public void start()
        {
            Socket handlerClient;
            IPAddress ip = IPAddress.Parse(Constants.SERVER_IP_ADDR);
            IPEndPoint localEndPoint = new IPEndPoint(ip, porta);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Boolean ok;
            mDatabase.OpenConnect();

            try {
                Thread keepalive = new Thread(() => checkKeepAlive());
                keepalive.Start();
                
                listener.Bind(localEndPoint);
                listener.Listen(2); // Start listening for connections.
            }
            catch (Exception e) {
                Console.WriteLine("Unexpected Error");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
                mDatabase.CloseConnect();
                return;
            }

            while (!isdead)
            {
                try
                {
                    Console.WriteLine("Waiting for a connection..." + "from client: " + this.myMac + "on port:" + porta);
                    Console.WriteLine("");
                    // Program is suspended while waiting for an incoming connection.
                    handlerClient = listener.Accept();

                    clientsock = new MySocket(handlerClient);

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
                                lastTime = Int64.Parse(time);
                            }
                                
                            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(OK);
                            clientsock.Send(toSend, toSend.Length, SocketFlags.None);
                            clientsock.Close();
                            break;

                        case "firstImage\0":
                            Console.WriteLine("Received first image from: " + myMac);
                            System.Drawing.Bitmap current1 = receiveFile(lungImage, clientsock.s);
                            long time1 = CurrentTimeMillis();

                            String strValue = myMac;
                            strValue = Regex.Replace(strValue, @"-", "");
                            strValue = strValue.Remove(strValue.Length - 1);
                           
                            String pictureFolderName = strValue+"\\firstimage"+time1+".jpg";
                            String picturePath = Constants.SERVER_DIRECTORY + Constants.IMAGES_FOLDER + pictureFolderName;
                            String relativePath = Constants.IMAGES_FOLDER + pictureFolderName;

                            bool exists = System.IO.Directory.Exists(Constants.SERVER_DIRECTORY + Constants.IMAGES_FOLDER + "\\" + strValue);
                            if(!exists)
                                System.IO.Directory.CreateDirectory(Constants.SERVER_DIRECTORY + Constants.IMAGES_FOLDER + "\\" + strValue);
                            try
                            {
                                current1.Save(picturePath);
                                String path = relativePath.Replace("\\", "/");
                                ok = mDatabase.insertSuspiciousPicturePath(myMac, time1, @"./" + path);
                                if (!ok)
                                    Console.WriteLine("Error: Impossible to store picture: " + picturePath + " on the database!\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error! Impossible to store the received picture! Exception: " + ex.ToString() + "\n");
                            }
                            Thread thread = new Thread(() => processImage(current1, clientsock));
                            thread.Start();
                            break;

                        case "manageImage\0":
                            Console.WriteLine("Received image from: " + myMac);
                            System.Drawing.Bitmap current = receiveFile(lungImage, clientsock.s);
                            Thread thread2 = new Thread(() => processImage(current, clientsock));
                            thread2.Start();
                            break;

                        case "unbind\0":
                            Console.WriteLine("Received unBind from: " + myMac);
                            isdead = true;
                            keepalive = false;
                            String mac = myMac.Remove(myMac.Length - 1);
                            mDatabase.removeClient(mac);
                            break;

                        default:
                            Console.WriteLine("Error : Unknokwn command");
                            byte[] toSend2 = System.Text.Encoding.UTF8.GetBytes(UNKNOW_COMMND);
                            clientsock.Send(toSend2, toSend2.Length, SocketFlags.None);
                            clientsock.Close();
                            break;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine("Unexpected Error");
                    Console.WriteLine("Source : " + e.Source);
                    Console.WriteLine("Message : " + e.Message);
                }
                finally {
                    mDatabase.CloseConnect();
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
            while (keepalive) {
                if(lastTime != 0) {
                    if (CurrentTimeMillis() - lastTime > 50000)
                    {
                        Console.WriteLine("Warning client disconnected \n");
                        String mac = myMac.Remove(myMac.Length-1);
                        String subject = "Warning client disconnected";
                        String message = "The client: "+mac+" has shut down at: "
                            + DateTime.Now.ToLongTimeString()+" of: "+DateTime.Now.ToLongDateString();

                        mDatabase.removeClient(mac);  // removing the client from the database
                        sendMail(subject, message, null);
                        keepalive = false;
                        clientsock.Close();
                        isdead = true;
                    }
                }
            }

        }


        private void processImage(Bitmap current, MySocket socket)
        {   
            //abilito il calcolo
            motionDetector.MotionLevelCalculation = true;

            if (lastFrame != null)
                lastFrame.Dispose();

            lastFrame = (System.Drawing.Bitmap)current.Clone();

            // apply motion detector
            if (motionDetector != null)
            {
                motionDetector.ProcessFrame(ref lastFrame);

                // check motion level
                if (motionDetector.MotionLevel >= alarmLevel)
                {
                    Console.WriteLine("MotionLevel:"+ motionDetector.MotionLevel);
                    //funzione che esegue operazioni quando scatta allarme
                    byte[] toSend = System.Text.Encoding.UTF8.GetBytes(ALARM);
                    socket.Send(toSend, toSend.Length, SocketFlags.None);
                    socket.Close();
                    Console.WriteLine("Allarme Scattato!\n");
                    isdead = true;
                    keepalive = false;
                    String mac = myMac.Remove(myMac.Length - 1);
                    mDatabase.removeClient(mac);
                    sendMail("Alarm", "The client" + mac +" has detected a sospicious motion, the alarm was thrown", MotionDetector4.lastimage);

                }
                else
                {
                    byte[] toSend = System.Text.Encoding.UTF8.GetBytes(NOALARM);
                    socket.Send(toSend, toSend.Length, SocketFlags.None);
                    socket.Close();
                }
            }

            return;
        }


        public Bitmap receiveFile(long lung, Socket s)
        {
            
            byte[] buffer = new byte[lung];

            long totRicevuti = 0;
            int ricevuti = -1;
            long mancanti = lung - totRicevuti;

            while( (mancanti = lung - totRicevuti) > 0)
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
            String mac = myMac.Remove(myMac.Length - 1);
            var fromAddress = new MailAddress(Constants.MAIL_SENDER, "FEZ SuvellianceCamera");
            var toAddress = new MailAddress(mDatabase.getClientEmail(mac), "Customer");
            String fromPassword = Constants.MAIL_SENDER_PASSWORD;

            SmtpClient smtpclient = new SmtpClient();

            smtpclient.Host = "smtp.gmail.com";
            smtpclient.Port = 587;
            smtpclient.EnableSsl = true;
            smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpclient.UseDefaultCredentials = false;
            smtpclient.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
            MailMessage mymailmex = null;
            try
            {
                mymailmex = new MailMessage(fromAddress, toAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Impossible to send the mail: " + e.Message + "\n");
                return;
            }
           
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


        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

    }

}
