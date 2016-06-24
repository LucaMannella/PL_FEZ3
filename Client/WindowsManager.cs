using System;
using Microsoft.SPOT;
using GHI.Glide.Display;
using GHI.Glide;

namespace Client
{
    public class WindowsManager
    {

        private static WindowsManager instance;
        private static Program mProgram;

        private WindowsManager(){
            
        }

        public static WindowsManager getInstance(Program p){

            if(instance == null){
                mProgram = p;
                instance = new WindowsManager();
            }
            return instance;

        }

        private static GHI.Glide.UI.PasswordBox password;
        

        public static void showWindowInsertPin(){


            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_pin));

            GlideTouch.Initialize();
            
            GHI.Glide.UI.Button btn1 = (GHI.Glide.UI.Button)window.GetChildByName("button1");
            GHI.Glide.UI.Button btn2 = (GHI.Glide.UI.Button)window.GetChildByName("button2");
            GHI.Glide.UI.Button btn3 = (GHI.Glide.UI.Button)window.GetChildByName("button3");
            GHI.Glide.UI.Button btn4 = (GHI.Glide.UI.Button)window.GetChildByName("button4");
            GHI.Glide.UI.Button btn5 = (GHI.Glide.UI.Button)window.GetChildByName("button5");
            GHI.Glide.UI.Button btn6 = (GHI.Glide.UI.Button)window.GetChildByName("button6");
            GHI.Glide.UI.Button btn7 = (GHI.Glide.UI.Button)window.GetChildByName("button7");
            GHI.Glide.UI.Button btn8 = (GHI.Glide.UI.Button)window.GetChildByName("button8");
            GHI.Glide.UI.Button btn9 = (GHI.Glide.UI.Button)window.GetChildByName("button9");
            GHI.Glide.UI.Button btn0 = (GHI.Glide.UI.Button)window.GetChildByName("button0");
            GHI.Glide.UI.Button btnOk = (GHI.Glide.UI.Button)window.GetChildByName("button_ok");
            GHI.Glide.UI.Button btnDel = (GHI.Glide.UI.Button)window.GetChildByName("button_del");
            password = (GHI.Glide.UI.PasswordBox)window.GetChildByName("textpassword");
            password.Enabled = true;
           
            btn1.TapEvent += btn1_TapEvent;
            btn2.TapEvent += btn2_TapEvent;
            btn3.TapEvent += btn3_TapEvent;
            btn4.TapEvent += btn4_TapEvent;
            btn5.TapEvent += btn5_TapEvent;
            btn6.TapEvent += btn6_TapEvent;
            btn7.TapEvent += btn7_TapEvent;
            btn8.TapEvent += btn8_TapEvent;
            btn9.TapEvent += btn9_TapEvent;
            btn0.TapEvent += btn0_TapEvent;
            btnDel.TapEvent += btnDel_TapEvent;
            btnOk.TapEvent += btnOk_TapEvent;
            
            Glide.MainWindow = window;
        }

        static void btnOk_TapEvent(object sender)
        {
            
            //Todo check pin with service
            Boolean ret = false;
            String pass = password.Text;
            if (pass.Length == 8)
            {
               ret = mProgram.checkLogin(pass);
            }
            else
            {
                //visualizzare pin corto
            }
            if (ret)
            {
                showWindowSetupCamera();
            }
            else
            {
                //visualizzare pin errato
            }
            
           // showWindowSetupCamera();
        }

    

        static void btnDel_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length > 0)
            {
                pass = pass.Substring(0, pass.Length - 1);
                password.Text = pass;
                password.Invalidate();
            }
            
        }

        static void btn0_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "0";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn9_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "9";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn8_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "8";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn7_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8){
                pass = pass + "7";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn6_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "6";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn5_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "5";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn4_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "4";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn3_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "3";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn2_TapEvent(object sender)
        {
            String pass = password.Text;
            if (pass.Length < 8)
            {
                pass = pass + "2";
                password.Text = pass;
                password.Invalidate();
            }
        }

        static void btn1_TapEvent(object sender)
        {
           
             String pass = password.Text;
             if (pass.Length < 8)
             {
                 pass = pass + "1";
                 password.Text = pass;
                 password.Invalidate();
             }
            
        }

        public static void showWindowSetupCamera()
        {


            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_setup_camera));
            GHI.Glide.UI.Button btnNext = (GHI.Glide.UI.Button)window.GetChildByName("btn_next");

            btnNext.TapEvent += btnNext_TapEvent;
            GlideTouch.Initialize();
            Glide.MainWindow = window;
        }

        public static GHI.Glide.UI.ProgressBar showWindowProgress()
        {


            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_loading));
            GHI.Glide.UI.ProgressBar progress = (GHI.Glide.UI.ProgressBar)window.GetChildByName("progress");

            progress.Enabled = true;
            progress.MaxValue = 100;

            GlideTouch.Initialize();
            Glide.MainWindow = window;
            return progress;
        }

        public static void showWindowNetworkDown()
        {

            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_network_down));
            GHI.Glide.UI.Button btnRetry = (GHI.Glide.UI.Button)window.GetChildByName("btn_retry");

            btnRetry.TapEvent += btnRetry_TapEvent;
            GlideTouch.Initialize();
            Glide.MainWindow = window;
        }

        public static void showWindowErrorPin()
        {

            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_error_pin));
            GHI.Glide.UI.Button btnRetryerrorpin = (GHI.Glide.UI.Button)window.GetChildByName("retry");

            btnRetryerrorpin.TapEvent += btnRetryerrorpin_TapEvent;
            GlideTouch.Initialize();
            Glide.MainWindow = window;
        }

        public static void showWindowNotRegistered()
        {

            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_not_regitered));
            GHI.Glide.UI.Button btnBack = (GHI.Glide.UI.Button)window.GetChildByName("back");

            btnBack.TapEvent += btnRetryerrorpin_TapEvent;
            GlideTouch.Initialize();
            Glide.MainWindow = window;
        }

        public static void showWindowErrorService()
        {

            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_error_service));
 
            Glide.MainWindow = window;
        }

        public static void showWindowErrorServer()
        {

            Window window = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.window_error_server));

            Glide.MainWindow = window;
        }
       

        static void btnRetryerrorpin_TapEvent(object sender)
        {
            showWindowInsertPin();
        }

        static void btnRetry_TapEvent(object sender)
        {

        }

        static void btnNext_TapEvent(object sender)
        {
            mProgram.setupCamera();
        }

    }
}
