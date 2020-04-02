using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;
using System.Linq;

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

			if (e == null || e.OldElement != null || Element == null)
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
			NSMutableAttributedString mutableHtmlString = RemoveTrailingNewLine(htmlString);			

			SetLinksStyles(element, mutableHtmlString);
			control.AttributedText = mutableHtmlString;

			if (!Element.GestureRecognizers.Any())
			{
				control.HandleLinkTap(element);
			}
		}

		private static NSMutableAttributedString RemoveTrailingNewLine(NSAttributedString htmlString)
		{
			NSAttributedString lastCharRange = htmlString.Substring(htmlString.Length - 1, 1);
			if (lastCharRange.Value == "\n")
			{
				htmlString = htmlString.Substring(0, htmlString.Length - 1);
			}

			return new NSMutableAttributedString(htmlString);
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