using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Java.Lang;
using HyperTextLabel.Maui.Platform.Droid;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Platform;
using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Utilities;
using HyperTextLabel.Maui.Extensions;

namespace HyperTextLabel.Maui.Platforms.Droid
{
    internal static class HtmlLabelExtensions
    {
        private const string _tagUlRegex = "[uU][lL]";
        private const string _tagOlRegex = "[oO][lL]";
        private const string _tagLiRegex = "[lL][iI]";

        public static void UpdateText(this AppCompatTextView view, IHtmlLabel label)
        {
            Color linkColor = label.LinkColor;
            if (!linkColor.IsDefault())
            {
                view.SetLinkTextColor(linkColor.ToPlatform());
            }

            view.SetIncludeFontPadding(false);
            var isRtl = AppInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft;
            var styledHtml = new RendererHelper(label, label.Text, DevicePlatform.Android, isRtl).ToString();
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
                SetText(view, label, styledHtml);
            }
        }

        public static void UpdateUnderlineText(this AppCompatTextView view, IHtmlLabel label)
        {
        }

        public static void UpdateLinkColor(this AppCompatTextView view, IHtmlLabel label)
        {
        }

        public static void UpdateBrowserLaunchOptions(this AppCompatTextView view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidLegacyMode(this AppCompatTextView view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidListIndent(this AppCompatTextView view, IHtmlLabel label)
        {
        }

        private static void SetText(AppCompatTextView control, IHtmlLabel htmlLabel, string html)
        {
            // Set the type of content and the custom tag list handler
            using var listTagHandler = new ListTagHandler(htmlLabel.AndroidListIndent); // KWI-FIX: added AndroidListIndent parameter
            var imageGetter = new UrlImageParser(control);
            FromHtmlOptions fromHtmlOptions = htmlLabel.AndroidLegacyMode ? FromHtmlOptions.ModeLegacy : FromHtmlOptions.ModeCompact;
            ISpanned sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
                Html.FromHtml(html, fromHtmlOptions, imageGetter, listTagHandler) :
                Html.FromHtml(html, imageGetter, listTagHandler);
            using var strBuilder = new SpannableStringBuilder(sequence);

            // Make clickable links
            if (!htmlLabel.GestureRecognizers.Any())
            {
                control.MovementMethod = LinkMovementMethod.Instance;
                URLSpan[] urls = strBuilder
                    .GetSpans(0, sequence.Length(), Class.FromType(typeof(URLSpan)))
                    .Cast<URLSpan>()
                    .ToArray();
                foreach (URLSpan span in urls)
                {
                    MakeLinkClickable(strBuilder, span, htmlLabel);
                }
            }

            // Android adds an unnecessary "\n" that must be removed
            using ISpanned value = RemoveTrailingNewLines(strBuilder);

            // Finally sets the value of the TextView 
            control.SetText(value, global::Android.Widget.TextView.BufferType.Spannable);
        }

        private static ISpanned RemoveTrailingNewLines(ICharSequence text)
        {
            var builder = new SpannableStringBuilder(text);

            var count = 0;
            for (int i = 1; i <= text.Length(); i++)
            {
                if (!'\n'.Equals(text.CharAt(text.Length() - i)))
                    break;

                count++;
            }

            if (count > 0)
                _ = builder.Delete(text.Length() - count, text.Length());

            return builder;
        }

        private static void MakeLinkClickable(ISpannable strBuilder, URLSpan span, IHtmlLabel htmlLabel)
        {
            var start = strBuilder.GetSpanStart(span);
            var end = strBuilder.GetSpanEnd(span);
            SpanTypes flags = strBuilder.GetSpanFlags(span);
            var clickable = new HtmlLabelClickableSpan(htmlLabel, span);
            strBuilder.SetSpan(clickable, start, end, flags);
            strBuilder.RemoveSpan(span);
        }

        private class HtmlLabelClickableSpan : ClickableSpan
        {
            private readonly IHtmlLabel _label;
            private readonly URLSpan _span;

            public HtmlLabelClickableSpan(IHtmlLabel label, URLSpan span)
            {
                _label = label;
                _span = span;
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = _label.UnderlineText;
            }

            public override void OnClick(global::Android.Views.View widget)
            {
                RendererHelper.HandleUriClick(_label, _span.URL);
            }
        }
    }
}
