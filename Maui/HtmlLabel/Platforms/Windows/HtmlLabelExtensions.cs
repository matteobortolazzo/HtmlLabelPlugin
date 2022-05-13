using Microsoft.UI.Xaml.Controls;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;



namespace LabelHtml.Forms.Plugin.Platforms.Windows
{
    internal static class HtmlLabelExtensions
    {
        public static void UpdateText(this TextBlock view, IHtmlLabel entry)
        {
            ProcessText( view, entry);
        }

        public static void UpdateUnderlineText(this TextBlock view, IHtmlLabel entry)
        {
        }

        public static void UpdateLinkColor(this TextBlock view, IHtmlLabel entry)
        {
        }

        public static void UpdateBrowserLaunchOptions(this TextBlock view, IHtmlLabel entry)
        {
        }

        public static void UpdateAndroidLegacyMode(this TextBlock view, IHtmlLabel entry)
        {
        }

        public static void UpdateAndroidListIndent(this TextBlock view, IHtmlLabel entry)
        {
        }

        private static void ProcessText(TextBlock view, IHtmlLabel entry)
        {
            // Gets the complete HTML string
            var isRtl = AppInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft;
            var styledHtml = new RendererHelper(entry, entry.Text, DevicePlatform.WinUI, isRtl).ToString();
            if (styledHtml == null)
            {
                return;
            }

            view.Text = styledHtml;

            // Adds the HtmlTextBehavior because UWP's TextBlock
            // does not natively support HTML content
            var behavior = new HtmlTextBehavior() { HtmlLabel = entry };
            BehaviorCollection behaviors = Interaction.GetBehaviors(view);
            behaviors.Clear();
            behaviors.Add(behavior);
        }
    }
}
