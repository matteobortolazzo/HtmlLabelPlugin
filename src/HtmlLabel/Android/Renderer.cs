using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Widget;
using Java.Lang;
using LabelHtml.Forms.Plugin.Droid;
using LabelHtml.Forms.Plugin.PlatformSpecifics.Android;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using PluginHtmlLabel = LabelHtml.Forms.Plugin.Abstractions;

[assembly: ExportRenderer(typeof(PluginHtmlLabel.HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.Droid
{
    /// <summary>
    /// HtmlLable Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class HtmlLabelRenderer : LabelRenderer
    {
        private const string _tagUlRegex = "[uU][lL]";
        private const string _tagOlRegex = "[oO][lL]";
        private const string _tagLiRegex = "[lL][iI]";

        /// <summary>
        /// Create an instance of the renderer.
        /// </summary>
        /// <param name="context"></param>
        public HtmlLabelRenderer(Android.Content.Context context) : base(context) { }

        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Initialize() { }

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e == null || e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                ProcessText();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@$"ERROR: ${ex.Message}");
            }
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e != null && PluginHtmlLabel.RendererHelper.RequireProcess(e.PropertyName))
            {
                try
                {
                    ProcessText();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(@$"ERROR: ${ex.Message}");
                }
            }
        }

        private void ProcessText()
        {
            if (Control == null || Element == null)
            {
                return;
            }

            Color linkColor = ((PluginHtmlLabel.HtmlLabel)Element).LinkColor;
            if (!linkColor.IsDefault)
            {
                Control.SetLinkTextColor(linkColor.ToAndroid());
            }

            Control.SetIncludeFontPadding(false);
            var isRtl = Device.FlowDirection == FlowDirection.RightToLeft;
            var styledHtml = new PluginHtmlLabel.RendererHelper(Element, Control.Text, Device.RuntimePlatform, isRtl).ToString();
            /* 
			 * Android's TextView doesn't support lists.
			 * List tags must be replaces with custom tags,
			 * that it will be renderer by a custom tag handler.
			 */
            styledHtml = styledHtml
                ?.ReplaceTag(_tagUlRegex, ListTagHandler.TagUl)
                ?.ReplaceTag(_tagOlRegex, ListTagHandler.TagOl)
                ?.ReplaceTag(_tagLiRegex, ListTagHandler.TagLi);

            if (styledHtml != null)
            {
                SetText((PluginHtmlLabel.HtmlLabel)Element, Control, styledHtml);
            }
        }

        private void SetText(PluginHtmlLabel.HtmlLabel element, TextView control, string html)
        {

            // Set the type of content and the custom tag list handler
            using var listTagHandler = new ListTagHandler();
            ISpanned sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
                Html.FromHtml(html, element.On<Xamarin.Forms.PlatformConfiguration.Android>().GetHtmlLegacyModeEnabled() ? FromHtmlOptions.ModeLegacy : FromHtmlOptions.ModeCompact, null, listTagHandler) :
                Html.FromHtml(html, null, listTagHandler);
            using var strBuilder = new SpannableStringBuilder(sequence);

            // Make clickable links
            if (!Element.GestureRecognizers.Any())
            {
                control.MovementMethod = LinkMovementMethod.Instance;
                URLSpan[] urls = strBuilder
                .GetSpans(0, sequence.Length(), Class.FromType(typeof(URLSpan)))
                .Cast<URLSpan>()
                .ToArray();
                foreach (URLSpan span in urls)
                {
                    MakeLinkClickable(strBuilder, span);
                }
            }

            // Android adds an unnecessary "\n" that must be removed
            using ISpanned value = RemoveLastChar(strBuilder);

            // Finally sets the value of the TextView 
            control.SetText(value, TextView.BufferType.Spannable);
        }

        private void MakeLinkClickable(ISpannable strBuilder, URLSpan span)
        {
            var start = strBuilder.GetSpanStart(span);
            var end = strBuilder.GetSpanEnd(span);
            SpanTypes flags = strBuilder.GetSpanFlags(span);
            var clickable = new HtmlLabelClickableSpan((PluginHtmlLabel.HtmlLabel)Element, span);
            strBuilder.SetSpan(clickable, start, end, flags);
            strBuilder.RemoveSpan(span);
        }

        private static ISpanned RemoveLastChar(ICharSequence text)
        {
            var builder = new SpannableStringBuilder(text);
            if (text.Length() != 0)
            {
                _ = builder.Delete(text.Length() - 1, text.Length());
            }

            return builder;
        }

        private class HtmlLabelClickableSpan : ClickableSpan
        {
            private readonly PluginHtmlLabel.HtmlLabel _label;
            private readonly URLSpan _span;

            public HtmlLabelClickableSpan(PluginHtmlLabel.HtmlLabel label, URLSpan span)
            {
                _label = label;
                _span = span;
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = _label.UnderlineText;
            }

            public override void OnClick(Android.Views.View widget)
            {
                PluginHtmlLabel.RendererHelper.HandleUriClick(_label, _span.URL);
            }
        }
    }
}
