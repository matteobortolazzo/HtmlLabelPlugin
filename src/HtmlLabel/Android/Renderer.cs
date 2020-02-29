using System;
using System.ComponentModel;
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
			ProcessText();
			base.OnElementChanged(e);
		}

		/// <inheritdoc />
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e != null && RendererHelper.RequireProcess(e.PropertyName))
			{
				ProcessText();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void ProcessText()
		{
			if (Control == null || Element == null)
			{
				return;
			}

			Control.SetIncludeFontPadding(false);

			var styledHtml = new RendererHelper(Element, Control.Text).ToString();
			/* Android's TextView doesn't support lists.
			 * List tags must be replaces with custom tags,
			 * that it will be renderer by a custom tag handler.
			 */
			styledHtml = styledHtml
				.ReplaceTag(_tagUlRegex, ListTagHandler.TagUl)
				.ReplaceTag(_tagOlRegex, ListTagHandler.TagOl)
				.ReplaceTag(_tagLiRegex, ListTagHandler.TagLi);

			SetTextViewHtml(Control, styledHtml);
		}

		private void SetTextViewHtml(TextView control, string html)
		{
			// Set the type of content and the custom tag list handler
			using var listTagHandler = new ListTagHandler();
			ISpanned sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
				Html.FromHtml(html, FromHtmlOptions.ModeCompact, null, listTagHandler) :
				Html.FromHtml(html, null, listTagHandler);

			// Make clickable links
			control.MovementMethod = LinkMovementMethod.Instance;
			using  var strBuilder = new SpannableStringBuilder(sequence);
			Java.Lang.Object[] urls = strBuilder.GetSpans(0, sequence.Length(), Class.FromType(typeof(URLSpan)));
			foreach (Java.Lang.Object span in urls)
			{
				MakeLinkClickable(strBuilder, (URLSpan)span);
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
			var clickable = new HtmlLabelClickableSpan((HtmlLabel)Element, span);
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
			private readonly HtmlLabel _label;
			private readonly URLSpan _span;

			public HtmlLabelClickableSpan(HtmlLabel label, URLSpan span)
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
	}
}
