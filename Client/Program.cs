using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Net.NetworkInformation;
using tempuri.org;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;
using Ws.Services.Binding;
using Ws.Services;


namespace Client
{
    public partial class Program
    {
        public const String DEFAULT_MY_IP = "192.168.137.2";
        public const String DEFAULT_DESTINATION_IP = "192.168.137.1";
        public const String DEFAULT_PORT = "1500";
        public const String MASK = "255.255.255.0";
        public const String SERVICE_ADDR = "http://192.168.137.1:8733/Service/";
        public const String MANAGE_IMAGE_COMMAND = "manageImage\0";
        String[] connectionInfo = { DEFAULT_DESTINATION_IP, DEFAULT_PORT };

        long servertime = 0;
        
        IService1ClientProxy proxy;

        IPEndPoint serverEndPoint = null;  // represent the connection with the server

        private AnalogInput pir_sensor = null;
        private AnalogOutput buzzer_sensor = null;
        private bool scatta_allarme = false;
        private string myMac;
        Bitmap bitmapA = null;
        Int32 RGlobal;
        Int32 PreviousAverage;

        private Boolean StopMe = false;

        /**
         * This method is run when the mainboard is powered up or reset.   
         */
        void ProgramStarted()
        {
            Debug.Print("Program Started");     // to show messages in "Output" window during debugging
            Mainboard.SetDebugLED(true);


            InitSensors();
            
            SetupEthernet();

            ethernetJ11D.NetworkUp += OnNetworkUp;
            ethernetJ11D.NetworkDown += OnNetworkDown;

            camera.PictureCaptured += camera_PictureCaptured;
            camera.BitmapStreamed += camera_BitmapStreamed;
            button.ButtonPressed += button_ButtonPressed;

            camera.StartStreaming();

           

            GT.Timer timer_pir = new GT.Timer(5000);
            timer_pir.Tick += PirDetection;
            timer_pir.Start();   
        }


// ------------------------- Network & Connections ------------------------- //
        void SetupEthernet()
        {
            ethernetJ11D.UseStaticIP(DEFAULT_MY_IP, MASK, DEFAULT_DESTINATION_IP);
            ethernetJ11D.UseThisNetworkInterface();
        }

        /**
         * This method is triggered when the network goes up.
         */ 
        private void OnNetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network up!");
            multicolorLED.TurnGreen();
            ListNetworkInterfaces();

            bindProxyService();
            
            getAddressAndPort();


            // Addressing
            IPAddress ipAddress = IPAddress.Parse(connectionInfo[0]);
            int port = int.Parse( connectionInfo[1] );
            serverEndPoint = new IPEndPoint(ipAddress, port);     

            // Starting keep alive thread
         //   new Thread(this.keepAlive).Start();
        }

      

        private void bindProxyService()
        {
            Debug.Print("Binding proxy service...");
             proxy = new IService1ClientProxy(new WS2007HttpBinding(),new ProtocolVersion11());

            // NOTE: the endpoint needs to match the endpoint of the servicehost
             proxy.EndpointAddress = SERVICE_ADDR;

            Debug.Print("Binding proxy service COMPLETE");

        }

        private void getAddressAndPort()
        {
            var data = proxy.getServerAddressWithPort(new getServerAddressWithPort()
            {
                myMacAddress = myMac,

            });
            connectionInfo[0] = data.getServerAddressWithPortResult.address;
            connectionInfo[1] = data.getServerAddressWithPortResult.port;
            Debug.Print("Server address: " + data.getServerAddressWithPortResult.address);
            Debug.Print("Server port: " + data.getServerAddressWithPortResult.port);
            servertime = Int64.Parse(data.getServerAddressWithPortResult.serverTime);
            Debug.Print("Server time: " + servertime);
        }

        /**
         * This method is triggered when the network goes down.
         * It triggers the alarm.
         */
        private void OnNetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down!");
            multicolorLED.TurnRed();
            StopMe = true;      //stopping keep alive thread
           // ScattaAllarme();
        }

        public void keepAlive()
        {
            Debug.Print("Keep Alive thread: starting...");

            StopMe = false;
            while (! StopMe)
            {
                // call keep alive service
            }

            Debug.Print("Keep Alive thread: terminating gracefully.");
        }
// ----------------------- End Network & Connections ----------------------- //


// -------------------------------- Sensori -------------------------------- //
        private void InitSensors()
        {
            Mainboard.SetDebugLED(true);
            Gadgeteer.Socket socket = Gadgeteer.Socket.GetSocket(9, true, null, null);
            pir_sensor = extender.CreateAnalogInput(Gadgeteer.Socket.Pin.Four);
            buzzer_sensor = extender.CreateAnalogOutput(Gadgeteer.Socket.Pin.Five);
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

        /**
         * This method is triggered when the button is pressed.
         */
        private void button_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            Debug.Print("Button pressed!");
            camera.StopStreaming();
            camera.TakePicture();

            // invio della prima immagine
        }

        private void camera_BitmapStreamed(GTM.GHIElectronics.Camera sender, Bitmap e)
        {            
            displayT35.SimpleGraphics.DisplayImage(e, 0, 0);    
        }

        /**
         * This method is triggered when an image is caught.
         */
        private void camera_PictureCaptured(GTM.GHIElectronics.Camera sender, GT.Picture picture)
        {
            Int32 HeurSum = 0;
            Bitmap bitmapB = picture.MakeBitmap();
            
            Debug.Print("Image captured! " +
                    "Size: " + picture.PictureData.Length.ToString());

            if (bitmapA == null)    //per gestire la prima volta
            {
                bitmapA = bitmapB;
                RGlobal = heuristicSum(bitmapA);
                PreviousAverage = RGlobal / 9;
                return;
            }

            Debug.Print(DateTime.Now.ToString());
            HeurSum = heuristicSum(bitmapB);
            Debug.Print(DateTime.Now.ToString());

            Debug.Print(PreviousAverage.ToString());
            Int32 average = (HeurSum / 9);
            Debug.Print(average.ToString());

            if (System.Math.Abs(PreviousAverage - average) > 45)    //SOGLIA LIMITE 40/50
            {
                Debug.Print("Suspicious picture!");
                sendPicture(picture.PictureData);
            }

            RGlobal = HeurSum;
            PreviousAverage = average;
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);
        } 

        /**
         * This method sum the value of green of 9 squares
         * of dimension 8x8 taken from a bitmap.
         */
        public Int32 heuristicSum(Microsoft.SPOT.Bitmap bitmapB)
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
            Socket clientSocket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
           
                // Connecting
                Debug.Print("Connecting to server " + serverEndPoint + ".");
                clientSocket.Connect(serverEndPoint);
                Debug.Print("Connected to server.");
                byte[] cmd = Encoding.UTF8.GetBytes(MANAGE_IMAGE_COMMAND);

                clientSocket.Send(cmd);
                clientSocket.Send(e);
                clientSocket.Close();
                
/*              byte[] inBuffer = new byte[100];
                int count = clientSocket.Receive(inBuffer);
                char[] chars = Encoding.UTF8.GetChars(inBuffer);
                string str = new string(chars, 0, count);
*/
        }


// --------------------------------- Debug --------------------------------- //
        /**
         * This method prints all the network interfaces.
         */ 
        void ListNetworkInterfaces()
        {
            var settings = ethernetJ11D.NetworkSettings;
            myMac = ByteExtensions.ToHexString(settings.PhysicalAddress, "-");

            Debug.Print("------------------------------------------------");
            Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
            Debug.Print("IP Address:   " + settings.IPAddress);
            Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
            Debug.Print("Subnet Mask:  " + settings.SubnetMask);
            Debug.Print("Gateway:      " + settings.GatewayAddress);
            Debug.Print("------------------------------------------------");
        }

    }

}
