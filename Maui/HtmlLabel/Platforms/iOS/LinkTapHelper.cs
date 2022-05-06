using CoreGraphics;
using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using System.Runtime.InteropServices;
using UIKit;

namespace LabelHtml.Forms.Plugin.iOS
{
	internal static class LinkTapHelper
	{
		public static readonly NSString CustomLinkAttribute = new NSString("LabelLink");

		public static void HandleLinkTap(this UILabel control, HtmlLabel element)
		{
			void TapHandler(UITapGestureRecognizer tap)
			{
				var detectedUrl = DetectTappedUrl(tap, (UILabel)tap.View);
				RendererHelper.HandleUriClick(element, detectedUrl);
			}

			var tapGesture = new UITapGestureRecognizer(TapHandler);
			control.AddGestureRecognizer(tapGesture);
			control.UserInteractionEnabled = true;
		}

		private static string DetectTappedUrl(UIGestureRecognizer tap, UILabel control)
		{
			CGRect bounds = control.Bounds;
			NSAttributedString attributedText = control.AttributedText;

			// Setup containers
			using var textContainer = new NSTextContainer(bounds.Size)
			{
				LineFragmentPadding = 0,
				LineBreakMode = control.LineBreakMode,
				MaximumNumberOfLines = (nuint)control.Lines
			};

			using var layoutManager = new NSLayoutManager();
			layoutManager.AddTextContainer(textContainer);

			using var textStorage = new NSTextStorage();
			textStorage.SetString(attributedText);

			using var fontAttributeName = new NSString("NSFont");
			var textRange = new NSRange(0, control.AttributedText.Length);
			textStorage.AddAttribute(fontAttributeName, control.Font, textRange);
			textStorage.AddLayoutManager(layoutManager);	
			CGRect textBoundingBox = layoutManager.GetUsedRect(textContainer);

			// Calculate align offset
			static NFloat GetAlignOffset(UITextAlignment textAlignment) => textAlignment switch
				{
					UITextAlignment.Center => 0.5f,
					UITextAlignment.Right => 1f,
					_ => 0.0f,
				};
			NFloat alignmentOffset = GetAlignOffset(control.TextAlignment);
			NFloat xOffset = (bounds.Size.Width - textBoundingBox.Size.Width) * alignmentOffset - textBoundingBox.Location.X;
			NFloat yOffset = (bounds.Size.Height - textBoundingBox.Size.Height) * alignmentOffset - textBoundingBox.Location.Y;

			// Find tapped character
			CGPoint locationOfTouchInLabel = tap.LocationInView(control);
			var locationOfTouchInTextContainer = new CGPoint(locationOfTouchInLabel.X - xOffset, locationOfTouchInLabel.Y - yOffset);
			var characterIndex = (nint)layoutManager.GetCharacterIndex(locationOfTouchInTextContainer, textContainer);

			if (characterIndex >= attributedText.Length)
			{
				return null;
			}

			// Try to get the URL
			NSObject linkAttributeValue = attributedText.GetAttribute(CustomLinkAttribute, characterIndex, out NSRange range);
			return linkAttributeValue is NSUrl url ? url.AbsoluteString : null;
		}
	}
}
