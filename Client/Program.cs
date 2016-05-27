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

namespace Client
{
    public partial class Program
    {
        Bitmap bitmapA;
        Int32 RGlobal;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
            Mainboard.SetDebugLED(true);
            SetupEthernet();
            camera.PictureCaptured += camera_PictureCaptured;
            button.ButtonPressed += button_ButtonPressed;
            camera.BitmapStreamed += camera_BitmapStreamed;
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkUp += OnNetworkUp;
            ethernetJ11D.NetworkDown += OnNetworkDown;
            camera.StartStreaming();

        }

        private void OnNetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down !!");
            multicolorLED.TurnRed();
        }

        private void OnNetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network up !!");
            multicolorLED.TurnGreen();
            ListNetworkInterfaces();
        }

        private void camera_BitmapStreamed(GTM.GHIElectronics.Camera sender, Bitmap e)
        {
            byte[] b_byte = new byte[e.Width * e.Height * 3 + 54];
            System.Util.BitmapToBMPFile(e.GetBitmap(), e.Width, e.Height, b_byte);
            GT.Picture b_picture = new GT.Picture(b_byte, GT.Picture.PictureEncoding.BMP);
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);
    
           
        }

        private void button_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            Debug.Print("Button pressed");
            camera.StopStreaming();
            camera.TakePicture();
        }

        private void camera_PictureCaptured(GTM.GHIElectronics.Camera sender, GT.Picture picture)
        {
            Debug.Print("image captured");
            Bitmap bitmapB = picture.MakeBitmap();
           
            Debug.Print( picture.PictureData.Length.ToString());
            sss(picture.PictureData);
            Int32 RB = 0;

            if (bitmapA == null)
            {
                //per gestire la prima volta

                bitmapA = bitmapB;
                RGlobal = euristicSum(bitmapA);
                return;
            }

            Debug.Print(DateTime.Now.ToString());
            RB = euristicSum(bitmapB);
            Debug.Print(DateTime.Now.ToString());
            Debug.Print((RGlobal / 9).ToString());
            Debug.Print((RB / 9).ToString());
            //SOGLIA LIMITE 40/50
            RGlobal = RB;
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);
        }

        void ListNetworkInterfaces()
        {
            var settings = ethernetJ11D.NetworkSettings;

            Debug.Print("------------------------------------------------");
            //    Debug.Print("MAC: " + ByteExtensions.ToHexString(settings.PhysicalAddress, "-"));
            Debug.Print("IP Address:   " + settings.IPAddress);
            Debug.Print("DHCP Enabled: " + settings.IsDhcpEnabled);
            Debug.Print("Subnet Mask:  " + settings.SubnetMask);
            Debug.Print("Gateway:      " + settings.GatewayAddress);

            Debug.Print("------------------------------------------------");
        }

        void SetupEthernet()
        {
            //   ethernetJ11D.UseDHCP();
            ethernetJ11D.UseStaticIP(
                "192.168.137.2",
                "255.255.255.0",
                "192.168.137.1");
        }

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

        void sss(byte[] e)
        {

            using (Socket clientSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp))
            {
                // Addressing
                IPAddress ipAddress = IPAddress.Parse("192.168.137.1");
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
