using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Plugin.HtmlLabel;
using Plugin.HtmlLabel.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Plugin.HtmlLabel.UWP
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
            Control.MaxLines = maxLines;
        }

        private void UpdateText()
        {
            if (Control == null || Element == null) return;

            var isHtml = HtmlLabel.GetIsHtml(Element);
            if (!isHtml) return;

            var removeTags = HtmlLabel.GetRemoveHtmlTags(Element);

            var text = removeTags ?
                HtmlToText.ConvertHtml(Control.Text) :
                Element.Text;

            var helper = new LabelRendererHelper(Element, text);
            Control.Text = helper.ToString();

            var behavior = new HtmlTextBehavior();
            Interaction.GetBehaviors(Control).Add(behavior);
        }
    }
}
