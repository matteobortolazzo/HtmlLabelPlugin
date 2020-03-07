using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.iOS;
using UIKit;

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
				System.Diagnostics.Debug.WriteLine(@$"ERROR: ${ex.Message}");
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
					System.Diagnostics.Debug.WriteLine(@$"ERROR: ${ex.Message}");
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

			var styledHtml = new RendererHelper(Element, Control.Text).ToString();
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
			var mutableHtmlString = new NSMutableAttributedString(htmlString);

			SetLinksStyles(element, mutableHtmlString);
			control.AttributedText = mutableHtmlString;
			control.HandleLinkTap(element);
		}

		private static void SetLinksStyles(HtmlLabel element, NSMutableAttributedString mutableHtmlString)
		{
			using var linkAttributeName = new NSString("NSLink");
			var linkAttributes = new UIStringAttributes();
			if (!element.UnderlineText)
			{
				linkAttributes.UnderlineStyle = NSUnderlineStyle.None;
			};
			if (!element.LinkColor.IsDefault)
			{
				linkAttributes.ForegroundColor = element.LinkColor.ToUIColor();
			};

			mutableHtmlString.EnumerateAttribute(linkAttributeName, new NSRange(0, mutableHtmlString.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired,
				(NSObject value, NSRange range, ref bool stop) =>
				{
					if (value != null && value is NSUrl)
					{
						mutableHtmlString.AddAttributes(linkAttributes, range);
					}
				});
		}
	}
}