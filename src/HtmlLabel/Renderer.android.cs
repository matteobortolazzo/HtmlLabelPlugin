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
// ReSharper disable once CheckNamespace
namespace LabelHtml.Forms.Plugin.Droid
{
    /// <summary>
    /// HtmlLable Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class HtmlLabelRenderer : LabelRenderer
    {

		private const string TAG_UL_REGEX = "[uU][lL]";
		private const string TAG_OL_REGEX = "[oO][lL]";
		private const string TAG_LI_REGEX = "[lL][iI]";

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
			if (Control == null || Element == null) return;
			if (string.IsNullOrEmpty(Control.Text)) return;

			// Gets the complete HTML string
			var customHtml = new LabelRendererHelper(Element, Control.Text).ToString();
			// Android's TextView doesn't handle <ul>s, <ol>s and <li>s 
			// so it replaces them with <ulc>, <olc> and <lic> respectively.
			// Those tags will be handles by a custom TagHandler
			customHtml = ReplaceTag(customHtml, TAG_UL_REGEX, ListTagHandler.TAG_ULC);
			customHtml = ReplaceTag(customHtml, TAG_OL_REGEX, ListTagHandler.TAG_OLC);
			customHtml = ReplaceTag(customHtml, TAG_LI_REGEX, ListTagHandler.TAG_LIC);

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

	// TagHandler that handles lists (ul, ol)
	internal class ListTagHandler : Java.Lang.Object, Html.ITagHandler
	{
		internal const string TAG_ULC = "ULC";
		internal const string TAG_OLC = "OLC";
		internal const string TAG_LIC = "LIC";

		private ListBuilder _listBuilder = new ListBuilder();

		public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
		{
			tag = tag.ToUpperInvariant();
			if (tag.Equals(TAG_LIC, StringComparison.Ordinal))
			{
				_listBuilder.Li(opening, output);
				return;
			}
			if (opening)
			{
				if (tag.Equals(TAG_OLC, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.StartList(true, output);
					return;
				}
				if (tag.Equals(TAG_ULC, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.StartList(false, output);
					return;
				}
			}
			else
			{
				if (tag.Equals(TAG_OLC, StringComparison.Ordinal) || tag.Equals(TAG_ULC, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.CloseList(output);
				}
				return;
			}
		}
	}

	internal class ListBuilder
	{
		public const int LIST_INTEND = 20;
		private int _liIndex = -1;
		private int _liStart = -1;
		private LiGap _liGap;
		private int _gap = 0;

		private ListBuilder _parent = null;

		internal ListBuilder() : this(null)
		{
		}

		internal ListBuilder(LiGap liGap)
		{
			_parent = null;
			_gap = 0;
			_liGap = liGap ?? GetLiGap(null);
		}

		private ListBuilder(ListBuilder parent, bool ordered)
		{
			_parent = parent;
			_liGap = parent._liGap;
			_gap = parent._gap + LIST_INTEND + _liGap.GetGap(ordered);
			_liIndex = ordered ? 0 : -1;
		}

		internal ListBuilder StartList(bool ordered, IEditable output)
		{
			if (_parent == null)
			{
				if (output.Length() > 0)
				{
					_ = output.Append("\n ");
				}
			}
			return new ListBuilder(this, ordered);
		}

		private bool IsOrdered()
		{
			return _liIndex >= 0;
		}

		internal void Li(bool opening, IEditable output)
		{
			if (opening)
			{
				EnsureParagraphBoundary(output);
				_liStart = output.Length();

				_ = IsOrdered() ? output.Append(++_liIndex + ". ") : output.Append("•  ");
			}
			else
			{
				if (_liStart >= 0)
				{
					EnsureParagraphBoundary(output);
					using var leadingMarginSpan = new LeadingMarginSpanStandard(_gap - _liGap.GetGap(IsOrdered()), _gap);
					output.SetSpan(leadingMarginSpan, _liStart, output.Length(), SpanTypes.ExclusiveExclusive);
					_liStart = -1;
				}
			}
		}


		internal ListBuilder CloseList(IEditable output)
		{
			EnsureParagraphBoundary(output);
			ListBuilder result = _parent;
			if (result == null)
			{
				result = this;
			}

			if (result._parent == null)
			{
				_ = output.Append('\n');
			}

			return result;
		}

		private static void EnsureParagraphBoundary(IEditable output)
		{
			if (output.Length() == 0)
			{
				return;
			}

			var lastChar = output.CharAt(output.Length() - 1);
			if (lastChar != '\n')
			{
				_ = output.Append('\n');
			}
		}

		internal class LiGap
		{
			private readonly int _orderedGap;
			private readonly int _unorderedGap;

			internal LiGap(int orderedGap, int unorderedGap)
			{
				_orderedGap = orderedGap;
				_unorderedGap = unorderedGap;
			}

			public int GetGap(bool ordered)
			{
				return ordered ? _orderedGap : _unorderedGap;
			}
		}

		internal static LiGap GetLiGap(TextView tv)
		{
			return tv == null ? 
				new LiGap(40, 30) : 
				new LiGap(ComputeWidth(tv, true), ComputeWidth(tv, false));
		}

		private static int ComputeWidth(TextView tv, bool ordered)
		{
			Android.Graphics.Paint paint = tv.Paint;

			//paint.setTypeface(tv.getPaint().getTypeface());
			//paint.setTextSize(tv.getPaint().getTextSize());

			// Now compute!
			using var bounds = new Android.Graphics.Rect();
			var myString = ordered ? "99. " : "• ";
			paint.GetTextBounds(myString, 0, myString.Length, bounds);
			var width = bounds.Width();
			var pt = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Pt, width, tv.Context.Resources.DisplayMetrics);
			var sp = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Sp, width, tv.Context.Resources.DisplayMetrics);
			var dip = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, width, tv.Context.Resources.DisplayMetrics);
			var px = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Px, width, tv.Context.Resources.DisplayMetrics);
			var mm = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Mm, width, tv.Context.Resources.DisplayMetrics);
			return (int)pt;
		}

	}
}
