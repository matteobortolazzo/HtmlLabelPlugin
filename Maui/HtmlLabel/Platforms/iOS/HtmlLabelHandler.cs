using Microsoft.Maui.Platform;
using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Extensions;
using HyperTextLabel.Maui.Platforms.iOS;

using PlatformView = Microsoft.Maui.Platform.MauiLabel;

namespace HyperTextLabel.Maui.Handlers
{
    public partial class HtmlLabelHandler : Microsoft.Maui.Handlers.LabelHandler
    {
        protected override void ConnectHandler(PlatformView platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(PlatformView platformView)
        {
            base.DisconnectHandler(platformView);
        }

        public static void MapLabelText(HtmlLabelHandler handler, IHtmlLabel label)
        {
            var fontManager = handler.GetRequiredService<IFontManager>();

            handler.PlatformView?.UpdateText(label, fontManager);
        }

        public static void MapUnderlineText(HtmlLabelHandler handler, IHtmlLabel label)
        {
            handler.PlatformView?.UpdateUnderlineText(label);
        }

        public static void MapLinkColor(HtmlLabelHandler handler, IHtmlLabel label)
        {
            handler.PlatformView?.UpdateLinkColor(label);
        }

        public static void MapBrowserLaunchOptions(HtmlLabelHandler handler, IHtmlLabel label)
        {
            handler.PlatformView?.UpdateBrowserLaunchOptions(label);
        }

        public static void MapAndroidLegacyMode(HtmlLabelHandler handler, IHtmlLabel label)
        {
            handler.PlatformView?.UpdateAndroidLegacyMode(label);
        }

        public static void MapAndroidListIndent(HtmlLabelHandler handler, IHtmlLabel label)
        {
            handler.PlatformView?.UpdateAndroidListIndent(label);
        }
    }
}
