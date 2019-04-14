using System;
using System.ComponentModel;
using System.Globalization;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Widget;
using Java.Lang;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.Droid;
using Org.Xml.Sax;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
namespace LabelHtml.Forms.Plugin.Droid
{
    /// <summary>
    /// HtmlLable Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class HtmlLabelRenderer : LabelRenderer
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public HtmlLabelRenderer(Android.Content.Context context) : base(context) { }
		
	    /// <summary>
	    /// Used for registration with dependency service
	    /// </summary>
	    public static void Initialize() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
            {
                return;
            }

            UpdateText();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FontAttributesProperty.PropertyName ||
				e.PropertyName == Label.FontFamilyProperty.PropertyName ||
				e.PropertyName == Label.FontSizeProperty.PropertyName ||
				e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
				e.PropertyName == Label.TextColorProperty.PropertyName)
            {
                UpdateText();
            }
        }

		private void UpdateText()
		{
			if (Control == null || Element == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Control.Text))
            {
                return;
            }

            // Gets the complete HTML string
            var customHtml = new LabelRendererHelper(Element, Control.Text).ToString();
			// Android's TextView doesn't handle <ul>s, <ol>s and <li>s 
			// so it replaces them with <ulc>, <olc> and <lic> respectively.
			// Those tags will be handles by a custom TagHandler
			customHtml = customHtml
                .Replace("ul>", "ulc>", StringComparison.Ordinal)
                .Replace("ol>", "olc>", StringComparison.Ordinal)
                .Replace("li>", "lic>", StringComparison.Ordinal);

			Control.SetIncludeFontPadding(false);

			SetTextViewHtml(Control, customHtml);
		}

		private void SetTextViewHtml(TextView text, string html)
		{
            // Tells the TextView that the content is HTML and adds a custom TagHandler
            ISpanned sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
				Html.FromHtml(html, FromHtmlOptions.ModeCompact, null, new ListTagHandler()) :
#pragma warning disable 618
				Html.FromHtml(html, null, new ListTagHandler());
#pragma warning restore 618

			// Makes clickable links
			text.MovementMethod = LinkMovementMethod.Instance;
			var strBuilder = new SpannableStringBuilder(sequence);
            Java.Lang.Object[] urls = strBuilder.GetSpans(0, sequence.Length(), Class.FromType(typeof(URLSpan)));
			foreach (Java.Lang.Object span in urls)
            {
                MakeLinkClickable(strBuilder, (URLSpan)span);
            }

            // Android adds an unnecessary "\n" that must be removed
            ISpanned value = RemoveLastChar(strBuilder);

			// Finally sets the value of the TextView 
			text.SetText(value, TextView.BufferType.Spannable);
		}

	    private void MakeLinkClickable(ISpannable strBuilder, URLSpan span)
		{
			var start = strBuilder.GetSpanStart(span);
			var end = strBuilder.GetSpanEnd(span);
            SpanTypes flags = strBuilder.GetSpanFlags(span);
			var clickable = new MyClickableSpan((HtmlLabel)Element, span);
			strBuilder.SetSpan(clickable, start, end, flags);
			strBuilder.RemoveSpan(span);
		}

		private class MyClickableSpan : ClickableSpan
		{
			private readonly HtmlLabel _label;
			private readonly URLSpan _span;

			public MyClickableSpan(HtmlLabel label, URLSpan span)
			{
				_label = label;
				_span = span;
			}

			public override void OnClick(global::Android.Views.View widget)
			{
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = _span.URL }, _span.URL);
				_label.SendNavigating(args);

				if (args.Cancel)
                {
                    return;
                }

                Device.OpenUri(new Uri(_span.URL));
				_label.SendNavigated(args);
			}
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
	}

	// TagHandler that handles lists (ul, ol)
	internal class ListTagHandler : Java.Lang.Object, Html.ITagHandler
	{
		private bool _first = true;
		private string _parent;
		private int _index = 1;

		public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
		{
			if (tag.Equals("ulc", StringComparison.Ordinal))
			{
				_parent = "ulc";
				_index = 1;
			}
			else if (tag.Equals("olc", StringComparison.Ordinal))
			{
				_parent = "olc";
				_index = 1;
			}

			if (!tag.Equals("lic", StringComparison.Ordinal))
            {
                return;
            }

            var lastChar = (char)0;
			if (output.Length() > 0)
			{
				lastChar = output.CharAt(output.Length() - 1);
			}
			if (_parent.Equals("ulc", StringComparison.Ordinal))
			{
				if (_first)
				{
                    _ = output.Append(lastChar == '\n' ? "\t•  " : "\n\t•  ");
					_first = false;
				}
				else
                {
                    _first = true;
                }
            }
			else
			{
				if (_first)
				{
                    if  (lastChar != '\n')
                    {
                        _ = output.Append("\n");
                    }
                    _ = output.Append("\t" + _index + ". ");

                    _first = false;
					_index++;
				}
				else
                {
                    _first = true;
                }
            }
		}
	}
}