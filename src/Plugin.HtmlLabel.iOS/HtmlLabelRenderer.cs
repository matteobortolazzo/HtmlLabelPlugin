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
            UpdateMaxLines();
            UpdateText();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Label.TextProperty.PropertyName ||
                e.PropertyName == HtmlLabel.IsHtmlProperty.PropertyName ||
                e.PropertyName == HtmlLabel.RemoveHtmlTagsProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == HtmlLabel.MaxLinesProperty.PropertyName)
                UpdateMaxLines();
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

            var removeTags = HtmlLabel.GetRemoveHtmlTags(Element);
            if (removeTags)
            {
                Control.Text = HtmlToText.ConvertHtml(Control.Text);
            }
            else
            {
                var value = Element.Text ?? string.Empty;
                var attr = new NSAttributedStringDocumentAttributes();
                var nsError = new NSError();
                attr.DocumentType = NSDocumentType.HTML;

                var myHtmlData = NSData.FromString(value, NSStringEncoding.Unicode);
                Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
            }
            SetNeedsDisplay();
        }
    }
}