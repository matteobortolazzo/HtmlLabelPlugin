using Microsoft.Maui.Platform;
using LabelHtml.Forms.Plugin.Platforms.iOS;

namespace LabelHtml.Forms.Plugin
{
    public partial class HtmlLabelHandler : Microsoft.Maui.Handlers.LabelHandler
    {
        protected override void ConnectHandler(MauiLabel nativeView)
        {
            base.ConnectHandler(nativeView);
        }

        protected override void DisconnectHandler(MauiLabel nativeView)
        {
            base.DisconnectHandler(nativeView);
        }

        public static void MapLabelText(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateText(entry);
        }

        public static void MapUnderlineText(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateUnderlineText(entry);
        }

        public static void MapLinkColor(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateLinkColor(entry);
        }

        public static void MapBrowserLaunchOptions(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateBrowserLaunchOptions(entry);
        }

        public static void MapAndroidLegacyMode(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateAndroidLegacyMode(entry);
        }

        public static void MapAndroidListIndent(HtmlLabelHandler handler, IHtmlLabel entry)
        {
            handler.PlatformView?.UpdateAndroidListIndent(entry);
        }
    }
}
