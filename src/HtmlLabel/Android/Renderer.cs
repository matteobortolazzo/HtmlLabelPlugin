using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Widget;
using Java.Lang;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.Droid;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
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

			if (Control == null)
			{
				return;
			}

			UpdateText();
		}

		/// <inheritdoc />
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

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
			var customHtml = new RendererHelper(Element, Control.Text).ToString();
			// Android's TextView doesn't handle <ul>s, <ol>s and <li>s 
			// so it replaces them with <ulc>, <olc> and <lic> respectively.
			// Those tags will be handles by a custom TagHandler
			customHtml = ReplaceTag(customHtml, _tagUlRegex, ListTagHandler.TagUlc);
			customHtml = ReplaceTag(customHtml, _tagOlRegex, ListTagHandler.TagOlc);
			customHtml = ReplaceTag(customHtml, _tagLiRegex, ListTagHandler.TagLic);

			Control.SetIncludeFontPadding(false);

			SetTextViewHtml(Control, customHtml);
		}

		private static string ReplaceTag(string html, string tag, string newTag)
		{
			return Regex.Replace(html, @"(<\s*\/?\s*)" + tag + @"((\s+[\w\-\,\.\(\)\=""\:\;]*)*>)", "$1" + newTag + "$2");
		}

		private void SetTextViewHtml(TextView text, string html)
		{
			// Tells the TextView that the content is HTML and adds a custom TagHandler
			using var listTagHanlder = new ListTagHandler();
			ISpanned sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
				Html.FromHtml(html, FromHtmlOptions.ModeCompact, null, listTagHanlder) :
#pragma warning disable 618
				Html.FromHtml(html, null, listTagHanlder);
#pragma warning restore 618

			// Makes clickable links
			text.MovementMethod = LinkMovementMethod.Instance;
			using  var strBuilder = new SpannableStringBuilder(sequence);
			Java.Lang.Object[] urls = strBuilder.GetSpans(0, sequence.Length(), Class.FromType(typeof(URLSpan)));
			foreach (Java.Lang.Object span in urls)
			{
				MakeLinkClickable(strBuilder, (URLSpan)span);
			}

			// Android adds an unnecessary "\n" that must be removed
			using ISpanned value = RemoveLastChar(strBuilder);

			// Finally sets the value of the TextView 
			text.SetText(value, TextView.BufferType.Spannable);
		}

	    private void MakeLinkClickable(ISpannable strBuilder, URLSpan span)
		{
			var start = strBuilder.GetSpanStart(span);
			var end = strBuilder.GetSpanEnd(span);
			SpanTypes flags = strBuilder.GetSpanFlags(span);
			using var clickable = new MyClickableSpan((HtmlLabel)Element, span);
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

			public override void OnClick(Android.Views.View widget)
			{
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = _span.URL }, _span.URL);
				_label.SendNavigating(args);

				if (args.Cancel)
				{
					return;
				}

				_ = Launcher.TryOpenAsync(new Uri(_span.URL));
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
}
