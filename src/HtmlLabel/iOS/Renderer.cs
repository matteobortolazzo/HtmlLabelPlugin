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
using Xamarin.Essentials;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.iOS
{
	/// <summary>
	/// HtmlLable Implementation
	/// </summary>
	[Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class HtmlLabelRenderer : LabelRenderer
    {
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

			var styledHtml = new RendererHelper(Element, Control.Text).ToString();

			CreateAttributedString(Control, styledHtml);
            SetNeedsDisplay();
        }

		private void CreateAttributedString(UILabel control, string html)
		{
			var attr = new NSAttributedStringDocumentAttributes();
			var nsError = new NSError();
			attr.DocumentType = NSDocumentType.HTML;
			
			var fontDescriptor = control.Font.FontDescriptor.VisibleName;
			var fontFamily = fontDescriptor.ToUpperInvariant().Contains("SYSTEM", StringComparison.Ordinal) ? "-apple-system,system-ui,BlinkMacSystemFont,Segoe UI" : control.Font.FamilyName;
			html += "<style> body{ font-family: " + fontFamily + ";}</style>";
			
			var myHtmlData = NSData.FromString(html, NSStringEncoding.Unicode);

			var attributedString = new NSAttributedString(myHtmlData, attr, ref nsError);
			var mutable = new NSMutableAttributedString();

			using (var newLine = new NSString("\n"))
			{
				if (mutable.MutableString.HasSuffix(newLine))
				{
					mutable.DeleteRange(new NSRange(mutable.MutableString.Length - 1, 1));
				}
			}			

			var links = new List<LinkData>();
			control.AttributedText = mutable;

			// Makes a list of all links:
			mutable.EnumerateAttributes(new NSRange(0, mutable.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, (NSDictionary attrs, NSRange range, ref bool stop) =>
			{
				foreach (KeyValuePair<NSObject, NSObject> a in attrs) // should use attrs.ContainsKey(something) instead
				{
					if (a.Key.ToString() != "NSLink")
                    {
                        continue;
                    }

                    links.Add(new LinkData(range, a.Value.ToString()));
					return;
				}
			});

			// Sets up a Gesture recognizer:
			if (links.Count <= 0)
            {
                return;
            }

            control.UserInteractionEnabled = true;

			void TapLinkAction(UITapGestureRecognizer tap)
			{
				var url = DetectTappedUrl(tap, (UILabel)tap.View, links);
				if (url == null)
				{
					return;
				}

				var label = (HtmlLabel)Element;
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
				label.SendNavigating(args);

				if (args.Cancel)
				{
					return;
				}

				Launcher.OpenAsync(new Uri(url)).GetAwaiter().GetResult();
				label.SendNavigated(args);
			}

			var tapGesture = new UITapGestureRecognizer(TapLinkAction);
			control.AddGestureRecognizer(tapGesture);
		}

		private string DetectTappedUrl(UIGestureRecognizer tap, UILabel label, IEnumerable<LinkData> linkList)
		{
			// Creates instances of NSLayoutManager, NSTextContainer and NSTextStorage
			using var layoutManager = new NSLayoutManager();
			using var textContainer = new NSTextContainer();
			using var textStorage = new NSTextStorage();
			textStorage.SetString(label.AttributedText);

			// Configures layoutManager and textStorage
			layoutManager.AddTextContainer(textContainer);
			textStorage.AddLayoutManager(layoutManager);

			// Configures textContainer
			textContainer.LineFragmentPadding = 0;
			textContainer.LineBreakMode = label.LineBreakMode;
			textContainer.MaximumNumberOfLines = (nuint)label.Lines;
            CGSize labelSize = label.Bounds.Size;
			textContainer.Size = labelSize;

            // Finds the tapped character location and compare it to the specified range
            CGPoint locationOfTouchInLabel = tap.LocationInView(label);
            CGRect textBoundingBox = layoutManager.GetUsedRectForTextContainer(textContainer);
			var textContainerOffset = new CGPoint((labelSize.Width - textBoundingBox.Size.Width) * 0.0 - textBoundingBox.Location.X,
				(labelSize.Height - textBoundingBox.Size.Height) * 0.0 - textBoundingBox.Location.Y);

			nfloat labelX = Element.HorizontalTextAlignment switch
			{
				TextAlignment.End => locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width),
				TextAlignment.Center => locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width) / 2,
				_ => locationOfTouchInLabel.X,
			};
			var locationOfTouchInTextContainer = new CGPoint(labelX - textContainerOffset.X, locationOfTouchInLabel.Y - textContainerOffset.Y);

            var indexOfCharacter = (nint)layoutManager.GetCharacterIndex(locationOfTouchInTextContainer, textContainer);

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

			foreach (LinkData link in linkList)
			{
                nint rangeLength = link.Range.Length;
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

		private class LinkData
		{
			public LinkData(NSRange range, string url) { Range = range; Url = url; }
			public readonly NSRange Range;
			public readonly string Url;
		}
	}
}