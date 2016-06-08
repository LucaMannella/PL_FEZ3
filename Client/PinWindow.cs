using System;
using System.Runtime.CompilerServices;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;

using GT = Gadgeteer;
using Microsoft.SPOT.Input;

namespace Client
{
    class PinWindow : BaseWindow
    {
        public PinWindow()
        {
            AddTitleBar("Inserire PIN", Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Red, GT.Color.Blue, GT.Color.Blue);
            

        }
    }
}
