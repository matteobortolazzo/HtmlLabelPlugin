using System;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("HtmlLabel.Forms.Plugin.Shared.Tests")]
namespace LabelHtml.Forms.Plugin.Abstractions
{
    /// <inheritdoc />
    /// <summary>
    /// A label that is able to display HTML content
    /// </summary>
    public class HtmlLabel : Label
    {
        /// <summary>
        /// Identify the UnderlineText property.
        /// </summary>
        public static readonly BindableProperty UnderlineTextProperty =
            BindableProperty.Create(nameof(UnderlineText), typeof(bool), typeof(HtmlLabel), true);

        /// <summary>
        /// Get or set if hyperlinks are underlined.
        /// </summary>
        public bool UnderlineText
        {
            get { return (bool)GetValue(UnderlineTextProperty); }
            set { SetValue(UnderlineTextProperty, value); }
        }

        /// <summary>
        /// Identify the LinkColor property.
        /// </summary>
        public static readonly BindableProperty LinkColorProperty =
            BindableProperty.Create(nameof(LinkColor), typeof(Color), typeof(HtmlLabel), default);

        /// <summary>
        /// Get or set the color of hyperlinks.
        /// </summary>
        public Color LinkColor
        {
            get { return (Color)GetValue(LinkColorProperty); }
            set { SetValue(LinkColorProperty, value); }
        }

        /// <summary>
        /// Identify the BrowserLaunchOptions property.
        /// </summary>
        public static readonly BindableProperty BrowserLaunchOptionsProperty =
            BindableProperty.Create(nameof(BrowserLaunchOptions), typeof(BrowserLaunchOptions), typeof(HtmlLabel), default);

        /// <summary>
        /// Get or set the options to use when opening a web link. <see cref="https://docs.microsoft.com/en-us/xamarin/essentials/open-browser"/>
        /// </summary>
        public BrowserLaunchOptions BrowserLaunchOptions
        {
            get { return (BrowserLaunchOptions)GetValue(BrowserLaunchOptionsProperty); }
            set { SetValue(BrowserLaunchOptionsProperty, value); }
        }

        /// <summary>
        /// Fires before the open URL request is done.
        /// </summary>
        public event EventHandler<WebNavigatingEventArgs> Navigating;

        /// <summary>
        /// Fires when the open URL request is done.
        /// </summary>
        public event EventHandler<WebNavigatingEventArgs> Navigated;

        /// <summary>
        /// Send the Navigating event
        /// </summary>
        /// <param name="args"></param>
        internal void SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        /// <summary>
        /// Send the Navigated event
        /// </summary>
        /// <param name="args"></param>
        internal void SendNavigated(WebNavigatingEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        public bool HtmlLegacyModeEnabled { get; set; } = false;
    }
}