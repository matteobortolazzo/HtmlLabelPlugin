using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.iOS
{
	/// <summary>
	/// HtmlLabel Implementation
	/// </summary>
	[Xamarin.Forms.Internals.Preserve(AllMembers = true)]
	public class HtmlLabelRenderer : BaseTextViewRenderer<HtmlLabel>
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Initialize() { }

		protected override bool NavigateToUrl(NSUrl url)
		{
            if (url == null)
            {
				throw new ArgumentNullException(nameof(url));
            }
			// Try to handle uri, if it can't be handled, fall back to IOS his own handler.
			return !RendererHelper.HandleUriClick(Element, url.AbsoluteString);
		}

		protected override void ProcessText()
		{
			if (string.IsNullOrWhiteSpace(Element?.Text))
			{
				Control.Text = string.Empty;
				return;
			}

			Control.Font = FontExtensions.ToUIFont(Element);
			if (!Element.TextColor.IsDefault)
			{
				Control.TextColor = Element.TextColor.ToUIColor();
			}

			var linkColor = Element.LinkColor;
			if (!linkColor.IsDefault)
			{
				Control.TintColor = linkColor.ToUIColor();
			}
			var isRtl = Device.FlowDirection == FlowDirection.RightToLeft;
			var styledHtml = new RendererHelper(Element, Element.Text, Device.RuntimePlatform, isRtl).ToString();
			SetText(styledHtml);
			SetNeedsDisplay();
		}

		private void SetText(string html)
		{
			// Create HTML data sting
			var stringType = new NSAttributedStringDocumentAttributes
			{
				DocumentType = NSDocumentType.HTML
			};
			var nsError = new NSError();

			var htmlData = NSData.FromString(html, NSStringEncoding.Unicode);

            using var htmlString = new NSAttributedString(htmlData, stringType, out _, ref nsError);
            var mutableHtmlString = htmlString.RemoveTrailingNewLines();

            mutableHtmlString.EnumerateAttributes(new NSRange(0, mutableHtmlString.Length), NSAttributedStringEnumeration.None,
                (NSDictionary value, NSRange range, ref bool stop) =>
                {
                    var md = new NSMutableDictionary(value);
                    var font = md[UIStringAttributeKey.Font] as UIFont;

                    if (Control.Font != null)
                    {
                        md[UIStringAttributeKey.Font] = Control.Font;
                    }
                    else
                    {
                        md[UIStringAttributeKey.Font] = Control.Font.WithTraitsOfFont(font);
                    }

                    var foregroundColor = md[UIStringAttributeKey.ForegroundColor] as UIColor;
                    if (foregroundColor == null || foregroundColor.IsEqualToColor(UIColor.Black))
                    {
                        md[UIStringAttributeKey.ForegroundColor] = Control.TextColor;
                    }
                    mutableHtmlString.SetAttributes(md, range);
                });

            mutableHtmlString.SetLineHeight(Element);
            mutableHtmlString.SetLinksStyles(Element);
            Control.AttributedText = mutableHtmlString;
        }
	}
}