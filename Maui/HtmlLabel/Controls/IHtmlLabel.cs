using Microsoft.Maui.Controls.Internals;

namespace HyperTextLabel.Maui.Controls
{
    /// <summary>
    /// Internal class requirments
    /// </summary>
    public interface IHtmlLabelInternals
    {
        void SendNavigating(WebNavigatingEventArgs args);

        void SendNavigated(WebNavigatingEventArgs args);
    }

    /// <summary>
    /// Properties that the HtmlLabel needs the base class to implement.
    /// <see cref="IFontElement"/> is currently a public interface but in an internal namespace and 
    /// EditorBrowsableState.Never so I'm not sure how that's going to play out. I don't want to reference the
    /// Xamarin.Forms interface with MAUI custom control.
    /// </summary>
    public interface IHtmlLabelRequiredProperties : ILabel, IFontElement
    {
        IList<IGestureRecognizer> GestureRecognizers { get; }
    }

    /// <summary>
    /// Need to look into what should be public vs internal.
    /// </summary>
    public interface IHtmlLabel : IHtmlLabelInternals, IHtmlLabelRequiredProperties
    {
        /// <summary>
        /// Get or set if hyperlinks are underlined.
        /// </summary>
        bool UnderlineText { get; }

        /// <summary>
        /// Get or set the color of hyperlinks.
        /// </summary>
        Color LinkColor { get; }

        /// <summary>
        /// Get or set the options to use when opening a web link. <see cref="https://docs.microsoft.com/en-us/xamarin/essentials/open-browser"/>
        /// </summary>
        BrowserLaunchOptions BrowserLaunchOptions { get; }

        /// <summary>
        ///  Get or set if the Android renderer separates block-level elements with blank lines.
        /// </summary>
        bool AndroidLegacyMode { get; }

        /// <summary>
        ///  Get or set if the Android List Indent property KWI-FIX.
        /// </summary>
        int AndroidListIndent { get; }
    }
}
