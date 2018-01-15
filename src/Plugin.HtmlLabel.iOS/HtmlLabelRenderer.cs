using System.ComponentModel;
using Foundation;
using Plugin.HtmlLabel;
using Plugin.HtmlLabel.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Plugin.HtmlLabel.iOS
{
    public class HtmlLabelRenderer : LabelRenderer
    {
        public static void Initialize() { }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;
            UpdateText();
            UpdateMaxLines();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == HtmlLabel.MaxLinesProperty.PropertyName)
                UpdateMaxLines();
            else if (e.PropertyName == Label.TextProperty.PropertyName ||
                e.PropertyName == HtmlLabel.IsHtmlProperty.PropertyName ||
                e.PropertyName == HtmlLabel.RemoveHtmlTagsProperty.PropertyName ||
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

            var isHtml = HtmlLabel.GetIsHtml(Element);
            if (!isHtml) return;


            var attr = new NSAttributedStringDocumentAttributes();
            var nsError = new NSError();
            attr.DocumentType = NSDocumentType.HTML;

            var removeTags = HtmlLabel.GetRemoveHtmlTags(Element);

            var text = removeTags ? 
                HtmlToText.ConvertHtml(Control.Text) : 
                Element.Text;

            var helper = new LabelRendererHelper(Element, text);

            var htmlData = NSData.FromString(helper.ToString(), NSStringEncoding.Unicode);
            Control.AttributedText = new NSAttributedString(htmlData, attr, ref nsError);
            Control.UserInteractionEnabled = true;
            SetNeedsDisplay();
        }
    }
}