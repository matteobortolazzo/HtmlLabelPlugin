using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Widget;
using Java.Util;
using Plugin.HtmlLabel;
using Plugin.HtmlLabel.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Plugin.HtmlLabel.Android
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
            Control.SetMaxLines(maxLines);
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
                var html = Build.VERSION.SdkInt >= BuildVersionCodes.N
                    ? Html.FromHtml(value, FromHtmlOptions.ModeCompact)
                    : Html.FromHtml(value);
                Control.SetText(html, TextView.BufferType.Spannable);
            }
        }
    }
}