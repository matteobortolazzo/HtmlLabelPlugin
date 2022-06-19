using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HyperTextLabel.Maui.Shared.Tests")]
namespace HyperTextLabel.Maui.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A label that is able to display HTML content
    /// </summary>
    public class HtmlLabel : Label, IHtmlLabel
    {
        /// <summary>
        /// Identify the UnderlineText property.
        /// </summary>
        public static readonly BindableProperty UnderlineTextProperty =
            BindableProperty.Create(nameof(UnderlineText), typeof(bool), typeof(HtmlLabel), true);

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public BrowserLaunchOptions BrowserLaunchOptions
        {
            get { return (BrowserLaunchOptions)GetValue(BrowserLaunchOptionsProperty); }
            set { SetValue(BrowserLaunchOptionsProperty, value); }
        }

        /// <summary>
        /// Identify the AndroidLegacyMode property.
        /// </summary>
        public static readonly BindableProperty AndroidLegacyModeProperty =
            BindableProperty.Create(nameof(AndroidLegacyMode), typeof(bool), typeof(HtmlLabel), default);

        /// <inheritdoc />
        public bool AndroidLegacyMode
        {
            get { return (bool)GetValue(AndroidLegacyModeProperty); }
            set { SetValue(AndroidLegacyModeProperty, value); }
        }

        /// <summary>
        /// Identify the AndroidListIndent property KWI-FIX.
        /// Default value = 20 (to continue support `old value`)
        /// </summary>
        public static readonly BindableProperty AndroidListIndentProperty =
            BindableProperty.Create(nameof(AndroidListIndent), typeof(int), typeof(HtmlLabel), defaultValue: 20);

        /// <inheritdoc />
        public int AndroidListIndent
        {
            get { return (int)GetValue(AndroidListIndentProperty); }
            set { SetValue(AndroidListIndentProperty, value); }
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
        void IHtmlLabelInternals.SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        /// <summary>
        /// Send the Navigated event
        /// </summary>
        /// <param name="args"></param>
        void IHtmlLabelInternals.SendNavigated(WebNavigatingEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }
    }
}