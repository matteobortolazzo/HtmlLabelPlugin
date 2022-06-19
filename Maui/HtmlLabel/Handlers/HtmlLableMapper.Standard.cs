using HyperTextLabel.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace HyperTextLabel.Maui.Handlers
{
    public partial class HtmlLabelHandler : ViewHandler<IHtmlLabel, object>
    {
        protected override object CreatePlatformView() => throw new NotImplementedException();

        public static void MapLabelText(HtmlLabelHandler handler, IHtmlLabel label) { }

        public static void MapUnderlineText(HtmlLabelHandler handler, IHtmlLabel label) { }

        public static void MapLinkColor(HtmlLabelHandler handler, IHtmlLabel label) { }

        public static void MapBrowserLaunchOptions(HtmlLabelHandler handler, IHtmlLabel label) { }

        public static void MapAndroidLegacyMode(HtmlLabelHandler handler, IHtmlLabel label) { }

        public static void MapAndroidListIndent(HtmlLabelHandler handler, IHtmlLabel label) { }
    }
}
