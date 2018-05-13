using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Plugin.HtmlLabel
{
    public class HtmlLabel : Label
    {
        /// <summary>
        /// Backing store for the MaxLines bindable property
        /// </summary>
        public static readonly BindableProperty MaxLinesProperty =
            BindableProperty.CreateAttached("MaxLines", typeof(int), typeof(HtmlLabel), default(int));

        public static int GetMaxLines(BindableObject view)
        {
            if (view == null) return default(int);
            return (int)view.GetValue(MaxLinesProperty);
        }

        public static void SetMaxLines(BindableObject view, int value)
        {
            view?.SetValue(MaxLinesProperty, value);
        }
         
        // Events
        public void SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        public void SendNavigated(WebNavigatingEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        public event EventHandler<WebNavigatingEventArgs> Navigated;
        public event EventHandler<WebNavigatingEventArgs> Navigating;
    }
}