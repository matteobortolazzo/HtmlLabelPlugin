using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using PreserveAttribute = Microsoft.Maui.Controls.Internals.PreserveAttribute;
using FontExtensions = Microsoft.Maui.Controls.Platform.FontExtensions;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.iOS
{
    /// <summary>
    /// HtmlLabel Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
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

            Control.Font = FontExtensions.ToUIFont( Element );

			var linkColor = Element.LinkColor;
			if (!linkColor.IsDefault())
			{
				Control.TintColor = linkColor.ToUIColor();
			}
			var isRtl = AppInfo.RequestedLayoutDirection == Microsoft.Maui.ApplicationModel.LayoutDirection.RightToLeft;
			var styledHtml = new RendererHelper(Element, Element.Text, DevicePlatform.iOS, isRtl).ToString();
			SetText(styledHtml);
			SetNeedsDisplay();
		}

		private void SetText(string html)
		{
			// Create HTML data sting
			var stringType = new NSAttributedStringDocumentAttributes
			{
                DocumentType = NSDocumentType.HTML,
                StringEncoding = NSStringEncoding.UTF8
			};
			var nsError = new NSError();

			var htmlData = NSData.FromString(html, NSStringEncoding.Unicode);

            using var htmlString = new NSAttributedString(htmlData, stringType, out _, ref nsError);
            var mutableHtmlString = htmlString.RemoveTrailingNewLines();

            mutableHtmlString.EnumerateAttributes(new NSRange(0, mutableHtmlString.Length), NSAttributedStringEnumeration.None,
                (NSDictionary value, NSRange range, ref bool stop) =>
                {
                    try
					{
						var md = new NSMutableDictionary( value );
                        var font = md[ UIStringAttributeKey.Font ] as UIFont;

                        if ( font != null )
                        {
                            md[ UIStringAttributeKey.Font ] = Control.Font.WithTraitsOfFont( font );
                        }
                        else
                        {
                            md[ UIStringAttributeKey.Font ] = Control.Font;
                        }

                        var foregroundColor = md[ UIStringAttributeKey.ForegroundColor ] as UIColor;
                        if ( foregroundColor == null || foregroundColor.IsEqualToColor( UIColor.Black ) )
                        {
                            md[ UIStringAttributeKey.ForegroundColor ] = Control.TextColor;
                        }

                        mutableHtmlString.SetAttributes( md, range );
                    }
					catch ( Exception e )
                    {
                        Console.WriteLine( e );

                        throw;
                    }
                });

            mutableHtmlString.SetLineHeight(Element);
            mutableHtmlString.SetLinksStyles(Element);
            Control.AttributedText = mutableHtmlString;
        }
	}
}