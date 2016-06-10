/* Copyright 2011 Marco Minerva, marco.minerva@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;

namespace MainWindowExample
{
    public class MainWindow : Canvas
    {
        public Border AddTitleBar(string title, Font font, GT.Color foreColor, GT.Color backgroundColor)
        {
            return AddTitleBar(title, font, foreColor, backgroundColor, backgroundColor);
        }

        public Border AddTitleBar(string title, Font font, GT.Color foreColor, GT.Color startColor, GT.Color endColor)
        {
            Brush backgroundBrush = null;
            if (startColor == endColor)
                backgroundBrush = new SolidColorBrush(startColor);
            else
                backgroundBrush = new LinearGradientBrush(startColor, endColor);

            return AddTitleBar(title, font, foreColor, backgroundBrush);
        }

        public Border AddTitleBar(string title, Font font, GT.Color foreColor, Brush backgroundBrush)
        {
            Border titleBar = new Border();
            titleBar.Width = 320;
            titleBar.Height = 27;
            titleBar.Background = backgroundBrush;
            
            Text text = new Text(font, title);
            text.Width = 320;
            text.ForeColor = foreColor;
            text.SetMargin(5);
            titleBar.Child = text;

            AddChild(titleBar, 0, 0);

            return titleBar;
        }

        public Border AddStatusBar(UIElement element, GT.Color backgroundColor)
        {
            return AddStatusBar(element, backgroundColor, backgroundColor);
        }

        public Border AddStatusBar(UIElement element, GT.Color startColor, GT.Color endColor)
        {
            Brush backgroundBrush = null;
            if (startColor == endColor)
                backgroundBrush = new SolidColorBrush(startColor);
            else
                backgroundBrush = new LinearGradientBrush(startColor, endColor);

            return AddStatusBar(element, backgroundBrush);
        }

        public Border AddStatusBar(UIElement element, Brush backgroundBrush)
        {
            Border statusBar = new Border();
            statusBar.Width = 320;
            statusBar.Height = 27;
            statusBar.Background = backgroundBrush;

            int left, top, right, bottom;
            element.GetMargin(out left, out top, out right, out bottom);
            left = System.Math.Max(left, 5);
            top = System.Math.Max(top, 5);
            bottom = System.Math.Max(bottom, 5);
            element.SetMargin(left, top, right, bottom);
            statusBar.Child = element;

            AddChild(statusBar, 215, 0);

            return statusBar;
        }

        public void AddChild(UIElement element, int top, int left)
        {
            Children.Add(element);
            Canvas.SetTop(element, top);
            Canvas.SetLeft(element, left);
        }
    }
}
