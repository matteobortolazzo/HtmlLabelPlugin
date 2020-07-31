using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;
using System.Linq;
using System;

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
			base.OnElementChanged(e);

			if (e == null || Element == null)
			{
				return;
			}

			try
			{
				ProcessText();
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
			}
		}

		/// <inheritdoc />
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e != null && RendererHelper.RequireProcess(e.PropertyName))
			{
				try
				{
					ProcessText();
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
				}
			}
		}

		private void ProcessText()
		{
			if (Control == null || Element == null)
            {
                return;
            }

			Color linkColor = ((HtmlLabel)Element).LinkColor;
			if (!linkColor.IsDefault)
			{
				Control.TintColor = linkColor.ToUIColor();
			}

			var isRtl = Device.FlowDirection == FlowDirection.RightToLeft;
			var styledHtml = new RendererHelper(Element, Control.Text, Device.RuntimePlatform, isRtl).ToString();
			if (styledHtml != null)
			{
				SetText(Control, styledHtml);
				SetNeedsDisplay();
			}
		}		

		private void SetText(UILabel control, string html)
		{
			var element = (HtmlLabel)Element;

			// Create HTML data sting
			var stringType = new NSAttributedStringDocumentAttributes
			{			
				DocumentType = NSDocumentType.HTML
			};
			var nsError = new NSError();
			var htmlData = NSData.FromString(html, NSStringEncoding.Unicode);
			using var htmlString = new NSAttributedString(htmlData, stringType, ref nsError);
			NSMutableAttributedString mutableHtmlString = RemoveTrailingNewLines(htmlString);

			SetLineHeight(element, mutableHtmlString);
			SetLinksStyles(element, mutableHtmlString);
			control.AttributedText = mutableHtmlString;

			if (!Element.GestureRecognizers.Any())
			{
				control.HandleLinkTap(element);
			}
		}

		private static NSMutableAttributedString RemoveTrailingNewLines(NSAttributedString htmlString)
		{
			var count = 0;
			for (int i = 1; i <= htmlString.Length; i++)
			{
				if ("\n" != htmlString.Substring(htmlString.Length - i, 1).Value)
					break;

				count++;
			}

			if (count > 0)
				htmlString = htmlString.Substring(0, htmlString.Length - count);

			return new NSMutableAttributedString(htmlString);
		}

		private static void SetLineHeight(HtmlLabel element, NSMutableAttributedString mutableHtmlString)
		{
			if (element.LineHeight < 0)
			{
				return;
			}

			var lineHeightStyle = new NSMutableParagraphStyle
			{
				LineHeightMultiple = (nfloat)element.LineHeight
			};
			mutableHtmlString.AddAttribute(new NSString("NSParagraphStyle"), lineHeightStyle, new NSRange(0, mutableHtmlString.Length));
		}

		private static void SetLinksStyles(HtmlLabel element, NSMutableAttributedString mutableHtmlString)
		{			
			using var linkAttributeName = new NSString("NSLink");
			UIStringAttributes linkAttributes = null;

			if (!element.UnderlineText)
			{
				linkAttributes ??= new UIStringAttributes();
				linkAttributes.UnderlineStyle = NSUnderlineStyle.None;
			};
			if (!element.LinkColor.IsDefault)
			{
				linkAttributes ??= new UIStringAttributes();
				linkAttributes.ForegroundColor = element.LinkColor.ToUIColor();
			};

			mutableHtmlString.EnumerateAttribute(linkAttributeName, new NSRange(0, mutableHtmlString.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired,
				(NSObject value, NSRange range, ref bool stop) =>
				{
					if (value != null && value is NSUrl)
					{
						// Replace the standard NSLink because iOS does not change the aspect of it otherwise.
						mutableHtmlString.AddAttribute(LinkTapHelper.CustomLinkAttribute, value, range);
						mutableHtmlString.RemoveAttribute("NSLink", range);

						// Applies the style
						if (linkAttributes != null)
						{
							mutableHtmlString.AddAttributes(linkAttributes, range);
						}
					}
				});
		}		
	}
}
