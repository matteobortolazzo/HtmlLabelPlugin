using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Foundation;
using CoreGraphics;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
namespace LabelHtml.Forms.Plugin.iOS
{
	/// <summary>
	/// HtmlLable Implementation
	/// </summary>
	[Xamarin.Forms.Internals.Preserve(AllMembers = true)]
	public class HtmlLabelRenderer : LabelRenderer
	{
		private class LinkData
		{
			public LinkData(NSRange range, string url) { Range = range; Url = url; }
			public readonly NSRange Range;
			public readonly string Url;
		}

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

			if (Control == null) return;

			UpdateText();
			UpdateMaxLines();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == HtmlLabel.MaxLinesProperty.PropertyName)
				UpdateMaxLines();
			else if (e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FontAttributesProperty.PropertyName ||
				e.PropertyName == Label.FontFamilyProperty.PropertyName ||
				e.PropertyName == Label.FontSizeProperty.PropertyName ||
				e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
				e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateText();
		}

		private void UpdateMaxLines()
		{
			var maxLines = HtmlLabel.GetMaxLines(Element);
			if (maxLines == default(int)) return;
			Control.Lines = maxLines;

			SetNeedsDisplay();
		}

		private void UpdateText()
		{
			if (Control == null || Element == null) return;

			if (string.IsNullOrEmpty(Control.Text)) return;
		
			// Gets the complete HTML string
			var helper = new LabelRendererHelper(Element, Control.Text);

			try
			{
				CreateAttributedString(Control, helper.ToString());
				SetNeedsDisplay();
			}
			catch
			{
				// ignored
			}
		}

		private void CreateAttributedString(UILabel control, string html)
		{
			var attr = new NSAttributedStringDocumentAttributes();
			var nsError = new NSError();
			attr.DocumentType = NSDocumentType.HTML;
			// --------------
			// 02-01-2018 : Fix for default font family => https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/9
			var fontDescriptor = control.Font.FontDescriptor.VisibleName;
			var fontFamily = fontDescriptor.ToLower().Contains("system") ? "-apple-system,system-ui,BlinkMacSystemFont,Segoe UI" : control.Font.FamilyName;
			html += "<style> body{ font-family: " + fontFamily + ";}</style>";
			// --------------
			var myHtmlData = NSData.FromString(html, NSStringEncoding.Unicode);
			// control.Lines = 0;
			var mutable = new NSMutableAttributedString(new NSAttributedString(myHtmlData, attr, ref nsError));

			if (mutable.MutableString.HasSuffix(new NSString("\n")))
			{
				mutable.DeleteRange(new NSRange(mutable.MutableString.Length - 1, 1));
			}

			var links = new List<LinkData>();
			control.AttributedText = mutable;

			// Makes a list of all links:
			mutable.EnumerateAttributes(new NSRange(0, mutable.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, (NSDictionary attrs, NSRange range, ref bool stop) =>
			{
				foreach (var a in attrs) // should use attrs.ContainsKey(something) instead
				{
					if (a.Key.ToString() != "NSLink") continue;
					links.Add(new LinkData(range, a.Value.ToString()));
					return;
				}
			});

			// Sets up a Gesture recognizer:
			if (links.Count <= 0) return;
			control.UserInteractionEnabled = true;
			var tapGesture = new UITapGestureRecognizer((tap) =>
			{
				var url = DetectTappedUrl(tap, (UILabel)tap.View, links);
				if (url == null) return;

				var label = (HtmlLabel)Element;
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
				label.SendNavigating(args);

				if (args.Cancel)
					return;

				Device.OpenUri(new Uri(url));
				label.SendNavigated(args);
			});
			control.AddGestureRecognizer(tapGesture);
		}

		private string DetectTappedUrl(UIGestureRecognizer tap, UILabel label, IEnumerable<LinkData> linkList)
		{
			// Creates instances of NSLayoutManager, NSTextContainer and NSTextStorage
			var layoutManager = new NSLayoutManager();
			var textContainer = new NSTextContainer();
			var textStorage = new NSTextStorage();
			textStorage.SetString(label.AttributedText);

			// Configures layoutManager and textStorage
			layoutManager.AddTextContainer(textContainer);
			textStorage.AddLayoutManager(layoutManager);

			// Configures textContainer
			textContainer.LineFragmentPadding = 0;
			textContainer.LineBreakMode = label.LineBreakMode;
			textContainer.MaximumNumberOfLines = (nuint)label.Lines;
			var labelSize = label.Bounds.Size;
			textContainer.Size = labelSize;

			// Finds the tapped character location and compare it to the specified range
			var locationOfTouchInLabel = tap.LocationInView(label);
			var textBoundingBox = layoutManager.GetUsedRectForTextContainer(textContainer);
			var textContainerOffset = new CGPoint((labelSize.Width - textBoundingBox.Size.Width) * 0.0 - textBoundingBox.Location.X,
				(labelSize.Height - textBoundingBox.Size.Height) * 0.0 - textBoundingBox.Location.Y);

			nfloat labelX;
			switch (Element.HorizontalTextAlignment)
			{
				case TextAlignment.End:
					labelX = locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width);
					break;
				case TextAlignment.Center:
					labelX = locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width) / 2;
					break;
				default:
					labelX = locationOfTouchInLabel.X;
					break;
			}
			var locationOfTouchInTextContainer = new CGPoint(labelX - textContainerOffset.X, locationOfTouchInLabel.Y - textContainerOffset.Y);

			nfloat partialFraction = 0;
			var indexOfCharacter = (nint)layoutManager.CharacterIndexForPoint(locationOfTouchInTextContainer, textContainer, ref partialFraction);

			nint scaledIndexOfCharacter = 0;
			// Problem is that method CharacterIndexForPoint always returns index based on UILabel font
			// ".SFUIText" which is the default Helvetica iOS font
			// HACK is to scale indexOfCharacter for 13% because NeoSans-Light is narrower font than ".SFUIText"
			if (label.Font.Name == "NeoSans-Light")
			{
				scaledIndexOfCharacter = (nint)(indexOfCharacter * 1.13);
			}

			// HelveticaNeue font family works perfect until character position in the string is more than 2000 chars
			// some uncosnsistent behaviour
			// if string has <b> tag than label.Font.Name from HelveticaNeue-Thin goes to HelveticaNeue-Bold
			if (label.Font.Name.StartsWith("HelveticaNeue", StringComparison.InvariantCulture))
			{
				scaledIndexOfCharacter = (nint)(indexOfCharacter * 1.02);
			}

			foreach (var link in linkList)
			{
				var rangeLength = link.Range.Length;
				var tolerance = 0;
				if (label.Font.Name == "NeoSans-Light")
				{
					rangeLength = (nint)(rangeLength * 1.13);
					tolerance = 25;
					indexOfCharacter = scaledIndexOfCharacter;
				}

				if (label.Font.Name.StartsWith("HelveticaNeue", StringComparison.InvariantCulture))
				{
					if (link.Range.Location > 2000)
					{
						indexOfCharacter = scaledIndexOfCharacter;
					}
				}

				// Xamarin version of NSLocationInRange?
				if ((indexOfCharacter >= (link.Range.Location - tolerance)) && (indexOfCharacter < (link.Range.Location + rangeLength + tolerance)))
				{
					return link.Url;
				}
			}
			return null;
		}
	}
}
