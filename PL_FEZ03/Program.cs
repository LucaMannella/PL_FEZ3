using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using System.Drawing;





using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace PL_FEZ03
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            Mainboard.SetDebugLED(true);
            Thread Bouncer = new Thread(BouncerLoop);
            Bouncer.Start();
            
        }

        void BouncerLoop()
        {
            try
            {
                // Do not redraw
                // while (true)
                //{                         
                // string fileName = @"C:\Users\Yoga2Pro\Desktop\ciao.txt";                             
                //byte[] buff = null;                        
                //FileStream fs = new FileStream(fileName, FileMode.Open,FileAccess.Read);                            
                //long numBytes = new FileInfo(fileName).Length;
                //fs.Read(buff, 0, (int)numBytes);
                //Bitmap a = new Bitmap(buff, Bitmap.BitmapImageType.Gif);
                //displayT35.SimpleGraphics.DisplayImage(a, 100, 100);
                //displayT35.SimpleGraphics.Redraw();                                    
                //Thread.Sleep(30); 
                //}
                if (camera.CameraReady)
                {
                    camera.BitmapStreamed += streamed;
                    camera.PictureCaptured += captured;
                    //camera.TakePicture();
                    camera.StartStreaming();
                    //Thread.Sleep(30);
                    //camera.StopStreaming();                         
                }
            }
            catch (Exception e)
            {
                e.StackTrace.ToString();
            }

        }
        void captured(Camera sender, GT.Picture picture)
        {
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);

        }
        void streamed(Camera sender, Microsoft.SPOT.Bitmap picture)
        {
            displayT35.SimpleGraphics.DisplayImage(picture, 0, 0);
        }
    }
}
