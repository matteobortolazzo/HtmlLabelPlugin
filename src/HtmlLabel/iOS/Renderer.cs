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

			var styledHtml = new RendererHelper(Element, Control.Text).ToString();
			if (styledHtml != null)
			{
				SetText(Control, styledHtml);
				SetNeedsDisplay();
			}
		}		

		private void SetText(UILabel control, string html)
		{
			// Create HTML data sting
			var stringType = new NSAttributedStringDocumentAttributes
			{
				DocumentType = NSDocumentType.HTML
			};
			var nsError = new NSError();
			var htmlData = NSData.FromString(html, NSStringEncoding.Unicode);
			var htmlString = new NSAttributedString(htmlData, stringType, ref nsError);
			var mutableHtmlString = new NSMutableAttributedString(htmlString);
			
			using var newLine = new NSString("\n");
			if (mutableHtmlString.MutableString.HasSuffix(newLine))
			{
				mutableHtmlString.DeleteRange(new NSRange(mutableHtmlString.MutableString.Length - 1, 1));
			}

			var element = (HtmlLabel)Element;
			control.AttributedText = mutableHtmlString;
			control.HandleLinkTap(element);
		}
	}
}