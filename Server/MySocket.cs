using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;


namespace Server
{
    class MySocket
    {
        private Socket s;
        private int cont = 0;

        public MySocket(Socket s)
        {
            this.s = s;
        }

        public bool Connected { get { return s.Connected; } }

        public EndPoint RemoteEndPoint { get { return s.RemoteEndPoint; } }

        public int ReceiveTimeout { get { return s.ReceiveTimeout; } set { s.ReceiveTimeout = value; } }

        public int Receive(byte[] buffer, int lung, SocketFlags socketFlags)
        {
            int mancanti = lung;
            int totRicevuti = 0;
            int ricevuti;
            while ((mancanti = lung - totRicevuti) > 0)
            {
                if ((ricevuti = s.Receive(buffer, totRicevuti, mancanti, SocketFlags.None)) == 0)
                    return 0;
                totRicevuti += ricevuti;
            }
            return lung;
        }

        internal void Shutdown(SocketShutdown w)
        {
            s.Shutdown(w);
        }

        internal void Close()
        {
            s.Close();
        }

        internal int Send(byte[] buffer, int lung, SocketFlags socketFlags)
        {
            return s.Send(buffer, lung, socketFlags);
        }

        internal int Send(byte[] bytes)
        {
            return s.Send(bytes);
        }

        internal void SendFile(string fileName)
        {
            s.SendFile(fileName);
        }
        public Bitmap receiveFile(long lung)
        {
            byte[] buffer = new byte[lung];
          
            
            long totRicevuti = 0;
            int ricevuti=0;
            long mancanti;
            while ((mancanti = lung - totRicevuti) > 0)
            {
                if (mancanti >= lung)
                    ricevuti = s.Receive(buffer, (int) lung, SocketFlags.None);
                else
                    ricevuti = s.Receive(buffer, (int) totRicevuti, (int)mancanti, SocketFlags.None);                
                totRicevuti += ricevuti;
            }


             var imageConverter = new ImageConverter();
             var image = (Image)imageConverter.ConvertFrom(buffer);
             Bitmap a =  new Bitmap(image);
             a.Save(@"C:\Users\Alfonso-LAPTOP\Desktop\image"+cont+".jpg");
             cont++;
            /*
            var ms = new MemoryStream();
            ms.Write(buffer, 0, (int)lung);
            ms.Seek(0, SeekOrigin.Begin);
            Bitmap a = Bitmap.FromStream(ms;
            System.Drawing.Image image = Image.FromStream(ms);
            
            image.Save(@"C:\Users\Alfonso-LAPTOP\Desktop\image.jpg");           */
          

            return a;
           
        }
    }
}
