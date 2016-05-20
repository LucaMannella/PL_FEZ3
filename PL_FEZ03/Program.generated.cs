//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PL_FEZ03 {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Display T35 module using sockets 14, 13, 12 and 10 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.DisplayT35 displayT35;
        
        /// <summary>The Camera module using socket 3 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Camera camera;
        
        /// <summary>The Multicolor LED module using socket 11 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.MulticolorLED multicolorLED;
        
        /// <summary>The Ethernet J11D module using socket 7 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.EthernetJ11D ethernet;
        
        /// <summary>The USB Client DP module using socket 1 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.USBClientDP usbClientDP;
        
        /// <summary>The Button module using socket 4 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Button button;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZSpider Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZSpider)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZSpider();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.displayT35 = new GTM.GHIElectronics.DisplayT35(14, 13, 12, 10);
            this.camera = new GTM.GHIElectronics.Camera(3);
            this.multicolorLED = new GTM.GHIElectronics.MulticolorLED(11);
         //   this.ethernet = new GTM.GHIElectronics.EthernetJ11D(7);
            this.usbClientDP = new GTM.GHIElectronics.USBClientDP(1);
            this.button = new GTM.GHIElectronics.Button(4);
        }
    }
}
