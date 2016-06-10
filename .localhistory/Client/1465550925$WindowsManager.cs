using System;
using Microsoft.SPOT;
using GHI.Glide.Display;
using GHI.Glide;

namespace Client
{
    public static class WindowsManager
    {

        public static void setupWindowInsertPin(){


            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window));

            GlideTouch.Initialize();
            
            GHI.Glide.UI.Button btn1 = (GHI.Glide.UI.Button)window.GetChildByName("button1");
            GHI.Glide.UI.Button btn2 = (GHI.Glide.UI.Button)window.GetChildByName("button2");
            GHI.Glide.UI.Button btn3 = (GHI.Glide.UI.Button)window.GetChildByName("button3");
            GHI.Glide.UI.Button btn4 = (GHI.Glide.UI.Button)window.GetChildByName("button4");
            GHI.Glide.UI.Button btn5 = (GHI.Glide.UI.Button)window.GetChildByName("button5");
            GHI.Glide.UI.Button btn6 = (GHI.Glide.UI.Button)window.GetChildByName("button6");
            GHI.Glide.UI.Button btn7 = (GHI.Glide.UI.Button)window.GetChildByName("button7");
            GHI.Glide.UI.Button btn8 = (GHI.Glide.UI.Button)window.GetChildByName("button8");
            btn1.TapEvent += btn1_TapEvent;
            
            Glide.MainWindow = window;
        }

        static void btn1_TapEvent(object sender)
        {
            

        }

    }
}
