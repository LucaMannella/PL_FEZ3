using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;


namespace Client
{
    public partial class Program
    {
        public const String DEFAULT_MY_IP = "192.168.137.2";
        public const String DEFAULT_DESTINATION_IP = "192.168.137.1";
        public const String MASK = "255.255.255.0";

        private AnalogInput pir_sensor;
        private AnalogOutput buzzer_sensor;
        private bool scatta_allarme = false;

        Bitmap bitmapA = null;
        Int32 RGlobal;
        Int32 PreviousAverage;

        /**
         * This method is run when the mainboard is powered up or reset.   
         */
        void ProgramStarted()
        {
            Debug.Print("Program Started");     // to show messages in "Output" window during debugging
            Mainboard.SetDebugLED(true);

            InitSensors();
            GT.Timer timer_pir = new GT.Timer(1000);
            timer_pir.Tick += PirDetection;
            timer_pir.Start();       
            
            SetupEthernet();

            camera.PictureCaptured += camera_PictureCaptured;
            button.ButtonPressed += button_ButtonPressed;
            camera.BitmapStreamed += camera_BitmapStreamed;

            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkUp += OnNetworkUp;
            ethernetJ11D.NetworkDown += OnNetworkDown;

            camera.StartStreaming();
        }


// ------------------------- Network & Connections ------------------------- //
        private void OnNetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network up!");
            multicolorLED.TurnGreen();
            ListNetworkInterfaces();

            // implementare qui binding con il proxy
            // 1) binding con ws
            // 2) getAddressWithPort
        }

        private void OnNetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down!");
            multicolorLED.TurnRed();
        }

        void SetupEthernet()
        {
            // ethernetJ11D.UseDHCP();
            ethernetJ11D.UseStaticIP(DEFAULT_MY_IP, MASK, DEFAULT_DESTINATION_IP);
        }

        void SetupConnection(String MyIP, String Mask, String DestIp)
        {
            ethernetJ11D.UseStaticIP(MyIP, Mask, DestIp);
        }
// ----------------------- End Network & Connections ----------------------- //


// -------------------------------- Sensori -------------------------------- //
        private void InitSensors()
        {
            Mainboard.SetDebugLED(true);
            Gadgeteer.Socket socket = Gadgeteer.Socket.GetSocket(9, true, null, null);
            pir_sensor = extender.CreateAnalogInput(Gadgeteer.Socket.Pin.Four);
            buzzer_sensor = extender.CreateAnalogOutput(Gadgeteer.Socket.Pin.Five);
            return;
        }

        private void ScattaAllarme()
        {       
            while (true)
            {
                buzzer_sensor.WriteProportion(1);
                Thread.Sleep(1);
                buzzer_sensor.WriteProportion(0);
            }            
        }

        private void SpegniAllarme()
        {
            buzzer_sensor.WriteProportion(0);
            return;
        }

        private void PirDetection(GT.Timer timer)
        {
            if (pir_sensor.ReadVoltage() > 3)
            {
                Debug.Print("beccato!!!");
                //codice da eseguire quando scatta pir
            }            
        }
// ------------------------------ End Sensori ------------------------------ //


        private void camera_BitmapStreamed(GTM.GHIElectronics.Camera sender, Bitmap e)
        {            
            displayT35.SimpleGraphics.DisplayImage(e, 0, 0);    
           
        }

        private void button_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            Debug.Print("Button pressed!");
            camera.StopStreaming();
            camera.TakePicture();

            // invio della prima immagine
        }

        private void camera_PictureCaptured(GTM.GHIElectronics.Camera sender, GT.Picture picture)
        {
            Int32 RB = 0;
            Bitmap bitmapB = picture.MakeBitmap();

            Debug.Print("Image captured! " +
                    "Size: " + picture.PictureData.Length.ToString());

            //per gestire la prima volta
            if (bitmapA == null)
            {
                bitmapA = bitmapB;
                RGlobal = euristicSum(bitmapA);
                PreviousAverage = RGlobal / 9;
                return;
            }

            Debug.Print(DateTime.Now.ToString());
            RB = euristicSum(bitmapB);
            Debug.Print(DateTime.Now.ToString());

            Debug.Print(PreviousAverage.ToString());
            Int32 average = (RB / 9);
            Debug.Print(average.ToString());

            if (System.Math.Abs(PreviousAverage - average) > 45)    //SOGLIA LIMITE 40/50
            {
                Debug.Print("Suspicious picture!");
                sendPicture(picture.PictureData);
            }

            RGlobal = RB;
            PreviousAverage = average;
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);
        } 

        void ListNetworkInterfaces()
        {
            var settings = ethernetJ11D.NetworkSettings;

            Debug.Print("------------------------------------------------");
            // Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
            Debug.Print("IP Address:   " + settings.IPAddress);
            Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
            Debug.Print("Subnet Mask:  " + settings.SubnetMask);
            Debug.Print("Gateway:      " + settings.GatewayAddress);
            Debug.Print("------------------------------------------------");
        }


        /**
         * This method sum the value of green of 9 squares
         * of dimension 8x8 taken from a bitmap.
         */
        public Int32 euristicSum(Microsoft.SPOT.Bitmap bitmapB)
        {
            Int32 RA = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    RA += ColorUtility.GetGValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 116; y < 124; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 232; y < 240; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 232; y < 240; y++)
            {
                for (int x = 156; x < 164; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 116; y < 124; y++)
            {
                for (int x = 156; x < 164; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 0; y < 8; y++)
            {
                for (int x = 156; x < 164; x++)
                {
                    RA += ColorUtility.GetGValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 0; y < 8; y++)
            {
                for (int x = 312; x < 320; x++)
                {
                    RA += ColorUtility.GetGValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 116; y < 124; y++)
            {
                for (int x = 312; x < 320; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            for (int y = 232; y < 240; y++)
            {
                for (int x = 312; x < 320; x++)
                {
                    RA += ColorUtility.GetRValue(bitmapB.GetPixel(x, y));
                }
            }

            return RA;
        }

        void sendPicture(byte[] e)
        {
            using (Socket clientSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp))
            {
                // Addressing
                IPAddress ipAddress = IPAddress.Parse(DEFAULT_DESTINATION_IP);
                IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, 1500);

                // Connecting
                Debug.Print("Connecting to server " + serverEndPoint + ".");

                clientSocket.Connect(serverEndPoint);
                Debug.Print("Connected to server.");

                // Sending

                //byte[] messageBytes = Encoding.UTF8.GetBytes(e.GetBitmap());
                clientSocket.Send(e);
                clientSocket.Close();
                /*
                byte[] inBuffer = new byte[100];
                int count = clientSocket.Receive(inBuffer);
                char[] chars = Encoding.UTF8.GetChars(inBuffer);
                string str = new string(chars, 0, count);
                */
            }
        }


    }
}
