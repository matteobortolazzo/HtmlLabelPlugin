using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Utilities;

namespace HyperTextLabel.Maui.Platforms.Windows
{
    internal static class HtmlLabelExtensions
    {
        public static void UpdateText(this TextBlock view, IHtmlLabel label)
        {
            ProcessText( view, label);
        }

        public static void UpdateUnderlineText(this TextBlock view, IHtmlLabel label)
        {
        }

        public static void UpdateLinkColor(this TextBlock view, IHtmlLabel label)
        {
        }

        public static void UpdateBrowserLaunchOptions(this TextBlock view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidLegacyMode(this TextBlock view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidListIndent(this TextBlock view, IHtmlLabel label)
        {
        }

        private static void ProcessText(TextBlock view, IHtmlLabel label)
        {
            // Gets the complete HTML string
            var isRtl = AppInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft;
            var styledHtml = new RendererHelper(label, label.Text, DevicePlatform.WinUI, isRtl).ToString();
            if (styledHtml == null)
            {
                return;
            }

            view.Text = styledHtml;

            // Adds the HtmlTextBehavior because UWP's TextBlock
            // does not natively support HTML content
            var behavior = new HtmlTextBehavior() { HtmlLabel = label };
            BehaviorCollection behaviors = Interaction.GetBehaviors(view);
            behaviors.Clear();
            behaviors.Add(behavior);
        }
    }
}
