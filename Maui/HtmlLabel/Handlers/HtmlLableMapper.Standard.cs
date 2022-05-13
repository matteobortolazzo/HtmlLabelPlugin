using Microsoft.Maui.Handlers;

namespace LabelHtml.Forms.Plugin
{
    public partial class HtmlLabelHandler : ViewHandler<IHtmlLabel, object>
    {
        protected override object CreatePlatformView() => throw new NotImplementedException();

        public static void MapLabelText(HtmlLabelHandler handler, IHtmlLabel entry) { }

        public static void MapUnderlineText(HtmlLabelHandler handler, IHtmlLabel entry) { }

        public static void MapLinkColor(HtmlLabelHandler handler, IHtmlLabel entry) { }

        public static void MapBrowserLaunchOptions(HtmlLabelHandler handler, IHtmlLabel entry) { }

        public static void MapAndroidLegacyMode(HtmlLabelHandler handler, IHtmlLabel entry) { }

        public static void MapAndroidListIndent(HtmlLabelHandler handler, IHtmlLabel entry) { }
    }
}
