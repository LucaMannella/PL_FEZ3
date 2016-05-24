using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Server
{
    class MySocket
    {
        private Socket s;

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
        public void receiveFile(string path_file, long lung)
        {
            FileStream fileW = File.Open(path_file, FileMode.Create);
            byte[] buffer = new byte[20480];

            long totRicevuti = 0;
            int ricevuti;
            long mancanti;
            while ((mancanti = lung - totRicevuti) > 0)
            {
                if (mancanti >= 20480)
                    ricevuti = s.Receive(buffer, 20480, SocketFlags.None);
                else
                    ricevuti = s.Receive(buffer, (int)mancanti, SocketFlags.None);
                fileW.Write(buffer, 0, ricevuti);
                totRicevuti += ricevuti;
            }

            fileW.Seek(0, SeekOrigin.Begin);            
            fileW.Close();
            return;
        }
    }
}
