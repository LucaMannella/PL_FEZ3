using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;

using GT = Gadgeteer;

namespace Client
{
    public abstract class BaseWindow : Canvas
    {
        private const int DEFAULT_WIDTH = 320;
        private const int DEFAULT_HEIGHT = 27;

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
            titleBar.Width = DEFAULT_WIDTH;
            titleBar.Height = DEFAULT_HEIGHT;
            titleBar.Background = backgroundBrush;

            Text text = new Text(font, title);
            text.Width = DEFAULT_WIDTH;
            text.ForeColor = foreColor;
            text.SetMargin(5);
            titleBar.Child = text;

            AddChild(titleBar, 0, 0);

            return titleBar;
        }

        public Border AddStatusBar(UIElement element, GT.Color backgroundColor)
        {
            return AddStatusBar(element, DEFAULT_HEIGHT, backgroundColor, backgroundColor);
        }

        public Border AddStatusBar(UIElement element, int height, GT.Color backgroundColor)
        {
            return AddStatusBar(element, height, backgroundColor, backgroundColor);
        }

        public Border AddStatusBar(UIElement element, GT.Color startColor, GT.Color endColor)
        {
            return AddStatusBar(element, DEFAULT_HEIGHT, startColor, endColor);
        }

        public Border AddStatusBar(UIElement element, int height, GT.Color startColor, GT.Color endColor)
        {
            Brush backgroundBrush = null;
            if (startColor == endColor)
                backgroundBrush = new SolidColorBrush(startColor);
            else
                backgroundBrush = new LinearGradientBrush(startColor, endColor);

            return AddStatusBar(element, DEFAULT_HEIGHT, backgroundBrush);
        }

        public Border AddStatusBar(UIElement element, Brush backgroundBrush)
        {
            return AddStatusBar(element, DEFAULT_HEIGHT, backgroundBrush);
        }

        public Border AddStatusBar(UIElement element, int height, Brush backgroundBrush)
        {
            Border statusBar = new Border();
            statusBar.Width = DEFAULT_WIDTH;
            statusBar.Height = height;
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
